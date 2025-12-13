using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Zeeble.Web.Admin.Controllers
{
    public class LogoutController : Controller
    {
        public LogoutController()
        {

        }

        public async Task<IActionResult> Index()
        {            
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();
            return RedirectToAction("index", "login");
        }
    }
}
