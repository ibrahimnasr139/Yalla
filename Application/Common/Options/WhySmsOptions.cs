using System.ComponentModel.DataAnnotations;

namespace Application.Common.Options
{
    public sealed class WhySmsOptions
    {
        [Required, Url]
        public string BaseUrl { get; set; } = default!;

        [Required]
        public string ApiToken { get; set; } = default!;

        [Required, MaxLength(11)]
        public string SenderId { get; set; } = default!;

        public string DefaultCountryCode { get; set; } = default!;
    }
}