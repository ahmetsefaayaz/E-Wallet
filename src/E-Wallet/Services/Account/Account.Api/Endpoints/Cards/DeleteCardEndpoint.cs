using Account.Application.Account.Commands.DeleteCard;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Cards;

public record DeleteCardResult(bool IsSuccess);
public class DeleteCardEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("accounts/{accountId}/cards/{cardId}", async (ISender sender, Guid accountId, Guid cardId) =>
        {
            var result = await sender.Send(new DeleteCardCommand(accountId, cardId));
            var response = result.Adapt<DeleteCardResult>();
            return Results.Ok(response);
        })
        .WithName("DeleteCard")
        .WithDescription("Delete card")
        .WithSummary("Delete card")
        .Produces<DeleteCardResult>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization("IsAdmin");
    }
}