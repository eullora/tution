using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("FlashCard")]
    public  class FlashCard : BaseEntityWithTenant
    {
        public string Title { get; set; }
        public int StandardId { get; set; }
        public int SubjectId { get; set; }
        public string FileId { get; set; }

    }
}
