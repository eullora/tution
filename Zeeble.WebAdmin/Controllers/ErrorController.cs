using Microsoft.AspNetCore.Mvc;

namespace Zeeble.Web.Admin.Controllers
{
    public class ErrorController : Controller
    {        
        public ErrorController()
        {
        }

        public IActionResult  NoAccess()
        {
            return View();
        }
        public IActionResult NoPage()
        {
            return View();
        }

    }
}
