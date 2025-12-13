using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Batch")]
    public  class Batch : BaseEntityWithTenant
    {
        public string Title { get; set; }
        public int StandardId { get; set; }
        public Standard Standard { get; set; }
        public bool IsActive { get; set; }        
        public int FeeId { get; set; }
        public Fee Fee { get; set; }
    }
}
