namespace Application.Contracts.Abstractions
{
    public interface IOtpRateLimitService
    {
        Task<OtpRateLimitResult> TryConsumeAsync(string phoneNumber);
    }
    public enum OtpRateLimitResult
    {
        Allowed,
        CooldownActive,
        DailyLimitExceeded,
        GlobalLimitExceeded
    }
}