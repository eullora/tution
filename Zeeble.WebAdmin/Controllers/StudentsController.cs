using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Extensions;
using Zeeble.Shared.Helpers;
using Zeeble.Web.Admin.Models;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,counsellor,teacher,cashier,foundation")]
    public class StudentsController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;        
        public StudentsController(WebApiDBContext dbContext, IHostEnvironment hostEnvironment)
        {
            _dbContext = dbContext;            
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rows = await _dbContext.Students
                .Include(i => i.Batch)                
                .Include(i => i.Standard)                
                .Where(w => w.IsActive && w.TenantId == TenantId && w.Batch.IsActive)
                .AsNoTracking().ToListAsync();
            ViewData["Rows"] = rows.OrderByDescending(x => x.CreatedOn);
            ViewData["UserRole"] = RoleName;

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            await SetViewData();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] AdmissionModel model)
        {
            var maxValue = await _dbContext.Students.CountAsync() > 0 ? await _dbContext.Students.MaxAsync(m => m.RollNumber) : 10001;

            var entity = await _dbContext.Students.AddAsync(new Student
            {
                BatchId = model.BatchId,
                FullName = model.FullName,
                CounsellorId = UserId,
                TenantId = TenantId,
                CreatedOn = DateTime.Now,
                Email = model.Email,
                IsActive = true,
                MobileNumber = model.StudentMobile,
                ParentMobile = model.ParentMobile,
                StandardId = model.StandardId,
                RollNumber = maxValue + 1,
                Password = model.ParentMobile.HashPassword(),
                Discount = model.Discount,
                EnquiryId = model.EnquiryId,
                Gender = model.GenderId,
                SchoolName = model.SchoolName,                
                Cast = model.Cast,
                Address = model.Address,
            });

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Perform(int id)
        {
            var row = await _dbContext.Students
                    .Include(x => x.Batch)
                    .Where(m => m.Id == id)
                    .FirstOrDefaultAsync();

            ViewData["Student"] = row;

            var rows = await _dbContext.Results
                .Include(i => i.Exam)
                .Where(x => x.StudentId == id)
                .AsNoTracking().ToListAsync();

            ViewData["Rows"] = rows.OrderByDescending(x => x.CreatedOn);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Reset(int id)
        {
            var row = await _dbContext.Students.FindAsync(id);
            row.DeviceToken = null;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var row = await _dbContext.Students.FindAsync(id);            
            row.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        private async Task SetViewData()
        {
            var standardRows = await _dbContext.Standards.AsNoTracking().ToListAsync();
            var batches = await _dbContext.Batches.Include(x => x.Fee).Where(x => x.TenantId == TenantId).
                AsNoTracking().
                ToListAsync();
            ViewData["BatchRows"] = batches;
            ViewData["StandardRows"] = standardRows;
            ViewData["BatchJson"] = batches.ToJson();

        }
    }
}
