namespace CluedIn.ExternalSearch.Providers.Glassdoor.Model
{
	public class GlassDoorResponse
    {
        public bool success { get; set; }
        public string status { get; set; }
        public string jsessionid { get; set; }
        public Response response { get; set; }
    }
}
