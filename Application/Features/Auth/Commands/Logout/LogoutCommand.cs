namespace Application.Features.Auth.Commands.Logout
{
    public sealed record LogoutCommand(string RefreshToken) : IRequest<OneOf<Unit, Error>>;
}