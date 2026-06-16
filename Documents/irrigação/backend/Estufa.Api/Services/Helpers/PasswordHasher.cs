using System;
using System.Security.Cryptography;

namespace Estufa.Api.Services.Helpers
{
    // Simple PBKDF2 password hasher (stores salt + hash in base64)
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
            var result = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);
            return Convert.ToBase64String(result);
        }

        public static bool Verify(string password, string hashed)
        {
            try
            {
                var bytes = Convert.FromBase64String(hashed);
                var salt = new byte[16];
                Buffer.BlockCopy(bytes, 0, salt, 0, salt.Length);
                var hash = new byte[32];
                Buffer.BlockCopy(bytes, salt.Length, hash, 0, hash.Length);
                var attempted = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
                return CryptographicOperations.FixedTimeEquals(attempted, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
