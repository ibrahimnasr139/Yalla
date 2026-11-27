using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Common
{
    internal class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string? UserId => _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public Role? Role
        {
            get
            {
                var roleClaim = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
                if (Enum.TryParse(roleClaim, true, out Role parsedRole))
                    return parsedRole;
                return null;
            }
        }
    }
}