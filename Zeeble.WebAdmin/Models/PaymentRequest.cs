
namespace Zeeble.Web.Admin.Models
{
    public class PaymentRequest
    {        
        public int PayTermId { get; set; }        
        public double Amount { get; set; }        
        public string Remark { get; set; }        
        public int StudentId { get; set; }
    }

}
