using System.Security.Claims;

namespace Account.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetCurrentUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? user.FindFirst("sub")?.Value;

        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
