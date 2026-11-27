namespace Infrastructure.Repositories
{
    internal sealed class PassengerRepository : IPassengerRepository
    {
        private readonly AppDbContext _context;

        public PassengerRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreatePassengerAsync(Passenger passenger, CancellationToken cancellationToken)
        {
            await _context.Passengers.AddAsync(passenger, cancellationToken);
        }
        public async Task<Passenger?> GetPassengerByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.Passengers.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        }
    }
}