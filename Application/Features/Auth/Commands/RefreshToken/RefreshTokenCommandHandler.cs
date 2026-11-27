using Application.Contracts.Authentication;
using Application.Contracts.Repositories;
using Application.Features.Auth.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.RefreshToken
{
    internal sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, OneOf<AuthDto, Error>>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public RefreshTokenCommandHandler(ITokenProvider tokenProvider, ICurrentUser currentUser, IUnitOfWork unitOfWork,
            IRefreshTokenRepository refreshTokenRepository, UserManager<ApplicationUser> userManager)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenProvider = tokenProvider;
            _userManager = userManager;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<AuthDto, Error>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);
            if (refreshToken is null || !refreshToken.IsActive)
                return UserErrors.Unauthorized;

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            if (user is null)
                return UserErrors.UserNotFound;

            refreshToken.RevokedAt = DateTime.UtcNow;
            var accessToken = _tokenProvider.GenerateJwtToken(user, request.AccountType);
            (string newRefreshToken, DateTime expiresAt) = _tokenProvider.GenerateRefreshToken();
            await _refreshTokenRepository.CreateRefreshTokenAsync(new Domain.Entities.RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiresAt = expiresAt
            }, cancellationToken);

            await _unitOfWork.SaveAsync(cancellationToken);
            return new AuthDto { AccessToken = accessToken, RefreshToken = newRefreshToken };
        }
    }
}