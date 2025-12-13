
namespace Zeeble.Mobile.Models
{
    public class FeeModel
    {
        public string FullName { get; set; }
        public string BatchName { get; set; }
        public double? Discount { get; set; }
        public double TotalFee { get; set; }        
        public double PaidAmount { get; set; }
        public double Balance { get; set; }
        public List<InstallmentPay> Payments { get; set; }
    }
  
    public class InstallmentPay
    {
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public int DueAmount { get; set; }
        public double? PaidAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool IsPaid { get; set; }
    }
}
