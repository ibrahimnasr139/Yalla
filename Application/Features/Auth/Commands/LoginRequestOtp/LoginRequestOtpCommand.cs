using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Commands.LoginRequestOtp
{
    public sealed record LoginRequestOtpCommand(string PhoneNumber, Role AccountType) : IRequest<OneOf<RequestOtpDto, Error>>;
}