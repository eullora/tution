using System.ComponentModel.DataAnnotations.Schema;

namespace Zeeble.Shared.Entities
{
    [Table("Exam")]
    public class Exam : BaseEntityWithTenant
    {
        public string Name { get; set; }                
        public int QuestionCount { get; set; }
        public int Marks {  get; set; }
        public string Description { get; set; }
        public int Time {  get; set; }
        public int ExamTypeId { get; set; }        
        public ExamType ExamType { get; set; }  
        public string PaperFile { get; set; }                        
        public DateTime StartDate { get; set; }    
        public DateTime EndDate { get; set; }    
        public DateTime CreatedOn { get; set; }    
        public bool IsActive { get; set; }                
        public virtual ICollection<ExamBatch> ExamBatches { get; set; }         
        public int ExamModeId { get; set; }        
        public string ResultExcel { get; set; }
        public string SubjectData { get; set; }
    }

    [Table("ExamType")]
    public class ExamType : BaseEntity
    {
        public string Name { get; set; }
        public int InstructionId { get; set; }
        public string Code { get; set; }
        public string CodeColor { get; set; }
        public Instruction Instruction { get; set; }
    }
}
