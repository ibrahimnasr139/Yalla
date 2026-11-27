using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Commands.LoginVerifyOtp
{
    public sealed record LoginVerifyOtpCommand(string PhoneNumber, Role AccountType, string Code) : IRequest<OneOf<VerifyOtpDto, Error>>;
}