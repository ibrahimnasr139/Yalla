namespace Application.Features.Auth.Dtos
{
    public sealed class DeviceDto
    {
        public string DeviceId { get; set; } = default!;
        public string DeviceName { get; set; } = default!;
        public Platform Platform { get; set; }
        public string PushToken { get; set; } = default!;
    }
}