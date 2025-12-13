using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Bank")]
    public class Bank : BaseEntity
    {
        public string Title { get; set; }
        public string FileId { get; set; }
        public string SourceFileId { get; set; }
        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }
        public int SubjectId { get; set; }
        public int StandardId { get; set; }
        public Standard Standard { get; set; }
        public Subject Subject { get; set; }
        public int TenantId { get; set; }        
    }
}
