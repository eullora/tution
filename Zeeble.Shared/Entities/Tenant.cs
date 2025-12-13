using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Tenant")]
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Logo { get; set; }
        public string LogoText { get; set; }
        public string GST { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }

    }
}
