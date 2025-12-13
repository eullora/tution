using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("ResultSubject")]
    public  class ResultSubject : BaseEntity
    {
        public int ResultId { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int Marks { get; set; }

    }
}
