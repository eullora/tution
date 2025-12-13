
using Zeeble.Shared.Entities;

namespace Zeeble.Web.Admin.Models
{
    public class ExamModel
    {
        public string Name { get; set; }
        public int[] BatchIds { get; set; }
        public int QuestionCount { get; set; }
        public int Marks { get; set; }
        public int Time { get; set; }
        public int ExamTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int[] SubjectIds { get; set; }
        public int ExamModeId { get; set; }
        
    }
}
