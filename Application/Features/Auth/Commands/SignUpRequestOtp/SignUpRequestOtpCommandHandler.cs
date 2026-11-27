using Application.Contracts.Abstractions;
using Application.Features.Auth.Dtos;
using Application.Helpers;

namespace Application.Features.Auth.Commands.SignUpRequestOtp
{
    internal sealed class SignUpRequestOtpCommandHandler : IRequestHandler<SignUpRequestOtpCommand, OneOf<RequestOtpDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly ISmsService _smsService;
        private readonly IOtpRateLimitService _otpRateLimitService;
        public SignUpRequestOtpCommandHandler(HybridCache hybridCache, ISmsService smsService, IOtpRateLimitService otpRateLimitService)
        {
            _hybridCache = hybridCache;
            _smsService = smsService;
            _otpRateLimitService = otpRateLimitService;
        }
        public async Task<OneOf<RequestOtpDto, Error>> Handle(SignUpRequestOtpCommand request, CancellationToken cancellationToken)
        {
            var phone = PhoneNumberHelper.NormalizeEgyptMobile(request.PhoneNumber);
            if (phone is null)
                return OtpErrors.InvalidPhoneNumber;

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