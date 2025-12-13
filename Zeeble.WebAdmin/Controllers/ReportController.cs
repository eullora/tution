using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin,foundation")]
    public class ReportController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
