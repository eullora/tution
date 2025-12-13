
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("ExamBatch")]
    public class ExamBatch : BaseEntity
    {
        public int ExamId { get; set; }
        public virtual Exam Exam { get; set; }
        public int BatchId { get; set; }
        public virtual Batch Batch { get; set; }
    }
}
