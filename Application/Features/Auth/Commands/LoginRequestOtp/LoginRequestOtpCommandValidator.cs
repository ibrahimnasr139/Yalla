using Application.Helpers;

namespace Application.Features.Auth.Commands.LoginRequestOtp
{
    public sealed class LoginRequestOtpCommandValidator : AbstractValidator<LoginRequestOtpCommand>
    {
        public LoginRequestOtpCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Must(p => PhoneNumberHelper.NormalizeEgyptMobile(p) is not null)
                .WithMessage("Invalid Egyptian phone number.");

            RuleFor(x => x.AccountType)
                .IsInEnum().WithMessage("Invalid account type.");
        }
    }
}