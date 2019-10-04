// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlassDoorExternalSearchProvider.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the GlassDoorExternalSearchProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Crawling.Helpers;
using CluedIn.ExternalSearch.Filters;
using CluedIn.ExternalSearch.Providers.Glassdoor.Model;
using CluedIn.ExternalSearch.Providers.Glassdoor.Vocabularies;

using RestSharp;

namespace CluedIn.ExternalSearch.Providers.Glassdoor
{
    /// <summary>The glass door external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    public class GlassDoorExternalSearchProvider : ExternalSearchProviderBase
    {
        /**********************************************************************************************************
         * FIELDS
         **********************************************************************************************************/

        /// <summary>The shared API tokens</summary>
        private List<GlassDoorApiToken> sharedApiTokens = new List<GlassDoorApiToken>()
            {
                // ConfigurationManager.AppSettings["Providers.ExternalSearch.Facebook.ApiToken"];

                // TIW
                // "WUPFfVaKUi", 
                // new GlassDoorApiToken(89797, "WUPFfVaKUi"), 

                // MSH HYLDAHLNET
                new GlassDoorApiToken(134530, "kkAUVCSZxcM"), 
            };

        /// <summary>The shared API tokens index</summary>
        private int sharedApiTokensIdx = 0;

        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/

        /// <summary>
        /// Initializes a new instance of the <see cref="GlassDoorExternalSearchProvider" /> class.
        /// </summary>
        public GlassDoorExternalSearchProvider()
            : base(Constants.ExternalSearchProviders.GlassDoorId, EntityType.Organization)
        {
        }

        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        /// <summary>Builds the queries.</summary>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        /// <returns>The search queries.</returns>
        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request)
        {
            if (!this.Accepts(request.EntityMetaData.EntityType))
                yield break;

            var existingResults = request.GetQueryResults<Employer>(this).ToList();

            Func<string, bool> nameFilter = value => OrganizationFilters.NameFilter(context, value) || existingResults.Any(r => string.Equals(r.Data.name, value, StringComparison.InvariantCultureIgnoreCase));

            // Query Input
            var entityType = request.EntityMetaData.EntityType;
            var organizationName = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, new HashSet<string>());

            if (!string.IsNullOrEmpty(request.EntityMetaData.Name))
                organizationName.Add(request.EntityMetaData.Name);
            if (!string.IsNullOrEmpty(request.EntityMetaData.DisplayName))
                organizationName.Add(request.EntityMetaData.DisplayName);
           
