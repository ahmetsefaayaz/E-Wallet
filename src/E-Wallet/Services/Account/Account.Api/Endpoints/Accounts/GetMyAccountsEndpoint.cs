using Account.Application.Account.Queries.GetMyAccounts;
using Account.Application.Dtos;
using Account.Api.Extensions;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Accounts;

public record GetMyAccountsResponse(IEnumerable<GetAccountDto> Accounts);

public sealed class GetMyAccountsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/accounts/me", async (
                ISender sender,
                HttpContext httpContext) =>
            {
                var userId = httpContext.User.GetCurrentUserId();
                if (userId is null)
                {
                    return Results.Unauthorized();
                }

                var result = await sender.Send(
                    new GetMyAccountsQuery(userId.Value));

                var response = result.Adapt<GetMyAccountsResponse>();

                return Results.Ok(response);
            })
            .Produces<GetMyAccountsResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithName("GetMyAccounts")
            .WithDescription("Gets the authenticated user's accounts")
            .WithSummary("Gets my accounts")
            .RequireAuthorization();
    }
}