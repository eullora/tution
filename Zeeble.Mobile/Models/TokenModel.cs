
namespace Zeeble.Mobile.Models
{
    public class SearchResponseModel 
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string StudentPhone { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public int StandardId { get; set; }
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public string StandardName { get; set; }        
        public int TenantId { get; set; }
        public string TenantName { get; set; }
    }

    public class TokenModel : SearchResponseModel
    {
        public string Token { get; set; }
        public double Interval { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
