using Account.Api.Extensions;
using Account.Application.Account.Commands.UpdateCard;
using Account.Application.Dtos;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Cards;

public record UpdateCardRequest(UpdateCardDto Card);
public record UpdateCardResponse(bool IsSuccess);

public class UpdateCardEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/accounts/{accountId}/cards/{cardId}",
            async (ISender sender, Guid accountId, Guid cardId, UpdateCardRequest request, HttpContext httpContext) =>
            {
                var userId = httpContext.User.GetCurrentUserId();
                if (userId is null)
                    return Results.Unauthorized();
                var result = await sender.Send(new UpdateCardCommand(accountId, cardId, userId.Value, request.Card));
                var response = result.Adapt<UpdateCardResponse>();
                return Results.Ok(response);
            })
            .WithName("UpdateCard")
            .Produces<UpdateCardResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update card")
            .WithDescription("Update card")
            .RequireAuthorization();
    }
}