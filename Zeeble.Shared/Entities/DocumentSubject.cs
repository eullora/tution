using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("DocumentSubject")]
    public class DocumentSubject:BaseEntity
    {
        public int DocumentId { get; set; }
        public Document Document { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

    }
}
