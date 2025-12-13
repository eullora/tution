
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Section")]
    public class Section: BaseEntityWithTenant
    {
        public string Name { get; set; }
        public string IconText { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }
}
