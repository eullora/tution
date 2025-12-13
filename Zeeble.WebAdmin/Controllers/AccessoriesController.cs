using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Helpers;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,operator")]
    public class AccessoriesController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;

        public AccessoriesController(WebApiDBContext dbContext, 
            IHostEnvironment hostEnvironment, 
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rows = await _dbContext.Students                
                .Include(i => i.Batch)                
                .Include(i => i.Transactions)
                .Include("Transactions.Product")
                .Where(x => x.TenantId == TenantId)
                .OrderByDescending(x => x.Id).AsNoTracking().ToListAsync();

            ViewData["Rows"] = rows;
            var productRows = await _dbContext.Products.Where(p => p.TenantId ==TenantId).ToListAsync();

            ViewData["Products"] = productRows;                

            return View();
        }
           
    }
}
