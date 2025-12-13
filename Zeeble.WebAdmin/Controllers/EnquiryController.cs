using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;
using Zeeble.Web.Admin.Models;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,frontdesk")]
    public class EnquiryController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;        
        public EnquiryController(WebApiDBContext dbContext)
        {
            _dbContext = dbContext;            
        }

        public async Task<IActionResult> Index()
        {
            var rows = await _dbContext.Enquiries                                 
                .Where(x => x.StatusId == EnquiryStatus.NewEnquiry && x.TenantId == TenantId)
                .AsNoTracking().ToListAsync();

            ViewData["Rows"] = rows.OrderByDescending(x => x.CreatedOn);
            await SetViewData();
            return View();
        }
        
        public async Task<IActionResult> Add() 
        {
            await SetViewData();
            return View();            
        }
        
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] EnquiryRequestModel model)
        {
            await _dbContext.Enquiries.AddAsync(new Enquiry
            {
                
                Address = model.Address,                
                FullName = model.FullName,
                SchoolName = model.SchoolName,                
                CreatedOn = DateTime.Now,
                StandardId = model.StandardId,
                TenantId = TenantId,
                UserId = UserId,
                StudentMobile = model.StudentMobile,
                ParentMobile = model.ParentMobile,
                StatusId =  EnquiryStatus.NewEnquiry          
                
            });

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

      
        public async Task<IActionResult> Delete(int id)
        {
            var entry = await _dbContext.Enquiries.FindAsync(id);
            _dbContext.Enquiries.Remove(entry);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        private async Task SetViewData()
        {
            var standardRows = await _dbContext.Standards.AsNoTracking().ToListAsync();            

            ViewData["StandardRows"] = standardRows;            
        }
    }
}
