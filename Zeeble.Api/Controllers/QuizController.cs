using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Helpers;
using FS = System.IO.File;

namespace Zeeble.Api.Controllers
{
    [Route("api/quiz")]
    public class QuizController : ControllerBase
    {
        private readonly WebApiDBContext _dbContext;
        private readonly IConfiguration _configuration;        
        public QuizController(WebApiDBContext dbContext, IConfiguration configuration) 
        {
            _dbContext = dbContext;            
            _configuration = configuration;             
        }
         
        [HttpGet]
        [Route("{chapterId:int}/files")]
        public async Task<IActionResult> GetFiles(int chapterId)
        {           
            var rows = await _dbContext.Banks.Where(w => w.ChapterId == chapterId).AsNoTracking().ToListAsync();

            var ids = rows.Select(x => x.Id);
            return Ok(ids.OrderBy( x=> Guid.NewGuid()));
        }

        [HttpGet]
        [Route("{fileId:int}")]
        public async Task<IActionResult> Get(int fileId)
        {
            var row =  await _dbContext.Banks.FindAsync(fileId);
            var directory = _configuration.GetSection("QuestionStorage:JsonData").Value;
#if DEBUG
            directory = @"D:\bank\files\";
#endif
            var filePath = $"{directory}/{row.FileId}.json";
#if DEBUG
            filePath = $"{directory}{row.FileId}.json";
#endif
            var data = await FS.ReadAllTextAsync(filePath);                

            return Ok(data);    
        }
    }
}
