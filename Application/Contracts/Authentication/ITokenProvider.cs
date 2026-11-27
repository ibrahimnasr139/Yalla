namespace Application.Contracts.Authentication
{
    public interface ITokenProvider
    {
        string GenerateJwtToken(ApplicationUser user, Role role);
        (string token, DateTime expiresOn) GenerateRefreshToken();
    }
}