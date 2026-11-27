using Application.Contracts.Abstractions;
using StackExchange.Redis;

namespace Infrastructure.Services
{
    internal sealed class OtpRateLimitService : IOtpRateLimitService
    {
        private const int CooldownSeconds = 30;
        private const int PhoneLimit = 5;
        private const int PhoneWindowSeconds = 24 * 3600;
        private const int GlobalLimit = 200;
        private const int GlobalWindowSeconds = 24 * 3600;
        private const string Script = """
            if redis.call('EXISTS', KEYS[1]) == 1 then return 1 end
            if tonumber(redis.call('GET', KEYS[2]) or '0') >= tonumber(ARGV[2]) then return 2 end
            if tonumber(redis.call('GET', KEYS[3]) or '0') >= tonumber(ARGV[4]) then return 3 end

            redis.call('SET', KEYS[1], '1', 'EX', ARGV[1])
            if redis.call('INCR', KEYS[2]) == 1 then redis.call('EXPIRE', KEYS[2], ARGV[3]) end
            if redis.call('INCR', KEYS[3]) == 1 then redis.call('EXPIRE', KEYS[3], ARGV[5]) end
            return 0
        """;
        private readonly IDatabase _database;
        public OtpRateLimitService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task<OtpRateLimitResult> TryConsumeAsync(string normalizedPhone)
        {
            var result = await _database.ScriptEvaluateAsync(
                Script,
                new RedisKey[] { $"otp:cooldown:{normalizedPhone}", $"otp:daily:{normalizedPhone}", "otp:global-daily" },
                new RedisValue[] { CooldownSeconds, PhoneLimit, PhoneWindowSeconds, GlobalLimit, GlobalWindowSeconds }
            );
            return (OtpRateLimitResult)(long)result;
        }
    }
}