using Microsoft.AspNetCore.Mvc;
using Zeeble.Shared.Helpers;

namespace Zeeble.Web.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;
        public HomeController(WebApiDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult  Index()
        {
            //DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);
            //DateTime endDate = new DateTime(DateTime.Now.Year, 12, 31);
 
            //var newCheckIns = Enumerable.Range(0, (endDate - startDate).Days + 1)
            //    .Select(i => startDate.AddDays(i))
            //     .Select(date => new Zeeble.Shared.Entities.Calender { IsHoliday = false,
                 
            //     LectureDay = date,TenantId =1  })
            //    .ToList();

            //if (newCheckIns.Any())
            //{
            //    await _dbContext.Calendars.AddRangeAsync(newCheckIns);
            //    _dbContext.SaveChanges();
            //}

            return View();
        }
 
    }
}
