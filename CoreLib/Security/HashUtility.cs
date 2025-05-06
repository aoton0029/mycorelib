using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Security
{
    /// <summary>
    /// ハッシュユーティリティ
    /// </summary>
    public static class HashUtility
    {
        public static string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            var builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static string GenerateSalt(int size = 16)
        {
            var salt = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public static (string Hash, string Salt) HashPassword(string password)
        {
            var salt = GenerateSalt();
            var hash = ComputeSha256Hash(password + salt);
            return (hash, salt);
        }

        public static bool VerifyPassword(string password, string hash, string salt)
        {
            var computedHash = ComputeSha256Hash(password + salt);
            return computedHash == hash;
        }
    }
}
