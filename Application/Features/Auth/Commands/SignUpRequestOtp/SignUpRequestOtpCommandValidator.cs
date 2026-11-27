using Application.Helpers;

namespace Application.Features.Auth.Commands.SignUpRequestOtp
{
    public sealed class SignUpRequestOtpCommandValidator : AbstractValidator<SignUpRequestOtpCommand>
    {
        public SignUpRequestOtpCommandValidator()
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