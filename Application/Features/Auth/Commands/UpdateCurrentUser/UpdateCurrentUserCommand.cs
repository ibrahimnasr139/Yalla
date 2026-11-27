using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Commands.UpdateCurrentUser
{
    public sealed record UpdateCurrentUserCommand(string? FirstName, string? LastName, string? Email, DateOnly? DateOfBirth,
        Gender? Gender, string? AvatarUrl) : IRequest<OneOf<UserDto, Error>>;
}