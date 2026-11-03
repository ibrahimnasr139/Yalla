using System.ComponentModel.DataAnnotations;

namespace Application.Common.Options
{
    public sealed class JwtOptions
    {
        [Required]
        public string SecretKey { get; set; } = string.Empty;
        [Range(1, 1440, ErrorMessage = "Invalid Expiry Value")]
        public int ExpiryMinutes { get; set; }
        [Required]
        public string Issuer { get; set; } = string.Empty;
        [Required]
        public string Audience { get; set; } = string.Empty;
    }
}