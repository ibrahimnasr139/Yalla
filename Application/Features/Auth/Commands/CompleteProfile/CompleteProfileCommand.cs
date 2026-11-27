using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Commands.CompleteProfile
{
    public sealed record CompleteProfileCommand(string FirstName, string LastName, string Email, DateOnly DateOfBirth,
        Gender Gender, string AvatarUrl) : IRequest<OneOf<UserDto, Error>>;
}