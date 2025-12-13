using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;
using FS = System.IO.File;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,operator,foundation")]
    public class AttendanceController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IConfiguration _configuration;

        public AttendanceController(WebApiDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rows = await _dbContext
                    .Presents
                    .Where(x => x.TenantId == TenantId)
                    .OrderByDescending(i => i.Id)
                    .AsNoTracking()
                    .ToListAsync();

            ViewData["Rows"] = rows;

            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Add(IFormFile attendanceFile)
        {
            var directory = _configuration.GetSection("Attendance:Files").Value;
            var fileId = Guid.NewGuid().ToString();
            var fileName = $"{directory}{fileId}.csv";

            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                await attendanceFile.CopyToAsync(stream);
            }
            ;

            var lines = await FS.ReadAllLinesAsync(fileName);
            var cnt = 0;
            var students = await _dbContext
                    .Students
                    .Where(w => w.TenantId == TenantId)
                    .AsNoTracking()
                    .ToListAsync();

            var presentEntity = await _dbContext.Presents.AddAsync(new Present
            {
                FileName = attendanceFile.FileName,
                FileId = $"{fileId}.csv",
                CreatedOn = DateTime.Now,
                TenantId = TenantId
            });

            await _dbContext.SaveChangesAsync();

            foreach (var line in lines)
            {
                if (cnt == 0) { cnt++; continue; }

                var columns = line.Split(',');
                var student = students.FirstOrDefault(w => w.RollNumber == Convert.ToInt32(columns[0]));
                if (student == null) { continue; }

                var checkIn = Convert.ToDateTime(columns[1]);

                await _dbContext.Attedances.AddAsync(new Attedance
                {
                    StudentId = student.Id,
                    CheckInTime = Convert.ToDateTime(columns[1]),
                    CheckOutTime = !string.IsNullOrEmpty(columns[2]) ? Convert.ToDateTime(columns[2]) : null,
                    PresentId = presentEntity.Entity.Id,
                });

                cnt++;

                if (cnt >= 50)
                {
                    await _dbContext.SaveChangesAsync();
                    cnt = 1;
                }
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var row = await _dbContext.Presents.FindAsync(id);
            if (row != null)
            {
                var attendance = await _dbContext.Attedances.Where(x => x.PresentId == id).ToListAsync();
                _dbContext.Attedances.RemoveRange(attendance);
                _dbContext.Presents.Remove(row);

                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("index");
        }
    }
}
