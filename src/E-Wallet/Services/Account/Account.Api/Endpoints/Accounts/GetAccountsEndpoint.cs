using Account.Api.Extensions;
using Account.Application.Account.Queries.GetAccounts;
using Account.Application.Dtos;
using BuildingBlocks.Pagination;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Accounts;

public record GetAccountsResponse(PaginatedResult<GetAccountDto> PaginatedResult);
public class GetAccountsEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("accounts", async (ISender sender, HttpContext httpContext, [AsParameters] PaginationRequest request) =>
            {
                var userId = httpContext.User.GetCurrentUserId();
                if (userId is null)
                    return Results.Unauthorized();
                var result = await sender.Send(new GetAccountsQuery(
                    request,
                    userId.Value,
                    httpContext.User.HasClaim("Department", "Account")));
                var response = result.Adapt<GetAccountsResponse>();
                return Results.Ok(response);
            })
            .Produces<GetAccountsResponse>()
            .WithName("GetAccounts")
            .WithDescription("GetAccounts")
            .WithSummary("GetAccounts")
            .RequireAuthorization();
    }
}