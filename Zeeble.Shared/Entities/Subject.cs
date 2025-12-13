using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Subject")]
    public  class Subject : BaseEntityWithTenant
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Code { get; set; }
        
    }
}
