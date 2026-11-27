namespace Application.Features.Auth.Dtos
{
    public sealed class VerifyOtpDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public bool RequiresProfileCompletion { get; set; } = true;
        public UserDto User { get; set; } = default!;
    }
}