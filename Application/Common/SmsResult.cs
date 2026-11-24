namespace Application.Common
{
    public sealed record SmsResult(bool IsSuccess, string? MessageId, string? Error)
    {
        public static SmsResult Success(string? id = null) => new(true, id, null);
        public static SmsResult Failure(string error) => new(false, null, error);
    }
}