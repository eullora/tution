using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Chapter")]
    public class Chapter : BaseEntity
    {
        public string Name { get; set; }
        public int SubjectId { get; set; }
        public int StandardId { get; set; }

    }
}
