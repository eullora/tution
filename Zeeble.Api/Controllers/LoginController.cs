using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zeeble.Api.Models;
using Zeeble.Api.Services;
using Zeeble.Shared.Extensions;
using Zeeble.Shared.Helpers;

namespace Zeeble.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        private readonly WebApiDBContext _dbContext;
        private readonly ITokenService _tokenService;
        public LoginController(WebApiDBContext dbContext, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        [Route("{phoneNumber}/search")]
        public async Task<IActionResult> Get(string phoneNumber)
        {
            var user = await _dbContext.Students
            .Include(i => i.Standard)
            .Include(x => x.Batch)
            .Where(w => w.ParentMobile == phoneNumber)
            .FirstOrDefaultAsync();

            if (user == null)
            {
                return Ok(new { Code = 401, Message = "Invalid parent mobile number. Please check your parent mobile number." });
            }

            if (!user.IsActive)
            {
                return Ok(new { Code = 401, Message = "Your account is disabled." });
            }

            var token = _tokenService.GenerateToken(user.Id.ToString(), user.FullName);

            return Ok(new
            {
                Code = string.IsNullOrEmpty(user.Password) ? 101 : 102,
                Message = "Ok",
                user.BatchId,
                BatchName = user.Batch.Title,
                user.TenantId,
                StandardName = user.Standard.Name,
                UserId = user.Id,
                user.FullName,
                user.Email,
                StudentPhone = user.MobileNumber,
            });
        }

        [Route("authorize")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginModel model)
        {
            var user = await _dbContext.Students
                .Include(i => i.Standard)
                .Include(i => i.Batch)
                .Where(w => w.ParentMobile == model.LoginID)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(user.DeviceToken))
            {
                if (user.DeviceToken != model.DeviceToken)
                {
                    return Ok(new { Code = 402, Message = "Your have already logged into another device." });
                }
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                user.Password = model.Pin.HashPassword();
            }

            user.DeviceToken = model.DeviceToken;
            await _dbContext.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user.Id.ToString(), user.FullName);

            return Ok(new
            {
                Code = 0,
                Message = "Ok",
                user.BatchId,
                user.StandardId,
                user.TenantId,
                Token = token,
                StandardName = user.Standard.Name,
                UserId = user.Id,
                user.FullName,
                user.Email,
                user.MobileNumber,
                Interval = 20.0,
                BatchName = user.Batch.Title
            });
        }

    }
}
