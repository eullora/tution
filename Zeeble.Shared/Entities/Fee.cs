using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Fee")]
    public class Fee:BaseEntityWithTenant
    {
        public string Title { get; set; }
        public double Amount { get; set; }
        public bool IsActive { get; set; }
    }
}
