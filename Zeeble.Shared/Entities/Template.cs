using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Template")]
    public class Template : BaseEntityWithTenant
    {
        public string Title { get; set; }
        public string FileName { get; set; }
        public string FileId { get; set; }
        
    }
}
