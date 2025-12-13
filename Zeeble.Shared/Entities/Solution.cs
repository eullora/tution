using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Solution")]
    public class Solution : BaseEntity
    {
        public int FileId { get; set; }
        public int BankId { get; set; }
        public Bank Bank { get; set; }
        public int FileType { get; set; }
        public string QuestionId { get; set; }
        public int ExamId { get; set; }
        public Exam Exam { get; set; }  
    }
}
