using Application.Common.Options;
using Application.Contracts.Authentication;
using Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services.AuthServices
{
    internal sealed class TokenProvider : ITokenProvider
    {
        private readonly AppDbContext _context;
        private readonly JwtOptions _jwtoptions;
        public TokenProvider(AppDbContext context, IOptions<JwtOptions> options)
        {
            _context = context;
            _jwtoptions = options.Value;
        }

        public string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtoptions.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: _jwtoptions.Issuer,
                audience: _jwtoptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtoptions.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public (string token, DateTime expiresOn) GenerateRefreshToken()
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiresOn = DateTime.UtcNow.AddDays(7);
            return (refreshToken, expiresOn);
        }
    }
}