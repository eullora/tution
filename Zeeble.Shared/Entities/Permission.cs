using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Permission")]
    public class Permission : BaseEntity
    {
        public string Title { get; set; }

    }
}
