using Application.Contracts.Authentication;
using Application.Contracts.Repositories;
using Application.Features.Auth.Dtos;
using Application.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Application.Features.Auth.Commands.SignUpVerifyOtp
{
    internal sealed class SignUpVerifyOtpCommandHandler : IRequestHandler<SignUpVerifyOtpCommand, OneOf<VerifyOtpDto, Error>>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly HybridCache _hybridCache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPassengerRepository _passengerRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public SignUpVerifyOtpCommandHandler(ITokenProvider tokenProvider, HybridCache hybridCache, IUnitOfWork unitOfWork,
            IRefreshTokenRepository refreshTokenRepository, IPassengerRepository passengerRepository, UserManager<ApplicationUser> userManager)
        {
            _tokenProvider = tokenProvider;
            _hybridCache = hybridCache;
            _unitOfWork = unitOfWork;
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;
            _passengerRepository = passengerRepository;
        }
        public async Task<OneOf<VerifyOtpDto, Error>> Handle(SignUpVerifyOtpCommand request, CancellationToken cancellationToken)
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
            if (user is not null && await _userManager.IsInRoleAsync(user, request.AccountType.ToString()))
                return UserErrors.PhoneAlreadyRegistered;

            var newUser = new ApplicationUser
            {
                UserName = phone,
                PhoneNumber = phone,
                PhoneNumberConfirmed = true,
                Role = request.AccountType,
                IsActive = true
            };

            var newPassenger = new Passenger
            {
                UserId = newUser.Id
            };

            string refreshTokenValue;
            DateTime refreshTokenExpiresAt;
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var createUserResult = await _userManager.CreateAsync(newUser);
                if (!createUserResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return new Error("UserCreationFailed", createUserResult.Errors.First().Description, HttpStatusCode.BadRequest);
                }
                var role = await _userManager.AddToRoleAsync(newUser, request.AccountType.ToString());
                if (!role.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return new Error("RoleAssignmentFailed", role.Errors.First().Description, HttpStatusCode.InternalServerError);
                }
                await _passengerRepository.CreatePassengerAsync(newPassenger, cancellationToken);
                (refreshTokenValue, refreshTokenExpiresAt) = _tokenProvider.GenerateRefreshToken();
                var refreshToken = new Domain.Entities.RefreshToken
                {
                    Token = refreshTokenValue,
                    ExpiresAt = refreshTokenExpiresAt,
                    UserId = newUser.Id
                };
                await _refreshTokenRepository.CreateRefreshTokenAsync(refreshToken, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }

            var accessToken = _tokenProvider.GenerateJwtToken(newUser, request.AccountType);
            await _hybridCache.RemoveAsync(cacheKey, cancellationToken);

            return new VerifyOtpDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                User = new UserDto
                {
                    Id = newPassenger.Id,
                    PhoneNumber = phone,
                    FullName = $"{newUser.FirstName} {newUser.LastName}",
                    Email = newUser.Email,
                    AvatarUrl = newUser.ProfilePicture,
                    DateOfBirth = newUser.BirthDate,
                    Gender = newUser.Gender,
                    Role = newUser.Role
                }
            };
        }
    }
}