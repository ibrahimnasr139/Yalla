using Application.Contracts.Abstractions;
using Application.Features.Auth.Dtos;
using Application.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.LoginRequestOtp
{
    internal class LoginRequestOtpCommandHandler : IRequestHandler<LoginRequestOtpCommand, OneOf<RequestOtpDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly ISmsService _smsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpRateLimitService _otpRateLimitService;
        public LoginRequestOtpCommandHandler(HybridCache hybridCache, ISmsService smsService, UserManager<ApplicationUser> userManager,
            IOtpRateLimitService otpRateLimitService)
        {
            _hybridCache = hybridCache;
            _smsService = smsService;
            _userManager = userManager;
            _otpRateLimitService = otpRateLimitService;
        }
        public async Task<OneOf<RequestOtpDto, Error>> Handle(LoginRequestOtpCommand request, CancellationToken cancellationToken)
        {
            var phone = PhoneNumberHelper.NormalizeEgyptMobile(request.PhoneNumber);
            if (phone is null)
                return OtpErrors.InvalidPhoneNumber;

            var user = await _userManager.FindByNameAsync(phone);
            if (user is null)
                return UserErrors.UserNotFound;

            if (!await _userManager.IsInRoleAsync(user, request.AccountType.ToString()))
                return UserErrors.UserNotFound;

            if (!user.IsActive)
                return UserErrors.AccountDisabled;

            if (!user.PhoneNumberConfirmed)
                return UserErrors.PhoneNumberNotConfirmed;

            var limit = await _otpRateLimitService.TryConsumeAsync(phone);
            if (limit != OtpRateLimitResult.Allowed)
            {
                return limit switch
                {
                    OtpRateLimitResult.CooldownActive => OtpErrors.CooldownActive,
                    OtpRateLimitResult.DailyLimitExceeded => OtpErrors.DailyLimitExceeded,
                    _ => OtpErrors.ServiceBusy
                };
            }

            var otpCode = await GenerateOtpHelper.GenerateOtp(phone, request.AccountType, _hybridCache, cancellationToken);
            var smsResult = await _smsService.SendAsync(phone, SmsMessagesConstants.SignUpOtp(otpCode), cancellationToken: cancellationToken);

            if (!smsResult.IsSuccess)
                return OtpErrors.SmsProviderFailed;

            return new RequestOtpDto { ExpiresAt = DateTime.UtcNow.AddMinutes(10) };
        }
    }
}