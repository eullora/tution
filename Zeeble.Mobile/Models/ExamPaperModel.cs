
namespace Zeeble.Mobile.Models
{
    public class ExamPaperModel
    {
        public string  Id { get; set; }
        public string SubjectName { get; set; }
        public string ImageData { get; set; }        
        public string UserAnswer { get; set; }
    }
    public class ResultModel 
    {
        public int Id { get; set; }
        public int Marks { get; set; }
        public string Data { get; set; }
    }
    public class QuestionPaperModel
    {
        public string Id { get; set; }
        public int SerialNumber { get; set; }
        public string Subject { get; set; }
        public string ImageData { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public int Mark { get; set; }
        public string Remark { get; set; }
    }
}
