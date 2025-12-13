using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Result")]
    public class Result : BaseEntity
    {
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public string ResultFile { get; set; }
        public int Marks { get; set; }
        public DateTime CreatedOn { get; set; }        
        public virtual ICollection<ResultSubject> ResultSubjects { get; set; }
        public int StatusId { get; set; }
    }
}
