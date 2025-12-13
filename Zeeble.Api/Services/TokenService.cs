using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Zeeble.Api.Config;

namespace Zeeble.Api.Services
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string userName);
    }
    public class TokenService : ITokenService
    {
        private readonly ITokenConfig _tokenConfig;
        public TokenService(ITokenConfig tokenConfig)
        {
            _tokenConfig = tokenConfig;
        }

        public string GenerateToken(string userId, string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenConfig.SymmetricKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.NameId, userName),
                new Claim("UserId", userId),                
                new Claim(JwtRegisteredClaimNames.Aud, _tokenConfig.Issuer),
                new Claim(JwtRegisteredClaimNames.Iss, _tokenConfig.Issuer),

            ]),
                Expires = DateTime.UtcNow.AddDays(180),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }

}
