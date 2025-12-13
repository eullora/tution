using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Stock")]
    public class Stock: BaseEntity
    {
        public int ProductId { get; set; }        
        public int Quantity { get; set; }
        public DateTime CreatedOn { get; set; }
        public int AddedBy { get; set; }
    }
}
