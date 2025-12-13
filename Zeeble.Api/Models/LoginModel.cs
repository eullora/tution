namespace Zeeble.Api.Models
{
    public class LoginModel
    {        
        public string LoginID { get; set; }
        public string Pin { get; set; }           
        public string DeviceToken { get; set; }        
    }

    public class QuizLoginModel
    {
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; }        
    }

    public class KSATLoginModel
    {
        public string ParentPhone { get; set; }
        public string Standard{ get; set; }
        public string CourseName { get; set; }
        public string DeviceToken { get; set; }
        public int UserId { get; set; }
        public int BatchId { get; set; }

    }
}
