using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Api.Services;
using Zeeble.Shared.Helpers;

namespace Zeeble.Api.Controllers
{
    [ApiController]
    [Route("api/sections")]
    public class SectionController : ControllerBase
    {
        private readonly WebApiDBContext _dbContext;
        private readonly ITokenService _tokenService;
        public SectionController(WebApiDBContext dbContext, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        [Route("{tenantId:int}")]        
        public async Task<IActionResult> Get(int tenantId)
        {
            var sections = await _dbContext.TenantSections
            .Include(i => i.Section)
            .Where(w => w.TenantId == tenantId)
            .OrderBy(m => m.Id)
            .ToListAsync();

            return Ok(sections.Select(x => new
            {
                x.Id,
                x.Section.Name,
                x.Section.Code,
                IconText = char.ConvertFromUtf32(int.Parse(x.Section.IconText.ToUpper(), System.Globalization.NumberStyles.HexNumber))
            }));
            
        }
    }
}
