
namespace MauiApp1.Models
{
    public class ExamListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Batches { get; set; }
        public string Subjects { get; set; }
        public string ExamType { get; set; }    
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Marks { get; set; }
        public int QuestionCount { get; set; }        
    }
}
