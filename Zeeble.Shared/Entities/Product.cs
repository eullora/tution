using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Product")]
    public class Product: BaseEntityWithTenant
    {
        public string Title { get; set; }
        public virtual ICollection<Stock> Stocks { get; set; }
        
    }
}
