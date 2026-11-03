namespace Infrastructure.Repositories
{
    internal sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddRefreshTokenAsync(ApplicationUser user, string token, DateTime expiresAt, CancellationToken cancellationToken)
        {
            var refreshToken = new RefreshToken
            {
                Token = token,
                UserId = user.Id,
                ExpiresAt = expiresAt
            };
            await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        }
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }
    }
}