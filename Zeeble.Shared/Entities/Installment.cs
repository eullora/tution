
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Installment")]
    public class Installment : BaseEntity
    {
        public int StudentId { get; set; }        
        public int PayTermId { get; set; }
        public PayTerm PayTerm { get; set; }
        public DateTime DueDate { get; set; }
        public double Amount { get; set; }
    }
}
