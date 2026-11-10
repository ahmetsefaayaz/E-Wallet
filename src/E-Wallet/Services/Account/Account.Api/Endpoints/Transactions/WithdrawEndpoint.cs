using Account.Api.Extensions;
using Account.Application.Account.Commands.WithdrawTransaction;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Transactions;

public record WithdrawRequest(decimal Amount);

public record WithdrawResponse(bool IsSuccess);

public class WithdrawEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("accounts/{accountId}/cards/{cardId}/withdraw",
            async (ISender sender, Guid accountId, Guid cardId, WithdrawRequest request, HttpContext httpContext) =>
            {
                var userId = httpContext.User.GetCurrentUserId();
                if (userId is null)
                    return Results.Unauthorized();
                var result = await sender.Send(new WithdrawTransactionCommand(accountId, cardId, userId.Value, request.Amount));
                var response = result.Adapt<WithdrawResponse>();
                return Results.Ok(response);
            })
            .Produces<WithdrawResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithName("Withdraw")
            .WithSummary("withdraw")
            .WithDescription("withdraw")
            .RequireAuthorization();
    }
}