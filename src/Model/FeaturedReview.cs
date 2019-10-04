namespace CluedIn.ExternalSearch.Providers.Glassdoor.Model
{
	public class FeaturedReview
	{
		public string attributionURL { get; set; }
		public int id { get; set; }
		public bool currentJob { get; set; }
		public string reviewDateTime { get; set; }
		public string jobTitle { get; set; }
		public string location { get; set; }
		public string jobTitleFromDb { get; set; }
		public string headline { get; set; }
		public string pros { get; set; }
		public string cons { get; set; }
		public int overall { get; set; }
		public int overallNumeric { get; set; }
	}
}