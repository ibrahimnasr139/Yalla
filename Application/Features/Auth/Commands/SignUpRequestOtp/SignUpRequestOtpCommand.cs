using Application.Features.Auth.Dtos;
namespace Application.Features.Auth.Commands.SignUpRequestOtp
{
    public sealed record SignUpRequestOtpCommand(string PhoneNumber, Role AccountType) : IRequest<OneOf<RequestOtpDto, Error>>;
}