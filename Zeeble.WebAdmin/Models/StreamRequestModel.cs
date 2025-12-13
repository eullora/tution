namespace Zeeble.Web.Admin.Models
{
    public class StreamRequestModel
    {
        public string Title { get; set; }
        public string URL { get; set; }
        public int[] BatchIds { get; set; }                
    }
}
