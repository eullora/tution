
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("StreamingBatch")]
    public class StreamingBatch : BaseEntity
    {
        public int StreamingId { get; set; }
        public virtual Streaming Streaming { get; set; }
        public int BatchId { get; set; }
        public virtual Batch Batch { get; set; }
    }
}
