using Domain.Entities;

namespace Application.Contracts.Authentication
{
    public interface ITokenProvider
    {
        string GenerateJwtToken(ApplicationUser user);
        (string token, DateTime expiresOn) GenerateRefreshToken();
    }
}