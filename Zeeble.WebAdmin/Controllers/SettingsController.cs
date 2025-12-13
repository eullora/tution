using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Helpers;
using Zeeble.Web.Admin.Models;
using FS = System.IO.File;
namespace Zeeble.Web.Admin.Controllers
{

    [Authorize(Roles = "admin,operator,foundation")]
    public class SettingsController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;        
        private readonly IConfiguration _configuration;

        public SettingsController(WebApiDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;            
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var rows = await _dbContext.TenantSections
                .Include(i => i.Section)
                .Where(w => w.TenantId == TenantId)
                .OrderBy(m => m.Id)
                .ToListAsync();



            var templateRows = await _dbContext.Templates.AsNoTracking().ToListAsync();
            var sections = rows.Select(x => new KeyPairModel
            {
                Id = x.SectionId,
                Title = x.Section.Name
            });

            ViewData["Rows"] = sections;                
            ViewData["TemplateRows"] = templateRows;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Template(int id)
        {
            var row = await _dbContext.Templates.FindAsync(id);
            var directory = _configuration.GetSection("TemplateStorage:Files").Value;
            var extension = Path.GetExtension(row.FileId);
            var path = $"{directory}{row.FileId}";

            var mimeType = extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" or ".docx" => "application/vnd.ms-word",
                ".csv" => "text/csv",
                _ => "application/octet-stream"
            };

            return File(await FS.ReadAllBytesAsync(path), mimeType, row.FileName);

        }

    }
}
