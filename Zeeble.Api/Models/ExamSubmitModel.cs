namespace Zeeble.Api.Models
{
    public class ExamSubmitModel
    {
        public int ExamId { get; set; }
        public int UserId { get; set; }
        public IEnumerable<AnswerSheetModel> ExamPaper { get; set; }
    }

    public class AnswerSheetModel
    {
        public string Id { get; set; }
        public string SubjectName { get; set; }        
        public string UserAnswer { get; set; }
    }

    public class ExamPaper
    {
        public string Id { get; set; }
        public string SubjectName { get; set; }
        public string ImageData { get; set; }
        public string UserAnswer { get; set; }
    }

    public class ExamPaperModel
    {
        public string Id { get; set; }
        public string SubjectName { get; set; }
        public string ImageData { get; set; }
        public string CorrectAnswer { get; set; }

    }

    public class ExamResultModel
    {
        public string Id { get; set; }
        public int SerialNumber { get; set; }
        public string Subject { get; set; }
        public string ImageData { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer{ get; set; }
        public int Mark { get; set; }
        public string Remark { get; set; }
    }
    public class SubjectGroup
    {
        public string SubjectName { get; set; }
        public int Marks { get; set; }
    }
}
