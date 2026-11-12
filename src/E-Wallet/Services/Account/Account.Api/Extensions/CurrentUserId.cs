using System.Security.Claims;
using Account.Application.Interfaces;

namespace Account.Api.Extensions;

public class CurrentUserId(IHttpContextAccessor httpContextAccessor): ICurrentUserId
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}