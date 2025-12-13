namespace Zeeble.Web.Admin.Models
{
    public class QuestionModel
    {
        public string Id { get; set; }
        public string SubjectName { get; set; }
        public string ImageData { get; set; }
        public string CorrectAnswer { get; set; }
    }

    public class QuizQuestionModel
    {
        public string Id { get; set; }        
        public string ImageData { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
