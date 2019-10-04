// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlassDoorPersonVocabulary.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the GlassDoorPersonVocabulary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.Glassdoor.Vocabularies
{
    /// <summary>The glass door person vocabulary.</summary>
    /// <seealso cref="CluedIn.Core.Data.Vocabularies.SimpleVocabulary" />
    public class GlassDoorPersonVocabulary : SimpleVocabulary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlassDoorPersonVocabulary"/> class.
        /// </summary>
        public GlassDoorPersonVocabulary()
        {
            this.VocabularyName = "GlassDoor Person";
            this.KeyPrefix      = "glassDoor.Person";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Person;

            this.NumberOfRatings    = this.Add(new VocabularyKey("numberOfRatings",         VocabularyKeyDataType.Number));
            this.PctApprove         = this.Add(new VocabularyKey("pctApprove",              VocabularyKeyDataType.Number));
            this.PctDisapprove      = this.Add(new VocabularyKey("pctDisapprove",           VocabularyKeyDataType.Number));
            this.Title              = this.Add(new VocabularyKey("title"));
            this.OrganizationName   = this.Add(new VocabularyKey("organizationName"));
            this.ImageSrc           = this.Add(new VocabularyKey("imageSrc",                VocabularyKeyDataType.Uri,          VocabularyKeyVisibility.Hidden));

            this.AddMapping(this.Title, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInUser.JobTitle);
            this.AddMapping(this.OrganizationName, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInUser.Organization);
        }

        public VocabularyKey NumberOfRatings { get; set; }

        public VocabularyKey PctApprove { get; set; }

        public VocabularyKey PctDisapprove { get; set; }

        public VocabularyKey Title { get; set; }

        public VocabularyKey OrganizationName { get; set; }

        public VocabularyKey ImageSrc { get; set; }
    }
}
