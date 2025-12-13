using System.Security.Cryptography;
using System.Text;

namespace Zeeble.Shared.Extensions
{
	public static class EncryptionExtension
	{
		public static string HashPassword(this string input)
		{
			using (var sha256Hash = SHA256.Create())
			{
				var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
				var builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}

				return builder.ToString();
			}
		}
	}
}
