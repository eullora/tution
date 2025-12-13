using System.Security.Cryptography;
using System.Text;

namespace Zeeble.Web.Admin.Extensions
{
    public static class AppExtensions
    {
        public static string ToHashCode(this string code)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(code.ToUpper()));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToUpper();
            }
        }
    }
}
