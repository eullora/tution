using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Zeeble.Web.Admin.Models;
using Zeeble.Web.Admin.Extensions;
using Zeeble.Shared.Helpers;

namespace Zeeble.Web.Admin.Controllers
{    
    public class LoginController : Controller
    {
        private readonly WebApiDBContext _dbContext;
        public LoginController(WebApiDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            HttpContext.Session.Remove("AdminUser");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromForm] LoginModel model)
        {
            var hash = model.Password.ToHashCode();

            var user = await _dbContext.Users
                .Include(i => i.Tenant)
                .Include(i => i.Role)
                .Where(x => x.Email == model.UserName.ToLower()
                && x.Password == model.Password.ToHashCode()).FirstOrDefaultAsync();

            if (user == null)
            {
                ViewData["Message"] = "Please enter valid email and password";
                return View(model);
            }

            if (!user.IsActive)
            {
                ViewData["Message"] = "Your account is disabled";
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role.Code),
                new Claim("UserId", user.Id.ToString()),
                new Claim("TenantId", user.TenantId.ToString()),
                new Claim("TenantName", user.Tenant.Name),
                new Claim("LogoText", user.Tenant.LogoText),
                new Claim("LogoImage", user.Tenant.Logo),
                new Claim("Address", user.Tenant.Address),
                new Claim("Phone", user.Tenant.Phone ),
                new Claim("GSTNumber", user.Tenant.GST)
            };

            var claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            return RedirectToAction("index", "home");
        }
    }
}
