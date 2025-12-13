namespace Zeeble.Web.Admin.Models
{
    public class ResultInsertModel
    {
        public int StudentId { get; set; }
        public string RollNumber { get; set; }
        public string StudentName { get; set; }
        public IEnumerable<int> Marks { get; set; }
    }
    public class ResultRootModel
    {
        public List<ResultInsertModel> ResultList { get; set; }
        public IEnumerable<string> Headers { get; set; }
        public ResultRootModel()
        {
            ResultList = new List<ResultInsertModel>();
        }
    }

}
