namespace Application.Features.Auth.Dtos
{
    public sealed class UserDto
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public Role Role { get; set; }

    }
}