using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Standard")]
    public class Standard : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
