using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Commands.RefreshToken
{
    public sealed record RefreshTokenCommand(string RefreshToken, Role AccountType) : IRequest<OneOf<AuthDto, Error>>;
}