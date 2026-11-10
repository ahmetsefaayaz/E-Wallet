using Account.Api.Extensions;
using Account.Application.Account.Commands.DepositTransaction;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Transactions;

public record DepositRequest(decimal Amount);

public record DepositResponse(bool IsSuccess);

public class DepositEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("accounts/{accountId}/cards/{cardId}/deposit",
            async (ISender sender, Guid accountId, Guid cardId, DepositRequest request, HttpContext httpContext) =>
            {
                var userId = httpContext.User.GetCurrentUserId();
                if (userId is null)
                    return Results.Unauthorized();
                var result = await sender.Send(new DepositTransactionCommand(accountId, cardId, userId.Value, request.Amount));
                var response = result.Adapt<DepositResponse>();
                return Results.Ok(response);
            })
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces<DepositResponse>()
            .WithName("Deposit Transaction")
            .WithDescription("Deposit transaction")
            .WithSummary("Deposit transaction")
            .RequireAuthorization();
    }
    
}