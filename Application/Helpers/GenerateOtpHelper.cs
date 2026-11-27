using System.Security.Cryptography;

namespace Application.Helpers
{
    public static class GenerateOtpHelper
    {
        public static async Task<string> GenerateOtp(string phoneNumber, Role accountType, HybridCache hybridCache, CancellationToken cancellationToken)
        {
            var otpCode = RandomNumberGenerator.GetInt32(100000, 1_000_000).ToString();
            var cacheKey = $"otp:code:{accountType}:{phoneNumber}";
            await hybridCache.SetAsync(cacheKey, otpCode, new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10)
            }, cancellationToken: cancellationToken);
            return otpCode;
        }
    }
}