using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;
using Zeeble.Web.Admin.Models;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,operator,foundation")]
    public class StreamingController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;

        public StreamingController(WebApiDBContext dbContext, 
            IHostEnvironment hostEnvironment, 
            IConfiguration configuration)
        {
            _dbContext = dbContext;        
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rows = await _dbContext
                .Streamings                 
                .Include(i => i.StreamingBatches)
                .Include("StreamingBatches.Batch")
                .Where(x => x.TenantId == TenantId && x.IsActive)
                .OrderByDescending(x => x.Id).AsNoTracking().ToListAsync();

            ViewData["Rows"] = rows;                
            return View();
        }
        
        [HttpGet]        
        public async Task<IActionResult> Add()
        {            
            var batches = await _dbContext.Batches.Where(x => x.TenantId == TenantId).AsNoTracking().ToListAsync();                
            ViewData["Batches"] = batches;            
            return View();
        }
      

        [HttpPost]        
        public async Task<IActionResult> Add([FromForm] StreamRequestModel model , IFormFile documentFile)
        {             
            await _dbContext.Streamings.AddAsync(new Streaming
            {                
                Title = model.Title,                
                CreatedOn = DateTime.Now,
                Url = model.URL,
                IsActive = true,
                StreamingBatches = model.BatchIds.Select(m => new StreamingBatch
                {
                    BatchId = m
                }).ToList(),
                TenantId = TenantId
            });

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]        
        public async Task<IActionResult> Delete(int id)
        {
            var row = await _dbContext.Streamings.FindAsync(id);
            _dbContext.Streamings.Remove(row);   
            await _dbContext.SaveChangesAsync();            
            return RedirectToAction("index");
        }      
    }
}
