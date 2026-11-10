using Account.Api.Extensions;
using Account.Application.Account.Queries.GetTransactionsByAccountId;
using Account.Application.Dtos;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Transactions;


public record GetTransactionsByAccountIdResponse(IEnumerable<TransactionDto> Transactions);

public class GetTransactionsByAccountIdEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("accounts/{id:guid}/transactions", async (ISender sender, Guid id, HttpContext httpContext) =>
            {
                var userId = httpContext.User.GetCurrentUserId();
                if (userId is null)
                    return Results.Unauthorized();
                var result = await sender.Send(new GetTransactionsByAccountIdQuery(
                    id,
                    userId.Value,
                    httpContext.User.HasClaim("Department", "Account")));
                var response = result.Adapt<GetTransactionsByAccountIdResponse>();
                return Results.Ok(response);
            })
            .Produces<GetTransactionsByAccountIdResponse>()
            .WithName("GetTransactionsByAccountId")
            .WithDescription("GetTransactionsByAccountId")
            .WithSummary("GetTransactionsByAccountId")
            .RequireAuthorization();
    }
}