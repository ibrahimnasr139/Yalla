namespace Application.Contracts.Abstractions
{
    public interface ISmsService
    {
        Task<SmsResult> SendAsync(string recipient, string message, DateTime? scheduleTime = null, CancellationToken cancellationToken = default);
        Task<SmsResult> SendBulkAsync(IEnumerable<string> recipients, string message, DateTime? scheduleTime = null, CancellationToken cancellationToken = default);
    }
}