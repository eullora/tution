using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Helpers;
using Zeeble.Web.Admin.Extensions;
using Zeeble.Web.Admin.Models;

namespace Zeeble.Web.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : AdminBaseController
    {
        private readonly WebApiDBContext _dbContext;

        public UsersController(WebApiDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rows = await _dbContext.Users.Include(x => x.Role).Where(m => m.TenantId == TenantId).ToListAsync();
            ViewData["Rows"] = rows;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            await SetViewState();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]        
        public async Task<IActionResult> Add([FromForm] UserRequestModel model)
        {
            var user = await _dbContext.Users.Where(w => w.Email == model.Email.ToLower()).ToListAsync();
            if (user.Any())
            {
                await SetViewState();
                return View();
            }

            await _dbContext.Users.AddAsync(new Shared.Entities.User
            {
                FullName = model.FullName,
                Email = model.Email.ToLower(),
                IsActive = true,
                Password = model.Password.ToHashCode(),
                TenantId = TenantId,
                RoleId = model.RoleId
            });

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            user.IsActive = false;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index");
            
        }

        [HttpGet]
        public async Task<IActionResult> Activate(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            user.IsActive = true;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("index");

        }
        private async Task SetViewState()
        {
            var roleRows = await _dbContext.Roles.ToListAsync();
            ViewData["RoleRows"] = roleRows;
        }
    }
}
