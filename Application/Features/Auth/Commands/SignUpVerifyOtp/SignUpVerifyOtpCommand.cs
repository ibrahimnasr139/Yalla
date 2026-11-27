using Application.Features.Auth.Dtos;

namespace Application.Features.Auth.Commands.SignUpVerifyOtp
{
    public sealed record SignUpVerifyOtpCommand(string PhoneNumber, Role AccountType, string Code) : IRequest<OneOf<VerifyOtpDto, Error>>;
}