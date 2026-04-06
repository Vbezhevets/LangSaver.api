using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        var id = context.User.FindFirstValue(ClaimTypes.NameIdentifier)  // we don't use ASP.NET Identity? so:
                 ?? context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.Parse(id!);
    }
}
 