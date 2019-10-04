namespace CluedIn.ExternalSearch.Providers.Glassdoor.Model
{
	public class Ceo
	{
		public string name { get; set; }
		public string title { get; set; }
		public int numberOfRatings { get; set; }
		public int pctApprove { get; set; }
		public int pctDisapprove { get; set; }
		public CeoImage image { get; set; }
	}
}