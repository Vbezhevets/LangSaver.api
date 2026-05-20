using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LangSaver.Api;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        var id = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!Guid.TryParse(id, out var userId))
            throw new UnauthorizedAccessException("Invalid user id in token.");

        return userId;
    }
}