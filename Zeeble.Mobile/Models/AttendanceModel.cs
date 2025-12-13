
namespace Zeeble.Mobile.Models
{

    public class AttendenceRoot
    {
        public List<AttendanceModel> Attendance { get; set; }
        public int DaysInYear { get; set; }
        public int PresentDaysInYear { get; set; }
        public int TotalDaysThisMonth { get; set; }
        public int PresentDaysThisMonth { get; set; }
    }

    public class AttendanceModel
    {
        public DateTime CheckInDate{ get; set; }
        public string CheckInTime { get; set; }
        public int Day { get; set; }
        public string Month { get; set; }
        public string Remark { get; set; }
    }

    public class AttendanceChart
    {
        public string Title { get; set; }
        public int Days { get; set; }
    }

    public class AttendanceGroup : List<AttendanceModel>
    {
        public string MonthName { get; set; }

        public AttendanceGroup(string monthName, List<AttendanceModel> days) : base(days)
        {
            MonthName = monthName;
        }
    }
}
