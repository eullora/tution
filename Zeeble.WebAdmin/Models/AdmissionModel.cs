namespace Zeeble.Web.Admin.Models
{
    public class AdmissionModel : EnquiryRequestModel
    {
        public int BatchId { get; set; }
        public string Email { get; set; }
        public string CounsellingRemark { get; set; }
        public double Discount { get; set; }
        public int EnquiryId { get; set; }
        public DateTime FirstInstallmentDate { get; set; }
        public DateTime? SecondInstallmentDate { get; set; }
        public DateTime? ThirdInstallmentDate { get; set; }
        public DateTime? FourthInstallmentDate { get; set; }
        public int FirstInstallmentAmount { get; set; }
        public int? SecondInstallmentAmount { get; set; }
        public int? ThirdInstallmentAmount { get; set; }
        public int? FourthInstallmentAmount { get; set; }
        public string GenderId { get; set; }
        public string Cast { get; set; }

    }
}
