
namespace Zeeble.Mobile.Models
{
    public class SubjectGroup
    {
        public string SubjectName { get; set; }
        public int Percent { get; set; }
    }
    public class SubjectMarkModel
    {
        public string SubjectName { get; set; }
        public int Marks { get; set; }
    }

    public class ExamGraphDataModel
    {
        public string Title { get; set; }
        public int Marks { get; set; }
        public IEnumerable<SubjectMarkModel> BreakDown { get; set; }
    }

    public class ExamListModel
    {
        public int Month { get; set; }
        public int Day { get; set; }
        public string ExamName { get; set; }
        public int Marks { get; set; }
        public string Remark { get; set; }
        public string SubjectMarks { get; set; }
    }
    public class ExamGroup : List<ExamListModel>
    {
        public string MonthName { get; set; }

        public ExamGroup(string monthName, List<ExamListModel> exams) : base(exams)
        {
            MonthName = monthName;
        }
    }
}
