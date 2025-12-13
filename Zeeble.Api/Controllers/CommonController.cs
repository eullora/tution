using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Helpers;

namespace Zeeble.Api.Controllers
{
	[ApiController]
	[Route("api")]
	public class CommonController : ControllerBase
	{
		private readonly WebApiDBContext _dbContext;
        private readonly string _cdnUrl;
		public CommonController(WebApiDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpGet]
		[Route("standards")]
		public async Task<IActionResult> Get()
		{
			var rows = await _dbContext.Standards.Where(x => x.IsActive).AsNoTracking().ToListAsync();
			var result = rows.Select(x => new
			{
				Text = x.Name,
				Value = x.Id
			});

			return Ok(result);
		}

        [HttpGet]
        [Route("{tenantId:int}/subjects")]
        public async Task<IActionResult> Get(int tenantId)
        {
            var rows = await _dbContext.Subjects.Where(x => x.TenantId == tenantId).AsNoTracking().ToListAsync();
            var result = rows.Select(x => new
            {
                x.Id,
                x.Name,                
				x.Icon
            });

            return Ok(result);
        }

        [HttpGet]
        [Route("{subjectId:int}/chapters")]
        public async Task<IActionResult> GetChapers(int subjectId)
        {
            var rows = await _dbContext.Chapters.Where(x => x.SubjectId== subjectId).AsNoTracking().ToListAsync();
            var result = rows.Select(x => new
            {
                x.Id,
                x.Name                
            });

            return Ok(result);
        }
     
        [HttpGet]
        [Route("student/{studentId:int}/subject/{subjectId:int}/flashcards/{standardId:int}")]
        public async Task<IActionResult> GetFlashCards(int studentId, int subjectId, int standardId)
        {
            var student = await _dbContext.Students.FindAsync(studentId);

            var row = await _dbContext
                            .FlashCards.Where(x => x.SubjectId == subjectId && x.StandardId == standardId && x.TenantId == student.TenantId)
                            .FirstOrDefaultAsync();
            var result = new
            {
                row.Id,
                row.Title,
                Url =  $"{_cdnUrl}{row.FileId}"                
            };

            return Ok(result);
        }
    }
}
