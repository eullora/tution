namespace Zeeble.Web.Admin.Models
{
    public class DocumentRequestModel
    {
        public string Title { get; set; }
        public int DocumentTypeId { get; set; }
        public int[] BatchIds { get; set; }
        public int[] SubjectIds { get; set; }
        public string Comments { get; set; }
    }
}
