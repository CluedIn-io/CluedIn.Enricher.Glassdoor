using System.Collections.Generic;

namespace CluedIn.ExternalSearch.Providers.Glassdoor.Model
{
	public class Response
	{
		public string attributionURL { get; set; }
		public int currentPageNumber { get; set; }
		public int totalNumberOfPages { get; set; }
		public int totalRecordCount { get; set; }
		public List<Employer> employers { get; set; }
	}
}