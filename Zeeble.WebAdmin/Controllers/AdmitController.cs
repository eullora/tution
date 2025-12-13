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
    
    public class AdmitController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;        
        public AdmitController(WebApiDBContext dbContext)
        {
            _dbContext = dbContext;            
        }

        [Authorize(Roles = "admin,operator,counsellor")]
        public async Task<IActionResult> Index(int enquiryId)
        {
            await SetViewData();
            var row = await _dbContext.Enquiries.FindAsync(enquiryId);                 
            ViewData["EnquiryRow"] = row;

            return View();
        }

        [Authorize(Roles = "admin,operator,counsellor")]
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] AdmissionModel model) 
        {
           var maxValue = await _dbContext.Students.CountAsync() > 0 ?  await _dbContext.Students.MaxAsync(m => m.RollNumber) : 10001;
            var installments = new List<Installment>
            {
                new Installment{Amount = model.FirstInstallmentAmount, DueDate = model.FirstInstallmentDate, PayTermId =  1 }
            };

            if(model.SecondInstallmentDate != null && model.SecondInstallmentAmount != null)
            {
                installments.Add(new Installment { Amount = model.SecondInstallmentAmount.Value, DueDate = model.SecondInstallmentDate.Value, PayTermId = 2 });
            }

            if (model.ThirdInstallmentDate != null && model.ThirdInstallmentAmount != null)
            {
                installments.Add(new Installment { Amount = model.ThirdInstallmentAmount.Value, DueDate = model.ThirdInstallmentDate.Value, PayTermId = 3 });
            }
            
            var entity =  await _dbContext.Students.AddAsync(new Student
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
                Gender =  model.GenderId,
                SchoolName =model.SchoolName,
                Installments = installments,
                Cast = model.Cast,
                Address = model.Address,                
            });

            var enquiry = await _dbContext.Enquiries.FindAsync(model.EnquiryId);
            if(enquiry != null)
            {
                enquiry.StatusId = EnquiryStatus.Admission;
                enquiry.CounsellorId = UserId;                
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("confirm",new {id = entity.Entity.Id});            
        }

        [Authorize(Roles = "admin,counsellor,foundation")]
        [HttpGet]        
        public async Task<IActionResult>Confirm(int id)
        {
            if(id == 0)
            {
                return RedirectToAction("index","students");
            }

            var student = await _dbContext.Students
                    .Include(x => x.Batch)                    
                    .Include(m => m.Installments)
                    .Include("Installments.PayTerm")
                    .Include(b => b.Batch.Fee)                    
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();


            ViewData["StudentRow"] = student;
            return View();
        }

        [Authorize(Roles = "admin,counsellor,foundation")]
        public async Task<IActionResult>PrintForm(int id)
        {
            var row = await
                         _dbContext.Students
                         .Include(x => x.Batch)
                         .Include(x => x.Batch.Fee)                         
                         .Include(x => x.Installments)                         
                         .Include("Installments.PayTerm")                                                  
                         .Where(x => x.Id == id).FirstOrDefaultAsync();

            ViewData["StudentRow"] = row;
            return View();            
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
