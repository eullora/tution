using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("DocumentUser")]
    public class DocumentUser : BaseEntity
    {
        public int DocumentId { get; set; }        
        public Document Document { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
