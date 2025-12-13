namespace Zeeble.Shared.Models
{
    public class InstallmentPayModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public double DueAmount { get; set; }
        public double? PaidAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool IsPaid { get; set; }
    }
}
