
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("TenantSection")]
    public class TenantSection : BaseEntityWithTenant
    {
        public int SectionId { get; set; }
        public Section Section { get; set; }

        
    }
}
