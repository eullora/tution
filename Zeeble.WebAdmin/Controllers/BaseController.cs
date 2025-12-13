using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Zeeble.Web.Admin.Controllers
{    
    public class AdminBaseController : Controller
    {

        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectResult("/login");
                return;
            }

            SetViewData();
            base.OnActionExecuting(context);
        }

        public string UserFullName
        {
            get
            {
                return User.Claims.Where(w => w.Type == ClaimTypes.Name).FirstOrDefault().Value;
            }
        }

        public int UserId
        {
            get
            {
                return Convert.ToInt32(User.Claims.Where(w => w.Type == "UserId").FirstOrDefault().Value);
            }
        }

        public int TenantId
        {
            get
            {
                return Convert.ToInt32(User.Claims.Where(w => w.Type == "TenantId").FirstOrDefault().Value);
            }
        }
        public string RoleName
        {
            get
            {
                return User.Claims.Where(w => w.Type == ClaimTypes.Role).FirstOrDefault().Value;
            }
        }
        public string LogoText
        {
            get
            {
                return User.Claims.Where(w => w.Type == "LogoText").FirstOrDefault().Value;
            }
        }
        public string LogoImage
        {
            get
            {
                return User.Claims.Where(w => w.Type == "LogoImage").FirstOrDefault().Value;
            }
        }

        public string Address
        {
            get
            {
                return User.Claims.Where(w => w.Type == "Address").FirstOrDefault().Value;
            }
        }

        public string Phone
        {
            get
            {
                return User.Claims.Where(w => w.Type == "Phone").FirstOrDefault().Value;
            }
        }
        public string GSTNumber
        {
            get
            {
                return User.Claims.Where(w => w.Type == "GSTNumber").FirstOrDefault().Value;
            }
        }

        private void SetViewData()
        {
            ViewData["UserRole"] = RoleName;
            ViewData["LogoText"] = LogoText;
            ViewData["LogoImage"] = LogoImage;           
            ViewData["Address"] = Address;           
            ViewData["Phone"] = Phone;         
            ViewData["GSTNumber"] = GSTNumber;                     
            ViewData["UserName"] = UserFullName;                     

        }
    }
}
