// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearBitOrganizationVocabulary.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the ClearBitOrganizationVocabulary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.RegularExpressions;

using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.Glassdoor.Vocabularies
{
    /// <summary>The clear bit organization vocabulary.</summary>
    /// <seealso cref="CluedIn.Core.Data.Vocabularies.SimpleVocabulary" />
    public class GlassDoorOrganizationVocabulary : SimpleVocabulary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlassDoorOrganizationVocabulary"/> class.
        /// </summary>
        public GlassDoorOrganizationVocabulary()
        {
            this.VocabularyName = "GlassDoor Organization";
            this.KeyPrefix      = "glassDoor.organization";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Organization;

            this.AddGroup("Metadata", group =>
            {
                this.SquareLogo                     = group.Add(new VocabularyKey("squareLogo",                             VocabularyKeyDataType.Uri,              VocabularyKeyVisibility.Hidden));
                this.SectorName                     = group.Add(new VocabularyKey("sectorName"));
                this.SectorId                       = group.Add(new VocabularyKey("sectorId",                               VocabularyKeyDataType.Number,           VocabularyKeyVisibility.Hidden));
                this.IsEEP                          = group.Add(new VocabularyKey("isEEP",                                  VocabularyKeyDataType.Boolean,          VocabularyKeyVisibility.Hidden));
                this.IndustryName                   = group.Add(new VocabularyKey("industryName"));
                this.IndustryId                     = group.Add(new VocabularyKey("industryId",                             VocabularyKeyDataType.Number,           VocabularyKeyVisibility.Hidden));
                this.Industry                       = group.Add(new VocabularyKey("industry"));
                this.Website                        = group.Add(new VocabularyKey("website",                                VocabularyKeyDataType.Uri));
            });

            this.AddGroup("Ratings", group =>
            {
                this.OverallRating                  = group.Add(new VocabularyKey("ratings.overallRating",                  VocabularyKeyDataType.Number)).WithDisplayName("Overall Rating");
                this.RatingDescription              = group.Add(new VocabularyKey("ratings.ratingDescription")                                           ).WithDisplayName("Rating Description");

                this.RecommendToFriendRating        = group.Add(new VocabularyKey("ratings.recommendToFriendRating",        VocabularyKeyDataType.Number)).WithDisplayName("Recommend To Friend Rating");
                this.CultureAndValuesRating         = group.Add(new VocabularyKey("ratings.cultureAndValuesRating",         VocabularyKeyDataType.Number)).WithDisplayName("Culture and Values Rating");
                this.CompensationAndBenefitsRating  = group.Add(new VocabularyKey("ratings.compensationAndBenefitsRating",  VocabularyKeyDataType.Number)).WithDisplayName("Compensation and Benefits Rating");
                this.CareerOpportunitiesRating      = group.Add(new VocabularyKey("ratings.careerOpportunitiesRating",      VocabularyKeyDataType.Number)).WithDisplayName("Career Opportunities Rating");
                this.WorkLifeBalanceRating          = group.Add(new VocabularyKey("ratings.workLifeBalanceRating",          VocabularyKeyDataType.Number)).WithDisplayName("Worklife Balance Rating");
                this.SeniorLeadershipRating         = group.Add(new VocabularyKey("ratings.seniorLeadershipRating",         VocabularyKeyDataType.Number)).WithDisplayName("Senior Leadership Rating");

                this.CeoPctApprove                  = group.Add(new VocabularyKey("ratings.ceo.pctApprove",                 VocabularyKeyDataType.Number)).WithDisplayName("CEO Approval %");
                this.CeoPctDisapprove               = group.Add(new VocabularyKey("ratings.ceo.pctDisapprove",              VocabularyKeyDataType.Number)).WithDisplayName("CEO Disapproval %");
                this.CeoNumberOfRatings             = group.Add(new VocabularyKey("ratings.ceo.numberOfRatings",            VocabularyKeyDataType.Number)).WithDisplayName("Number of CEO Ratings");

                this.NumberOfRatings                = group.Add(new VocabularyKey("ratings.numberOfRatings",                VocabularyKeyDataType.Number)).WithDisplayName("Number of Ratings");
            });

            this.AddGroup("Featured Review", group =>
            {
                this.FeaturedReviewId              = group.Add(new VocabularyKey("featuredReview.id",                       VocabularyKeyDataType.Number,                   VocabularyKeyVisibility.Hidden));
                this.FeaturedReviewHeadline        = group.Add(new VocabularyKey("featuredReview.headline"));
                this.FeaturedReviewReviewDateTime  = group.Add(new VocabularyKey("featuredReview.reviewDateTime",           VocabularyKeyDataType.DateTime));
                this.FeaturedReviewOverall         = group.Add(new VocabularyKey("featuredReview.overall",                  VocabularyKeyDataType.Number));
                this.FeaturedReviewOverallNumeric  = group.Add(new VocabularyKey("featuredReview.overallNumeric",           VocabularyKeyDataType.Number,                   VocabularyKeyVisibility.Hidden));
                this.FeaturedReviewLocation        = group.Add(new VocabularyKey("featuredReview.location",                 VocabularyKeyDataType.GeographyLocation));
                this.FeaturedReviewJobTitle        = group.Add(new VocabularyKey("featuredReview.jobTitle"));
                this.FeaturedReviewJobTitleFromDb  = group.Add(new VocabularyKey("featuredReview.jobTitleFromDb",                                                           VocabularyKeyVisibility.Hidden));
                this.FeaturedReviewCurrentJob      = group.Add(new VocabularyKey("featuredReview.currentJob",               VocabularyKeyDataType.Boolean));
                this.FeaturedReviewPros            = group.Add(new VocabularyKey("featuredReview.pros"));
                this.FeaturedReviewCons            = group.Add(new VocabularyKey("featuredReview.cons"));
                this.FeaturedReviewAttributionURL  = group.Add(new VocabularyKey("featuredReview.attributionURL",           VocabularyKeyDataType.Uri));
            });

            this.AddMapping(this.IndustryName, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Industry);
            this.AddMapping(this.Website, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Website);
        }

        public VocabularyKey WorkLifeBalanceRating { get; set; }
        public VocabularyKey SquareLogo { get; set; }
        public VocabularyKey SeniorLeadershipRating { get; set; }
        public VocabularyKey SectorName { get; set; }
        public VocabularyKey SectorId { get; set; }
        public VocabularyKey RecommendToFriendRating { get; set; }
        public VocabularyKey RatingDescription { get; set; }
        public VocabularyKey OverallRating { get; set; }
        public VocabularyKey NumberOfRatings { get; set; }
        public VocabularyKey IsEEP { get; set; }
        public VocabularyKey IndustryName { get; set; }
        public VocabularyKey IndustryId { get; set; }
        public VocabularyKey Industry { get; set; }
        public VocabularyKey FeaturedReviewReviewDateTime { get; set; }
        public VocabularyKey FeaturedReviewPros { get; set; }
        public VocabularyKey FeaturedReviewOverallNumeric { get; set; }
        public VocabularyKey FeaturedReviewOverall { get; set; }
        public VocabularyKey FeaturedReviewLocation { get; set; }
        public VocabularyKey FeaturedReviewJobTitleFromDb { get; set; }
        public VocabularyKey FeaturedReviewJobTitle { get; set; }
        public VocabularyKey FeaturedReviewId { get; set; }
        public VocabularyKey FeaturedReviewHeadline { get; set; }
        public VocabularyKey FeaturedReviewCurrentJob { get; set; }
        public VocabularyKey FeaturedReviewCons { get; set; }
        public VocabularyKey FeaturedReviewAttributionURL { get; set; }
        public VocabularyKey CultureAndValuesRating { get; set; }
        public VocabularyKey CompensationAndBenefitsRating { get; set; }
        public VocabularyKey CareerOpportunitiesRating { get; set; }
        public VocabularyKey CeoPctApprove { get; set; }
        public VocabularyKey CeoPctDisapprove { get; set; }
        public VocabularyKey CeoNumberOfRatings { get; set; }
        public VocabularyKey Website { get; set; }
    }
}
