using System.Security.Claims;
using Account.Application.Account.Queries.GetMyAccounts;
using Account.Application.Dtos;
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
                ClaimsPrincipal user) =>
            {
                var userIdValue =
                    user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? user.FindFirst("sub")?.Value;

                if (!Guid.TryParse(userIdValue, out var userId))
                {
                    return Results.Unauthorized();
                }

                var result = await sender.Send(
                    new GetMyAccountsQuery(userId));

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