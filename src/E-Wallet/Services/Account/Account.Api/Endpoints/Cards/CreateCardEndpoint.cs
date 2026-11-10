using Account.Api.Extensions;
using Account.Application.Account.Commands.CreateCard;
using Account.Application.Dtos;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Cards;

public record CreateCardRequest(CreateCardDto Card);
public record CreateCardResponse(Guid CardId);
public class CreateCardEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("accounts/{id}/cards", async (ISender sender, Guid id, CreateCardRequest request, HttpContext httpContext) =>
        {
            var userId = httpContext.User.GetCurrentUserId();
            if (userId is null)
                return Results.Unauthorized();
            var result = await sender.Send(new CreateCardCommand(id, userId.Value, request.Card));
            var response = result.Adapt<CreateCardResponse>();
            return Results.Created($"/accounts/{id}/cards", response);
            
        })
        .Produces<CreateCardResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("CreateCard")
        .WithDescription("Create card request")
        .WithSummary("Create card request")
        .RequireAuthorization();
    }
}