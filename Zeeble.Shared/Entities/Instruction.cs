using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Instruction")]
    public class Instruction : BaseEntity
    {
        public string Description { get; set; }
    }
}
