using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using Zeeble.Shared.Entities;
using Zeeble.Shared.Helpers;
using Zeeble.Web.Admin.Models;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,operator,cashier,foundation")]
    public class BatchController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;
        public BatchController(WebApiDBContext dbContext)
        {
            _dbContext = dbContext;            
        }

        public async Task<IActionResult> Index()
        {
            var rows = await _dbContext.Batches
                .Include(i => i.Fee)
                .Include(i => i.Standard)
                .Where(x => x.IsActive && x.TenantId == TenantId)
                .AsNoTracking()
                .ToListAsync();

            ViewData["Rows"] = rows.OrderByDescending(x => x.Id);

            return View();
        }

        public async Task<IActionResult> Add()
        {
            await SetViewData();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            await SetViewData();
            var row = await _dbContext.Batches.FindAsync(id);
            ViewData["BatchRow"] = row;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] BatchRequestModel model)
        {            
            var row = await _dbContext.Batches.FindAsync(model.BatchId);
            row.Title = model.Title;
            row.StandardId= model.StandardId;
            row.FeeId = model.FeeId;            
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] BatchRequestModel model)
        {
            await _dbContext.Batches.AddAsync(new Batch
            {
                FeeId = model.FeeId,
                StandardId = model.StandardId,
                Title = model.Title,
                IsActive = true,
                TenantId = TenantId
            });

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var batch = await _dbContext.Batches.FindAsync(id);
            batch.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        private async Task SetViewData()
        {
            var standardRows = await _dbContext.Standards.AsNoTracking().ToListAsync();
            var feesRows = await _dbContext.Fees.Where(f => f.IsActive && f.TenantId == TenantId).AsNoTracking().ToListAsync();

            ViewData["StandardRows"] = standardRows;
            ViewData["FeesRows"] = feesRows;
            ViewData["FeesJson"] = feesRows.ToJson();

        }
    }
}
