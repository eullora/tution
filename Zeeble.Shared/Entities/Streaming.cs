using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Streaming")]
    public class Streaming : BaseEntityWithTenant
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual ICollection<StreamingBatch> StreamingBatches {  get; set; }         
    }
}
