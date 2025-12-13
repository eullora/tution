using Microsoft.AspNetCore.Mvc;

namespace Zeeble.Api.Controllers
{
    [ApiController]
    [Route("api/authorize")]
    public class AuthorizeController : ControllerBase
    {
        public AuthorizeController() 
        {
        }
        
        [HttpGet]        
        public IActionResult Get()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok(new { userName = User.Identity.Name});
            }

            else
            {
                return BadRequest();
            }
        }

    }
}
