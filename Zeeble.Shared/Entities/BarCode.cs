using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("BarCode")]
    public class BarCode:BaseEntity
    {
        public string BarCodeValue { get; set; }
        public DateTime CreatedOn { get; set; }
        public int StudentId { get; set; }
    }
}
