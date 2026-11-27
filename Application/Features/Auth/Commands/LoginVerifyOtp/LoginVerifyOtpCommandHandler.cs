using Application.Contracts.Authentication;
using Application.Contracts.Repositories;
using Application.Features.Auth.Dtos;
using Application.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.LoginVerifyOtp
{
    internal sealed class LoginVerifyOtpCommandHandler : IRequestHandler<LoginVerifyOtpCommand, OneOf<VerifyOtpDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly ITokenProvider _tokenProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPassengerRepository _passengerRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LoginVerifyOtpCommandHandler(HybridCache hybridCache, ITokenProvider tokenProvider, IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager, IPassengerRepository passengerRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _hybridCache = hybridCache;
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _passengerRepository = passengerRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<OneOf<VerifyOtpDto, Error>> Handle(LoginVerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var phone = PhoneNumberHelper.NormalizeEgyptMobile(request.PhoneNumber);
            if (phone is null)
                return OtpErrors.InvalidPhoneNumber;

            var cacheKey = $"otp:code:{request.AccountType}:{phone}";
            var cachedOtp = await _hybridCache.GetOrCreateAsync(
                cacheKey,
                async _ => await Task.FromResult<string?>(null),
                cancellationToken: cancellationToken
            );

            if (request.Code != cachedOtp)
                return OtpErrors.InvalidOtpCode;

            var user = await _userManager.FindByNameAsync(phone);
            if (user is null)
                return UserErrors.UserNotFound;

            if (!await _userManager.IsInRoleAsync(user, request.AccountType.ToString()))
                return UserErrors.UserNotFound;

            if (!user.IsActive)
                return UserErrors.AccountDisabled;

            if (!user.PhoneNumberConfirmed)
                return UserErrors.PhoneNumberNotConfirmed;

            Passenger? passenger = null;
            //Captain? captain = null;
            if (request.AccountType is Role.PASSENGER)
            {
                passenger = await _passengerRepository.GetPassengerByUserIdAsync(user.Id, cancellationToken);
                if (passenger is null)
                    return UserErrors.UserNotFound;

            }
            else if (request.AccountType is Role.CAPTAIN)
            {
                //captain = await _captainRepository.GetCaptainByUserIdAsync(user.Id, cancellationToken);
                //if (captain is null)
                //    return UserErrors.UserNotFound;
            }

            var accessToken = _tokenProvider.GenerateJwtToken(user, request.AccountType);
            (string refreshToken, DateTime expiresAt) = _tokenProvider.GenerateRefreshToken();

            var newRefreshToken = new Domain.Entities.RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = expiresAt
            };

            await _refreshTokenRepository.CreateRefreshTokenAsync(newRefreshToken, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _hybridCache.RemoveAsync(cacheKey, cancellationToken);

            return new VerifyOtpDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    //Id = passenger?.Id ?? captain?.Id ?? default,
                    Id = passenger?.Id ?? default,
                    PhoneNumber = phone,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    AvatarUrl = user.ProfilePicture,
                    DateOfBirth = user.BirthDate,
                    Gender = user.Gender,
                    Role = user.Role
                }
            };
        }
    }
}