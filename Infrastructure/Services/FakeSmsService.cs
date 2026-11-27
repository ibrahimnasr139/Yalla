using Application.Common;
using Application.Contracts.Abstractions;
using Application.Helpers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    internal sealed class FakeSmsService : ISmsService
    {
        private readonly ILogger<FakeSmsService> _logger;
        public FakeSmsService(ILogger<FakeSmsService> logger) => _logger = logger;
        public Task<SmsResult> SendAsync(string recipient, string message, DateTime? scheduleTime = null, CancellationToken cancellationToken = default)
            => SendBulkAsync([recipient], message, scheduleTime, cancellationToken);
        public Task<SmsResult> SendBulkAsync(IEnumerable<string> recipients, string message, DateTime? scheduleTime = null, CancellationToken cancellationToken = default)
        {
            var numbers = recipients
                .Select(PhoneNumberHelper.NormalizeEgyptMobile)
                .OfType<string>()
                .ToList();

            if (numbers.Count == 0)
                return Task.FromResult(SmsResult.Failure("No valid phone numbers."));

            // أي رقم بينتهي بـ 0000 بيحاكي فشل الـ Provider
            if (numbers.Any(n => n.EndsWith("0000")))
                return Task.FromResult(SmsResult.Failure("Simulated provider failure."));

            _logger.LogInformation("[FAKE SMS] To: {Numbers} | Message: {Message}", string.Join(',', numbers), message);
            return Task.FromResult(SmsResult.Success(Guid.NewGuid().ToString("N")));
        }
    }
}