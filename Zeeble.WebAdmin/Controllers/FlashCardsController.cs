using Microsoft.AspNetCore.Mvc;
using Zeeble.Shared.Helpers;

namespace Zeeble.Web.Admin.Controllers
{
    public class FlashCardsController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;
        public FlashCardsController(WebApiDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult  Index()
        {
            return View();
        } 
    }
}
