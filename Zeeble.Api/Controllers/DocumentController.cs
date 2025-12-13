using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Helpers;
using FS = System.IO.File;

namespace Zeeble.Api.Controllers
{
    [Route("api/documents")]
    public class DocumentController : ControllerBase
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IConfiguration _configuration;
        public DocumentController(WebApiDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("{batchId:int}")]
        public async Task<IActionResult> GetDocuments(int batchId)
        {

            var rows = await _dbContext.DocumentBatches
                     .Include(x => x.Document)
                     .Include(x => x.Document.DocumentType)
                     .Where(e => e.BatchId == batchId)
                     .OrderByDescending(x => x.Id)
                     .AsNoTracking().ToListAsync();


            var result = rows.Select(x => new
            {
                x.Id,
                x.Document.Title,
                DocumentType = x.Document.DocumentType.Title,
                x.Document.DocumentType.Code,
                x.Document.CreatedOn,
                x.Document.FileName,
                x.Document.Comments
            });

            return Ok(result);
        }

        [HttpGet]
        [Route("{id:int}/document")]
        public async Task<IActionResult> Get(int id)
        {
            var row = await _dbContext.Documents.FindAsync(id);
            var directory = _configuration.GetSection("DocumentStorage:FilesData").Value;
            var path = $"{directory}/{row.FileName}.pdf";
#if DEBUG
            path = $"{directory}\\{row.FileName}.pdf";
#endif
            var data = await FS.ReadAllBytesAsync(path);

            return File(data, "application/pdf");

        }

        [HttpGet]
        [Route("{id:int}/paper")]
        public async Task<IActionResult> GetPaper(int id)
        {
            var directory = _configuration.GetSection("ExamStorage:ExamFiles").Value;
            var row = await _dbContext.Exams.FindAsync(id);
            var path = $"{directory}/wordfiles/{row.PaperFile}";
            var data = await FS.ReadAllBytesAsync(path);

            return File(data, "application/pdf");

        }
    }
}
