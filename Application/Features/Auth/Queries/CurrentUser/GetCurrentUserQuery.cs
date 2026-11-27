using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Queries.CurrentUser
{
    public sealed record GetCurrentUserQuery : IRequest<OneOf<UserDto, Error>>;
}