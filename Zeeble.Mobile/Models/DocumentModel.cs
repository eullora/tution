
namespace Zeeble.Mobile.Models
{
    public class DocumentModel
    {
        public int Id { get; set; } 
        public string FileName { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public string DocumentType { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
