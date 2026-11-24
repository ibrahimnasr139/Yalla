using Application.Common;
using Application.Contracts.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services
{
    internal sealed class WhySmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly WhySmsOptions _options;
        private readonly ILogger<WhySmsService> _logger;
        public WhySmsService(HttpClient httpClient, IOptions<WhySmsOptions> options, ILogger<WhySmsService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }
        public Task<SmsResult> SendAsync(string recipient, string message, DateTime? scheduleTime = null, CancellationToken cancellationToken = default)
        {
            return SendBulkAsync([recipient], message, scheduleTime, cancellationToken);
        }
        public async Task<SmsResult> SendBulkAsync(IEnumerable<string> recipients, string message, DateTime? scheduleTime = null, CancellationToken cancellationToken = default)
        {
            var numbers = recipients.Select(Normalize).Where(n => n is not null).Distinct().ToList();

            if (numbers.Count == 0)
                return SmsResult.Failure("No valid phone numbers.");

            var request = new SendRequest
            {
                Recipient = string.Join(',', numbers),
                SenderId = _options.SenderId,
                Message = message,
                Type = RequiresUnicode(message) ? "unicode" : "plain",
                ScheduleTime = scheduleTime?.ToString("yyyy-MM-dd HH:mm")
            };

            try
            {
                using var response = await _httpClient.PostAsJsonAsync("sms/send", request, cancellationToken);
                var body = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: cancellationToken);

                if (response.IsSuccessStatusCode &&
                    string.Equals(body?.Status, "success", StringComparison.OrdinalIgnoreCase))
                {
                    return SmsResult.Success(ExtractId(body));
                }

                var error = body?.Message ?? $"HTTP {(int)response.StatusCode}";
                _logger.LogWarning("WhySMS send failed: {Error}", error);
                return SmsResult.Failure(error);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
            {
                _logger.LogError(ex, "WhySMS request threw an exception");
                return SmsResult.Failure("SMS provider unavailable.");
            }
        }

        private string? Normalize(string raw)
        {
            var digits = new string(raw.Where(char.IsDigit).ToArray());

            if (digits.Length == 0)
                return null;
            if (digits.StartsWith("00"))
                digits = digits[2..];
            else if (digits.StartsWith('0'))
                digits = _options.DefaultCountryCode + digits[1..];

            return digits.Length is >= 8 and <= 15 ? digits : null;
        }
        private static string? ExtractId(ApiResponse? body)
        {
            return body?.Data is { ValueKind: JsonValueKind.Object } data &&
                data.TryGetProperty("uid", out var uid) ? uid.GetString() : null;
        }
        private static bool RequiresUnicode(string message) => message.Any(c => c > 127);
        private sealed class SendRequest
        {
            [JsonPropertyName("recipient")]
            public string Recipient { get; init; } = default!;

            [JsonPropertyName("sender_id")]
            public string SenderId { get; init; } = default!;

            [JsonPropertyName("type")]
            public string Type { get; init; } = default!;

            [JsonPropertyName("message")]
            public string Message { get; init; } = default!;

            [JsonPropertyName("schedule_time")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? ScheduleTime { get; init; }
        }
        private sealed class ApiResponse
        {
            [JsonPropertyName("status")]
            public string? Status { get; init; }

            [JsonPropertyName("message")]
            public string? Message { get; init; }

            [JsonPropertyName("data")]
            public JsonElement? Data { get; init; }
        }
    }
}