using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Helpers;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,operator")]
    public class ProductController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;

        public ProductController(WebApiDBContext dbContext, 
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
            var rows = await _dbContext.Products
                        .Include(x => x.Stocks)
                        .Where(x => x.TenantId == TenantId)
                        .AsNoTracking().ToListAsync();

            ViewData["Rows"] = rows;                
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var feeRows = await _dbContext.Fees.Where(w => w.TenantId == TenantId && w.IsActive).AsNoTracking().ToListAsync();
            ViewData["FeesRows"] =feeRows;
            return View();
        }
    }
}
