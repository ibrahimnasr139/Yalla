namespace Application.Features.Auth.Dtos
{
    public sealed class AuthDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}