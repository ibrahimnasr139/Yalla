namespace Application.Contracts.Repositories
{
    public interface IPassengerRepository
    {
        Task CreatePassengerAsync(Passenger passenger, CancellationToken cancellationToken);
        Task<Passenger?> GetPassengerByUserIdAsync(string userId, CancellationToken cancellationToken);
    }
}