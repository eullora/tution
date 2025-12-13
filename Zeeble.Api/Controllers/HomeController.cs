using Microsoft.AspNetCore.Mvc;

namespace Zeeble.Api.Controllers
{
	[ApiController]
	[Route("api/home")]
	public class HomeController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			return Ok("Quiz Api V1.0");
		}
	}
}
