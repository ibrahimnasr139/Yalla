using Domain.Abstractions;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public sealed class ApplicationUser : IdentityUser, IAuditable
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicture { get; set; }
        public Gender? Gender { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; }
        public bool HasOnboarded { get; set; }
        public DateOnly? BirthDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
        public Passenger? Passenger { get; set; }
        public ICollection<DevicePushToken> DevicePushTokens { get; set; } = [];
    }
}