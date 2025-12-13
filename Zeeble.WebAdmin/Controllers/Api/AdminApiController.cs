using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;
using Zeeble.Shared.Models;
using Zeeble.Web.Admin.Models;

namespace Zeeble.Web.Admin.Controllers.Api
{
    [Route("api")]
    public class AdminApiController : ControllerBase
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IConfiguration _configuration;
        public AdminApiController(WebApiDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost]
        [Route("paynow")]
        public async Task<IActionResult> AddPayment(PaymentRequest request)
        {
            var result = await _dbContext.Payments.AddAsync(new Payment
            {
                Amount = request.Amount,
                CreatedOn = DateTime.UtcNow,
                InstallmentId = request.PayTermId,
                Remarks = request.Remark,
                StudentId = request.StudentId,
                AddedBy = 1
            });

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [Route("{studentId:int}/analysis")]
        public async Task<IActionResult> Group(int studentId)
        {
            var rows = await _dbContext
                    .ResultSubjects
                    .Include(m => m.Subject)
                    .Where(x => x.StudentId == studentId)
                    .ToListAsync();

            var resultGroup = rows
                  .GroupBy(d => d.Subject.Name)
                  .Select(group => new SubjectGroup
                  {
                      SubjectName = group.Key,
                      Marks = group.Sum(d => d.Marks)
                  }).ToList();

            var examRows = await _dbContext.Results.Include(i => i.Exam).Where(x => x.StudentId == studentId).Take(20).ToListAsync();
            var attedances = await GetMonthlyAttendanceAsync(studentId);

            var result = new
            {
                resultGroup,
                examResults = examRows.Select(x => new { x.Exam.Name, x.Marks }),
                attedances
            };

            return Ok(result);
        }


        [HttpPost]
        [Route("stock")]
        public async Task<IActionResult> AddStock(StockRequest request)
        {
            var result = await _dbContext.Stocks.AddAsync(new Stock
            {
                AddedBy = 1,
                CreatedOn = DateTime.Now,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("{studentId:int}/accessories")]
        public async Task<IActionResult> GetAccessories(int studentId)
        {
            var student = await _dbContext.Students.FindAsync(studentId);
            var transactions = await _dbContext.Transactions.Include(x => x.Product).Where(x => x.StudentId == studentId).ToListAsync();
            return Ok(new
            {
                student,
                transactions
            });
        }

        [HttpPost]
        [Route("distribute")]
        public async Task<IActionResult> Distribute(DisbtibuteModel model)
        {
            var trans = await _dbContext
                .Transactions
                .Where(x => x.StudentId == model.StudentId)
                .ToListAsync();

            if (trans.Any())
            {
                _dbContext.RemoveRange(trans);
            }

            var rows = model.ProductIds.Select(x => new Transaction
            {
                CreatedOn = DateTime.Now,
                GivenBy = 1,
                ProductId = x,
                Quantity = 1,
                StudentId = model.StudentId,
            });

            await _dbContext.Transactions.AddRangeAsync(rows);

            var stockRows = model.ProductIds.Select(x => new Stock
            {
                CreatedOn = DateTime.Now,
                AddedBy = 1,
                ProductId = x,
                Quantity = -1
            });
            await _dbContext.Stocks.AddRangeAsync(stockRows);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("{subjectId:int}/chapters/{standardId:int}")]
        public async Task<IActionResult> GetChapters(int subjectId, int standardId)
        {
            var chapters = await _dbContext.Chapters.Where(x => x.SubjectId == subjectId && x.StandardId == standardId).ToListAsync();

            return Ok(chapters.Select(m => new { m.Id, m.Name }));
        }


        [HttpGet]
        [Route("{presentId:int}/presents")]
        public async Task<IActionResult> GetAttendance(int presentId)
        {
            var list = await _dbContext.Attedances
                .Include(i => i.Student)
                .Where(x => x.PresentId == presentId)
                .AsNoTracking()
                .ToListAsync();

            return Ok(list);
        }

        private async Task<List<MonthlyAttendance>> GetMonthlyAttendanceAsync(int studentId)
        {
            var attendanceSummary = await _dbContext.Attedances
                .Where(a => a.StudentId == studentId)
                .GroupBy(a => new { a.CheckInTime.Year, a.CheckInTime.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    MonthNumber = g.Key.Month,
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    DaysPresent = g.Select(a => a.CheckInTime.Date).Distinct().Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.MonthNumber)
                .Select(g => new MonthlyAttendance
                {
                    Month = g.MonthName,
                    DaysPresent = g.DaysPresent
                })
                .ToListAsync();

            return attendanceSummary;
        }

    }
}
