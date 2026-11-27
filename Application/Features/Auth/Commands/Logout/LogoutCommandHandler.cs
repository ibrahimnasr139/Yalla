using Application.Contracts.Repositories;

namespace Application.Features.Auth.Commands.Logout
{
    internal sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, OneOf<Unit, Error>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<Unit, Error>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);
            if (refreshToken is null || !refreshToken.IsActive)
                return UserErrors.Unauthorized;

            refreshToken.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.SaveAsync(cancellationToken);
            return Unit.Value;
        }
    }
}