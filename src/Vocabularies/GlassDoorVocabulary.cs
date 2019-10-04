// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlassDoorVocabulary.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the GlassDoorVocabulary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CluedIn.ExternalSearch.Providers.Glassdoor.Vocabularies
{
    /// <summary>The glass door vocabulary.</summary>
    public static class GlassDoorVocabulary
    {
        /// <summary>
        /// Initializes static members of the <see cref="GlassDoorVocabulary" /> class.
        /// </summary>
        static GlassDoorVocabulary()
        {
            Person       = new GlassDoorPersonVocabulary();
            Organization = new GlassDoorOrganizationVocabulary();
        }

        /// <summary>Gets the organization.</summary>
        /// <value>The organization.</value>
        public static GlassDoorOrganizationVocabulary Organization { get; private set; }

        /// <summary>Gets the person.</summary>
        /// <value>The person.</value>
        public static GlassDoorPersonVocabulary Person { get; private set; }
    }
}