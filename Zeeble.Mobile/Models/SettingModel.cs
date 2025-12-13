
namespace Zeeble.Mobile.Models
{
    public class SettingModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Token { get; set; }
        public int UserId {  get; set; }        
        public int StandardId { get; set; }
        public string StandardName { get; set; }        
        public string CourseName { get; set; }        
        public int TenantId { get; set; }        
        public int BatchId { get; set; }           
        public string BatchName { get; set; }           
    }
}
