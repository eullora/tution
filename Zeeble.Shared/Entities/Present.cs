using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Present")]
    public  class Present : BaseEntityWithTenant
    {
        public string FileName { get; set; }
        public string FileId { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual ICollection<Attedance> Attedances { get; set; }

    }
}
