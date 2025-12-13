
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("DocumentBatch")]
    public class DocumentBatch:BaseEntity
    {
        public int DocumentId { get; set; }
        public virtual Document Document { get; set; }
        public int BatchId { get; set; }
        public virtual Batch Batch { get; set; }    
    }
}
