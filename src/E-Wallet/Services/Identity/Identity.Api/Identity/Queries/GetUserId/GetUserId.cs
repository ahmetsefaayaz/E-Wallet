using System.Security.Claims;
using Identity.Api.Exceptions;

namespace Identity.Api.Identity.Queries.GetUserId;

internal sealed class GetUserId(IHttpContextAccessor accessor): IGetUserId
{

    public async Task<Guid> GetUserIdFromContext()
    {
        var result = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(result))
            throw new UserNotFoundException();
        return Guid.Parse(result);
    }
}