using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;
using Zeeble.Web.Admin.Models;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,cashier")]
    public class FeeController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;
        public FeeController(WebApiDBContext dbContext, 
            IHostEnvironment hostEnvironment, 
            IConfiguration configuration)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rows = await _dbContext.Fees.Where(x =>   x.TenantId == TenantId).AsNoTracking().ToListAsync();

            ViewData["Rows"] = rows;                
            return View();
        }
        
        [HttpGet]        
        public IActionResult Add()
        {            
            return View();
        }
      

        [HttpPost]        
        public async Task<IActionResult> Add([FromForm] FeeModel model )
        {           
            await _dbContext.Fees.AddAsync(new Fee
            {
                Amount = model.Amount,
                IsActive =  true,
                TenantId = TenantId,
                Title = model.Title
            });

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

        [HttpGet]        
        public async Task<IActionResult> Delete(int id)
        {
            var row = await _dbContext.Fees.FindAsync(id);
            row.IsActive = false;            
            await _dbContext.SaveChangesAsync();            
            return RedirectToAction("index");
        }      
    }
}
