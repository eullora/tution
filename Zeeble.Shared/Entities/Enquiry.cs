
using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Enquiry")]
    public class Enquiry : BaseEntity
    {
        public string FullName { get; set; }      
        public string ParentMobile { get; set; }
        public string StudentMobile { get; set; }
        public int StandardId { get; set; }
        public string Address { get; set; }        
        public DateTime CreatedOn { get; set; }
        public int UserId { get; set; }      
        public int? CounsellorId { get; set; }
        public DateTime? CounsellingDate { get; set; }        
        public EnquiryStatus StatusId { get; set; }
        public string SchoolName { get; set; }        
        public int TenantId { get; set; }        
    }
    public enum EnquiryStatus
    {
        NewEnquiry,
        Counselling,
        Admission,
        Discarded
    }
}
