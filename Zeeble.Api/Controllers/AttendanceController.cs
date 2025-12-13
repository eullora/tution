using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Api.Models;
using Zeeble.Shared.Helpers;

namespace Zeeble.Api.Controllers
{
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IConfiguration _configuration;
        public AttendanceController(WebApiDBContext dbContext, IConfiguration configuration) 
        {
            _dbContext = dbContext;            
            _configuration = configuration; 
        }

        [HttpGet]
        [Route("{studentId:int}")]
        public async Task<IActionResult> Get(int studentId)
        {

            var calenderRows = await _dbContext.Calendars.Where(x => x.LectureDay.Year == DateTime.Now.Year)
                .OrderByDescending(x => x.Id)
                .AsNoTracking().ToListAsync();

            var attendenceRows = await _dbContext.Attedances.Where(w => w.StudentId== studentId)
                                        .OrderByDescending(w => w.CheckInTime).AsNoTracking().ToListAsync();

            var daysInYear = calenderRows.Count(w => !w.IsHoliday && w.LectureDay.Date <= DateTime.Now.Date);
            var presentDaysInYear = attendenceRows.Count();

            var totalDaysThisMonth = calenderRows.Count( x => !x.IsHoliday &&  x.LectureDay.Date.Month == DateTime.Now.Month && 
            x.LectureDay.Date <= DateTime.Now.Date);

            var presentDaysThisMonth = attendenceRows.Count(x => x.CheckInTime.Date.Month == DateTime.Now.Date.Month);

            var daysMonthRows = calenderRows.Where(x => x.LectureDay.Month > DateTime.Now.Month - 3 &&  x.LectureDay.Date <= DateTime.Now.Date);    

            var all = new List<AttendanceModel>();  

            foreach(var day in daysMonthRows)
            {
                
                var present  = attendenceRows.Where(x => x.CheckInTime.Date == day.LectureDay.Date).FirstOrDefault();
                var attendance = new AttendanceModel();
                attendance.Day = day.LectureDay.Day;
                attendance.Month = $"{GetMonthName(day.LectureDay.Month)}-{day.LectureDay.Year}";
                attendance.CheckInTime = present == null ? "" : present.CheckInTime.ToString("HH:mm"); 

                if (present == null && !day.IsHoliday)
                {
                    attendance.Remark = "Absent";
                }
                else if(present == null && day.IsHoliday)
                {
                    attendance.Remark = "Holiday";
                }
                else
                {
                    attendance.Remark = "Present";
                }
                
                all.Add(attendance);                
            }

            var result = new
            {
                attendance  = all,
                daysInYear,
                presentDaysInYear,
                totalDaysThisMonth,
                presentDaysThisMonth
            };

            return Ok(result);
        }
        
        private string GetMonthName(int month)
        {
            if (month == 1) return "Jan";
            if (month == 2) return "Feb";
            if (month == 3) return "Mar";
            if (month == 4) return "Apr";
            if (month == 5) return "May";
            if (month == 6) return "Jun";
            if (month == 7) return "Jul";
            if (month == 8) return "Aug";
            if (month == 9) return "Sep";
            if (month == 10) return "Oct";
            if (month == 11) return "Nov";

            return "Dec";
        }
    }
}
