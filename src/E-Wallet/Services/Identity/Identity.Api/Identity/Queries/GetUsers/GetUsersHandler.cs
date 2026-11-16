using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Identity.Api.Data;
using Identity.Api.Dtos;
using Identity.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Identity.Queries.GetUsers;


public record GetUsersQuery(PaginationRequest PaginationRequest): IQuery<GetUsersResult>;

public record GetUsersResult(PaginatedResult<UserDto> Users);

internal sealed class GetUsersQueryHandler(IdentityServiceDbContext dbContext)
    : IQueryHandler<GetUsersQuery, GetUsersResult>
{
    public async Task<GetUsersResult> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;
        var totalCount = await dbContext.Users.LongCountAsync(cancellationToken);

        var users = await dbContext.Users
            .OrderBy(o => o.UserName)
            .AsNoTracking()
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return new GetUsersResult(new PaginatedResult<UserDto>(pageIndex, pageSize, totalCount, users.ToUserDtoList()));
    }
}