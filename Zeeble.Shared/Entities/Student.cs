using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Student")]
    public class Student : BaseEntityWithTenant
    {
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string ParentMobile{ get; set; }
        public string Email { get; set; }        
        public int RollNumber { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; }
        public int StandardId { get; set; }
        public virtual Standard Standard { get; set; }        
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }        
        public int BatchId { get; set; }
        public virtual Batch Batch { get; set; }
        public int? CounsellorId { get; set; }        
        public int? EnquiryId { get; set; }
        public virtual Enquiry Enquiry { get; set; }
        public double? Discount { get; set; }
        public string SchoolName { get; set; }        
        public string Gender { get; set; }
        public string Address { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }        
        public virtual ICollection<Installment> Installments { get; set; }  
        public string Cast { get; set; }

    }
}
