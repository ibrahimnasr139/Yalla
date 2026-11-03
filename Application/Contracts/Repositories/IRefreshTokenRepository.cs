namespace Application.Contracts.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(ApplicationUser user, string token, DateTime expiresAt, CancellationToken cancellationToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken);
    }
}