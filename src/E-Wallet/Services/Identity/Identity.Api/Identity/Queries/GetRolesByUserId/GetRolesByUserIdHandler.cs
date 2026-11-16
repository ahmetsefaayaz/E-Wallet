using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Identity.Api.Entities;
using Identity.Api.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Identity.Queries.GetRolesByUserId;

public record GetRolesByUserIdQuery(Guid UserId): IQuery<GetRolesByUserIdResult>;

public record GetRolesByUserIdResult(IEnumerable<string> Roles);


internal sealed class GetRolesByUserIdQueryHandler(UserManager<User> userManager): IQueryHandler<GetRolesByUserIdQuery, GetRolesByUserIdResult>
{
    public async Task<GetRolesByUserIdResult> Handle(GetRolesByUserIdQuery query, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString()) 
            ?? throw new UserNotFoundException();
        var roles = await userManager.GetRolesAsync(user)
            ?? throw new RoleNotFoundException();
        return new GetRolesByUserIdResult(roles);
    }
}