using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,counsellor")]
    public class CounsellingController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;        
        public CounsellingController(WebApiDBContext dbContext)
        {
            _dbContext = dbContext;            
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rows = await _dbContext.Enquiries                
                .Where(x => x.StatusId == EnquiryStatus.NewEnquiry && x.TenantId == TenantId)
                .AsNoTracking().ToListAsync();

            ViewData["Rows"] = rows.OrderByDescending(x => x.CreatedOn);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var row = await _dbContext.Enquiries.FindAsync(id);
            _dbContext.Enquiries.Remove(row);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }
    }
}
