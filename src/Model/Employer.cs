namespace CluedIn.ExternalSearch.Providers.Glassdoor.Model
{
	public class Employer
	{
		public int id { get; set; }
		public string name { get; set; }
		public string website { get; set; }
		public bool isEEP { get; set; }
		public bool exactMatch { get; set; }
		public string industry { get; set; }
		public int numberOfRatings { get; set; }
		public string squareLogo { get; set; }
		public string overallRating { get; set; }
		public string ratingDescription { get; set; }
		public string cultureAndValuesRating { get; set; }
		public string seniorLeadershipRating { get; set; }
		public string compensationAndBenefitsRating { get; set; }
		public string careerOpportunitiesRating { get; set; }
		public string workLifeBalanceRating { get; set; }
		public int recommendToFriendRating { get; set; }
		public int sectorId { get; set; }
		public string sectorName { get; set; }
		public int industryId { get; set; }
		public string industryName { get; set; }
		public FeaturedReview featuredReview { get; set; }
		public Ceo ceo { get; set; }
	}
}