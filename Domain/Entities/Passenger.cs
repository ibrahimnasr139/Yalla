using Domain.Abstractions;

namespace Domain.Entities
{
    public sealed class Passenger : IAuditable
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public int TotalRides { get; set; } = 0;
        public int Rating { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;
    }
}