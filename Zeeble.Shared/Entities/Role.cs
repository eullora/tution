using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Role")]
    public  class Role : BaseEntity
    {
        public string Title { get; set; }        
        public string Code { get; set; }

    }
}
