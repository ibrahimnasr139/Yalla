namespace Application.Contracts.Repositories
{
    public interface IDevicePushTokenRepository
    {
        Task CreateDevicePushTokenAsync(DevicePushToken devicePushToken, CancellationToken cancellationToken);
    }
}