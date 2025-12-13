using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{    
    [Table("Payment")]
    public class Payment: BaseEntity
    {
        public int StudentId { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Remarks { get; set; }
        public int InstallmentId { get; set; }
        public Installment Installment { get; set; }
        public int AddedBy { get; set; }

    }

    [Table("PayTerm")]
    public class PayTerm
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
    }

}