            if (organizationName != null)
            {
                var values = organizationName.GetOrganizationNameVariants()
                                             .Select(NameNormalization.Normalize)
                                             .ToHashSet();

                foreach (var value in values.Where(v => !nameFilter(v)))
                    yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
            }
        }

        /// <summary>Executes the search.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <returns>The results.</returns>
        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query)
        {
            var name = query.QueryParameters[ExternalSearchQueryParameter.Name].FirstOrDefault();

            if (string.IsNullOrEmpty(name))
                yield break;

            var client = new RestClient("http://api.glassdoor.com/api");
            client.UserAgent = "Mozilla/5.0";

            GlassDoorApiToken sharedApiToken;

            lock (this)
            {
                sharedApiToken = this.sharedApiTokens[this.sharedApiTokensIdx++];

                if (this.sharedApiTokensIdx >= this.sharedApiTokens.Count)
                    this.sharedApiTokensIdx = 0;
            }

            var request = new RestRequest(string.Format("api.htm?t.p={0}&t.k={1}&format=json&v=1&action=employers&q={2}", sharedApiToken.PartnerId, sharedApiToken.Key, name), Method.GET);

            var response = client.ExecuteTaskAsync<GlassDoorResponse>(request).Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if(response.Data?.response?.employers == null)
                    yield break;

                foreach (var result in response.Data.response.employers)
                    yield return new ExternalSearchQueryResult<Employer>(query, result);
            }
            else if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
                yield break;
            else if (response.ErrorException != null)
                throw new AggregateException(response.ErrorException.Message, response.ErrorException);
            else
                throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode);
        }

        /// <summary>Builds the clues.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The clues.</returns>
        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<Employer>();

            var clues = new List<Clue>();

            var code = this.GetOriginEntityCode(resultItem);

            var clue = new Clue(code, context.Organization);

            this.PopulateMetadata(clue.Data.EntityData, resultItem);

            this.DownloadPreviewImage(context, resultItem.Data.squareLogo, clue);

            if (resultItem.Data.ceo != null && !string.IsNullOrEmpty(resultItem.Data.ceo.name))
            {
                var personCode = this.GetPersonOriginEntityCode(resultItem);

                var personClue = new Clue(personCode, context.Organization);

                this.PopulatePersonMetadata(personClue.Data.EntityData, resultItem);

                if (resultItem.Data.ceo.image != null)
                    this.DownloadPreviewImage(context, resultItem.Data.ceo.image.src, personClue);

                clues.Add(personClue);
            }

            clues.Add(clue);

            return clues;
        }

        /// <summary>Gets the primary entity metadata.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The primary entity metadata.</returns>
        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<Employer>();
            return this.CreateMetadata(resultItem);
        }

        /// <summary>Gets the preview image.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The preview image.</returns>
        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            return this.DownloadPreviewImageBlob<Employer>(context, result, r => r.Data.squareLogo);
        }

        /// <summary>Creates the metadata.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The metadata.</returns>
        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<Employer> resultItem)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem);

            return metadata;
        }

        /// <summary>Gets the origin entity code.</summary>
        /// <param name="resultItem">The result item.</param>
        /// <returns>The origin entity code.</returns>
        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<Employer> resultItem)
        {
            return new EntityCode(EntityType.Organization, this.GetCodeOrigin(), resultItem.Data.id);
        }

        private EntityCode GetPersonOriginEntityCode(IExternalSearchQueryResult<Employer> resultItem)
        {
            return new EntityCode(EntityType.Person, this.GetCodeOrigin(), resultItem.Data.id + "|" + resultItem.Data.ceo.name);
        }

        /// <summary>Gets the code origin.</summary>
        /// <returns>The code origin</returns>
        private CodeOrigin GetCodeOrigin()
        {
            return CodeOrigin.CluedIn.CreateSpecific("glassDoor");
        }

        /// <summary>Populates the metadata.</summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="resultItem">The result item.</param>
        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<Employer> resultItem)
        {
            var code = this.GetOriginEntityCode(resultItem);

            metadata.EntityType = EntityType.Organization;
            metadata.Name       = resultItem.Data.name;
            metadata.OriginEntityCode = code;

            if (!string.IsNullOrEmpty(resultItem.Data.website))
                metadata.Codes.Add(new EntityCode(EntityType.Organization, CodeOrigin.CluedIn.CreateSpecific("website"), resultItem.Data.website));

            metadata.Codes.Add(code);

            metadata.Properties[GlassDoorVocabulary.Organization.CareerOpportunitiesRating]     = resultItem.Data.careerOpportunitiesRating;
            metadata.Properties[GlassDoorVocabulary.Organization.CompensationAndBenefitsRating] = resultItem.Data.compensationAndBenefitsRating;
            metadata.Properties[GlassDoorVocabulary.Organization.CultureAndValuesRating]        = resultItem.Data.cultureAndValuesRating;
            metadata.Properties[GlassDoorVocabulary.Organization.Industry]                      = resultItem.Data.industry;
            metadata.Properties[GlassDoorVocabulary.Organization.IndustryId]                    = resultItem.Data.industryId.PrintIfAvailable();
            metadata.Properties[GlassDoorVocabulary.Organization.IndustryName]                  = resultItem.Data.industryName;
            metadata.Properties[GlassDoorVocabulary.Organization.IsEEP]                         = resultItem.Data.isEEP.PrintIfAvailable();
            metadata.Properties[GlassDoorVocabulary.Organization.NumberOfRatings]               = resultItem.Data.numberOfRatings.PrintIfAvailable();
            metadata.Properties[GlassDoorVocabulary.Organization.OverallRating]                 = resultItem.Data.overallRating;
            metadata.Properties[GlassDoorVocabulary.Organization.RatingDescription]             = resultItem.Data.ratingDescription;
            metadata.Properties[GlassDoorVocabulary.Organization.RecommendToFriendRating]       = resultItem.Data.recommendToFriendRating.PrintIfAvailable();
            metadata.Properties[GlassDoorVocabulary.Organization.SectorId]                      = resultItem.Data.sectorId.PrintIfAvailable();
            metadata.Properties[GlassDoorVocabulary.Organization.SectorName]                    = resultItem.Data.sectorName;
            metadata.Properties[GlassDoorVocabulary.Organization.SeniorLeadershipRating]        = resultItem.Data.seniorLeadershipRating;
            metadata.Properties[GlassDoorVocabulary.Organization.SquareLogo]                    = resultItem.Data.squareLogo;
            metadata.Properties[GlassDoorVocabulary.Organization.WorkLifeBalanceRating]         = resultItem.Data.workLifeBalanceRating;
            metadata.Properties[GlassDoorVocabulary.Organization.Website]                       = resultItem.Data.website;
            metadata.Properties[GlassDoorVocabulary.Organization.CeoPctApprove]                 = resultItem.Data.ceo.PrintIfAvailable(v => v.pctApprove);
            metadata.Properties[GlassDoorVocabulary.Organization.CeoPctDisapprove]              = resultItem.Data.ceo.PrintIfAvailable(v => v.pctDisapprove);
            metadata.Properties[GlassDoorVocabulary.Organization.CeoNumberOfRatings]            = resultItem.Data.ceo.PrintIfAvailable(v => v.numberOfRatings);

            if (resultItem.Data.featuredReview != null)
            {
                metadata.Properties[GlassDoorVocabulary.Organization.FeaturedReviewAttributionURL]  = resultItem.Data.featuredReview.attributionURL;
                metadata.Properties[GlassDoorVocabulary.Organization.FeaturedReviewCons]            = resultItem.Data.featuredReview.cons;
                metadata.Properties[GlassDoorVocabulary.Organization.FeaturedReviewCurrentJob]      = resultItem.Data.featuredReview.currentJob.PrintIfAvailable();
                metadata.Properties[GlassDoorVocabulary.Organization.FeaturedReviewHeadline]        = resultItem.Data.featuredReview.headline;
                metadata.Properties[GlassDoorVocabulary.Organization.FeaturedReviewId]              = resultItem.Data.featuredReview.id.PrintIfAvailable();
                metadata.Properties[GlassDoorVocabulary.Organization.FeaturedReviewJobTitle]        = resultItem.Data.featuredReview.jobTitle;
                metadata.Properties[GlassDoorVocabulary.Organization.FeaturedReviewJobTitleFromDb]  = resultItem.Data.featuredReview.jobTitleFromDb;
                metadata.Properties[GlassDoorVocabulary.Organization.FeaturedReviewLocation]        = resultItem.Data.featuredReview.location;
                metadata.Properties[GlassDoorVocabulary.Organization.FeaturedReviewOverall]         = resultItem.Data.featuredReview.overall.PrintIfAvailable();
                metadata.Properties[GlassDoorVocabulary.Organization.FeaturedReviewOverallNumeric]  = resultItem.Data.featuredReview.overallNumeric.PrintIfAvailable();
                metadata.Properties[GlassDoorVocabulary.Organization.FeaturedReviewPros]            = resultItem.Data.featuredReview.pros;
                metadata.Properties[GlassDoorVocabulary.Organization.FeaturedReviewReviewDateTime]  = resultItem.Data.featuredReview.reviewDateTime;
            }
        }

        private void PopulatePersonMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<Employer> resultItem)
        {
            var code = this.GetPersonOriginEntityCode(resultItem);

            metadata.EntityType       = EntityType.Person;
            metadata.Name             = resultItem.Data.ceo.name;
            metadata.OriginEntityCode = code;

            metadata.Codes.Add(code);

            metadata.Properties[GlassDoorVocabulary.Person.NumberOfRatings]     = resultItem.Data.ceo.numberOfRatings.PrintIfAvailable();
            metadata.Properties[GlassDoorVocabulary.Person.PctApprove]          = resultItem.Data.ceo.pctApprove.PrintIfAvailable();
            metadata.Properties[GlassDoorVocabulary.Person.PctDisapprove]       = resultItem.Data.ceo.pctDisapprove.PrintIfAvailable();
            metadata.Properties[GlassDoorVocabulary.Person.Title]               = resultItem.Data.ceo.title;
            metadata.Properties[GlassDoorVocabulary.Person.OrganizationName]    = resultItem.Data.name;
            metadata.Properties[GlassDoorVocabulary.Person.ImageSrc]            = resultItem.Data.ceo.image.PrintIfAvailable(v => v.src);
        }

        /**********************************************************************************************************
         * PRIVATE TYPES
         **********************************************************************************************************/

        /// <summary>The glass door api token.</summary>
        private class GlassDoorApiToken
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="GlassDoorApiToken"/> class.
            /// </summary>
            /// <param name="partnerId">The partner identifier.</param>
            /// <param name="key">The key.</param>
            public GlassDoorApiToken(int partnerId, string key)
            {
                this.PartnerId = partnerId;
                this.Key       = key;
            }

            /// <summary>Gets or sets the partner identifier.</summary>
            /// <value>The partner identifier.</value>
            public int PartnerId { get; set; }

            /// <summary>Gets or sets the key.</summary>
            /// <value>The key.</value>
            public string Key { get; set; }
        }
    }
}
