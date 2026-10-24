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
                async (ISender sender, Guid accountId, Guid cardId, TransferRequest request) =>
                {
                    var result = await 
                        sender.Send(new TransferMoneyCommand(accountId, request.ReceiverId, cardId, request.Amount));
                    var response = result.Adapt<TransferResponse>();
                    return Results.Ok(response);
                })
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces<TransferRequest>()
            .WithName("TransferTransaction")
            .WithDescription("Transfer transaction")
            .WithSummary("Transfer transaction");
    }
}