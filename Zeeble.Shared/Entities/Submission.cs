using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Submission")]
    public class Submission : BaseEntity
    {
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public string DataFile { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
