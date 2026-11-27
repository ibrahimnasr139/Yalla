namespace Application.Contracts.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task CreateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken);
    }
}