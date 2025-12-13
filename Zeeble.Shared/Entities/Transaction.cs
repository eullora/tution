using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Transaction")]
    public class Transaction : BaseEntity
    {
        public int ProductId { get; set; }      
        public Product Product { get; set; }
        public int StudentId { get; set; }                
        public int Quantity { get; set; }
        public DateTime CreatedOn { get; set; }
        public int GivenBy { get; set; }
    }
}
