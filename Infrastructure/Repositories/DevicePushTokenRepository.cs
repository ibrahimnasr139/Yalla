namespace Infrastructure.Repositories
{
    internal class DevicePushTokenRepository : IDevicePushTokenRepository
    {
        private readonly AppDbContext _context;

        public DevicePushTokenRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateDevicePushTokenAsync(DevicePushToken devicePushToken, CancellationToken cancellationToken)
        {
            await _context.DevicePushTokens.AddAsync(devicePushToken, cancellationToken);
        }
    }
}