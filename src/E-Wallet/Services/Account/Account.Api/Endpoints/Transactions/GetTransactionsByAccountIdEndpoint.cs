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
        app.MapGet("accounts/{id:guid}/transactions", async (ISender sender, Guid id) =>
            {
                var result = await sender.Send(new GetTransactionsByAccountIdQuery(id));
                var response = result.Adapt<GetTransactionsByAccountIdResponse>();
                return Results.Ok(response);
            })
            .Produces<GetTransactionsByAccountIdResponse>()
            .WithName("GetTransactionsByAccountId")
            .WithDescription("GetTransactionsByAccountId")
            .WithSummary("GetTransactionsByAccountId");
    }
}