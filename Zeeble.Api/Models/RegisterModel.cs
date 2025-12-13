namespace Zeeble.Api.Models
{
    public class RegisterModel : BaseModel
    {        
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }                
        public int StandardId { get; set; }
        public string DeviceToken { get; set; }
        public string Source { get; set; }
        
    }

    public class QuizRegisterModel : BaseModel
    {
        public string FullName { get; set; }
        public string MobileNumber { get; set; }        
        public string Password { get; set; }
        public int StandardId { get; set; }
        public string DeviceToken { get; set; }     
        public int TenantId { get; set; }

    }
}
