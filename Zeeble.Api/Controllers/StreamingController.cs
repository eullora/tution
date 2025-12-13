using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Helpers;

namespace Zeeble.Api.Controllers
{
    [Route("api/streams")]
    public class StreamingController : ControllerBase
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IConfiguration _configuration;
        public StreamingController(WebApiDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("{batchId:int}")]
        public async Task<IActionResult> Get(int batchId)
        {
            var rows = await _dbContext.StreamingBatches
                .Include(x => x.Streaming)
                .Where(w => w.BatchId == batchId)
                .OrderByDescending(w => w.Id)
                .AsNoTracking().ToListAsync();

            var result = rows.Select(x => new
            {
                x.Id,
                x.Streaming.Title,                
                x.Streaming.Url,
                x.Streaming.CreatedOn
            });

            return Ok(result);
        }

        [HttpGet]
        [Route("{id:int}/stream")]
        public async Task<IActionResult> GetById(int id)
        {
            var row = await _dbContext.Streamings.FindAsync(id);
            return Ok(row);
        }
    }
}
