using Account.Api.Extensions;
using Account.Application.Account.Commands.TransferMoney;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Transactions;

public record TransferRequest(Guid ReceiverId, decimal Amount);

public record TransferResponse(bool IsSuccess);

public class TransferEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("accounts/{accountId}/cards/{cardId}/transfer",
                async (ISender sender, Guid accountId, Guid cardId, TransferRequest request, HttpContext httpContext) =>
                {
                    var userId = httpContext.User.GetCurrentUserId();
                    if (userId is null)
                        return Results.Unauthorized();
                    if (!httpContext.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey)
                        || string.IsNullOrWhiteSpace(idempotencyKey))
                        return Results.BadRequest("The Idempotency-Key header is required.");
                    var result = await 
                        sender.Send(new TransferMoneyCommand
                            (accountId, request.ReceiverId, cardId, userId.Value, request.Amount, idempotencyKey.ToString()));
                    var response = result.Adapt<TransferResponse>();
                    return Results.Ok(response);
                })
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithName("TransferTransaction")
            .WithDescription("Transfer transaction")
            .WithSummary("Transfer transaction")
            .RequireAuthorization();
    }
}