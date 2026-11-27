using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities
{
    public sealed class DevicePushToken : IAuditable
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string Token { get; set; } = default!;
        public string DeviceId { get; set; } = default!;
        public string DeviceName { get; set; } = default!;
        public string DeviceModel { get; set; } = default!;
        public Platform Platform { get; set; }
        public Provider Provider { get; set; } = Provider.EXPO;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;
    }
}