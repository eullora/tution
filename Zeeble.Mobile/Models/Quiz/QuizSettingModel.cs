
namespace Zeeble.Mobile.Models.Quiz
{
    public class QuizSettingModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Token { get; set; }
        public int UserId {  get; set; }        
        public int StandardId { get; set; }
        public string StandardName { get; set; }
        public string SubjectName { get; set; }
        public int TenantId { get; set; }
        public double Interval { get; set; }

        public QuizSettingModel() 
        {
            Interval = 15.0;
        }   
    }
}
