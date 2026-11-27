using Application.Helpers;

namespace Application.Features.Auth.Commands.LoginVerifyOtp
{
    internal class LoginVerifyOtpCommandValidator : AbstractValidator<LoginVerifyOtpCommand>
    {
        public LoginVerifyOtpCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
              .NotEmpty()
              .WithMessage("Phone number is required.")
              .Must(p => PhoneNumberHelper.NormalizeEgyptMobile(p) is not null)
              .WithMessage("Invalid Egyptian phone number.");

            RuleFor(x => x.AccountType)
                .IsInEnum().WithMessage("Invalid account type.");

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("OTP code is required.")
                .Length(6).WithMessage("OTP code must be 6 digits.")
                .Matches(@"^\d{6}$").WithMessage("OTP code must be numeric.");
        }
    }
}