
namespace Zeeble.Mobile.Models
{
    public class ExamModel 
    {
        public int Id { get; set; }
        public string Name { get; set; }                
        public string Mode { get; set; }
        public int Marks { get; set; }        
        public int Time { get; set; }        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }        
        public int ExamTypeId { get; set; }  
        public string ExamType { get; set; }
        public int InstructionId { get; set; }
        public string Code { get; set; }
        public string CodeColor { get; set; }        
        public int StatusId { get; set; }
        public bool IsCompleted { get; set; }
        public string Remark { get; set; }
        public IEnumerable<BreakDownModel> BreakDown { get; set; }
        public int TotalBreakDownMarks => BreakDown?.Sum(x => x.Marks) ?? 0;
        public string MarksDisplay => $"{TotalBreakDownMarks} / {Marks}";
    }

    public class InstructionModel
    {
        public string Description { get; set; }
    }
    public class ExamResult
    {                    
            public int Id { get; set; }
            public string ResultFile { get; set; }
            public int Marks { get; set; }            
            public int StatusId { get; set; }        
    }
}
