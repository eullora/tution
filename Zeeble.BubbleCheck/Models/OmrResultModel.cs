
using Windows.Media.AppBroadcasting;

namespace MauiApp1.Models
{
    public class OmrResultModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string RollNumber { get; set; }
        public IEnumerable<string> Sheet { get; set; }
        public IEnumerable<SubjectMarksModel> SubjectMarks { get; set; }
        public string MarksLabel { get; set; }
        public string UploadStatus { get; set; }
        public OmrResultModel()
        {
            
        }
    }

    public class SubjectMarksModel
    {
        public string Label { get; set; }
        public string SubjectName { get; set; }
        public int Marks { get; set; }
        public string Remark { get; set; }
    }

    public class AnswerSheetModel
    {
        public int QuestionNumber { get; set; }
        public string SubjectCode { get; set; }
        public string QuestionLabel { get; set; }
        public string CorrectAnswer { get; set; }
    }

    public class QuestionAnswerKeyPair
    {
        public string QuestionLabel { get; set; }
        public string UserAnswer { get; set; }
    }
}
