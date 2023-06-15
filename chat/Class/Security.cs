using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace chat.Class
{
    class Security
    {
		public static string ComputeSHA256Hash(string input)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				byte[] inputBytes = Encoding.UTF8.GetBytes(input);
				byte[] hashBytes = sha256.ComputeHash(inputBytes);

				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					builder.Append(hashBytes[i].ToString("x2")); // Convert byte to hex string
				}

				return builder.ToString();
			}
		}
		//---------------------------------------------------------
		public static string Encrypt(string text)
		{
			var b = Encoding.UTF8.GetBytes(text);
			var encrypted = getAes().CreateEncryptor().TransformFinalBlock(b, 0, b.Length);
			return Convert.ToBase64String(encrypted);
		}

		public static string Decrypt(string encrypted)
		{
			var b = Convert.FromBase64String(encrypted);
			var decrypted = getAes().CreateDecryptor().TransformFinalBlock(b, 0, b.Length);
			return Encoding.UTF8.GetString(decrypted);
		}

		static Aes getAes()
		{
			var keyBytes = new byte[16];
			var skeyBytes = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
			Array.Copy(skeyBytes, keyBytes, Math.Min(keyBytes.Length, skeyBytes.Length));

			Aes aes = Aes.Create();
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;
			aes.KeySize = 128;
			aes.Key = keyBytes;
			aes.IV = keyBytes;

			return aes;
		}
	}
}
