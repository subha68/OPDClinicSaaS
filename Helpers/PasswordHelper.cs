using Microsoft.AspNetCore.Identity;

namespace Helpers
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<object> _hasher = new();

        public static string HashPassword(string password)
        {
            return _hasher.HashPassword(null, password);
        }

        public static bool VerifyPassword(string hashedPassword, string enteredPassword)
        {
            var result = _hasher.VerifyHashedPassword(
                null,
                hashedPassword,
                enteredPassword);

            return result == PasswordVerificationResult.Success;
        }
    }
}