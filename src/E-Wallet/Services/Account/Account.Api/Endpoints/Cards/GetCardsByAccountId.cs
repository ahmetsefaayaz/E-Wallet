using Account.Api.Extensions;
using Account.Application.Account.Queries.GetCardsByAccountId;
using Account.Application.Dtos;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Cards;

public record GetCardsByAccountIdResponse(IEnumerable<GetCardDto> Cards);

public class GetCardsByAccountId: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("accounts/{id:guid}/cards", async (ISender sender, Guid id, HttpContext httpContext) =>
            {
                var userId = httpContext.User.GetCurrentUserId();
                if (userId is null)
                    return Results.Unauthorized();
                var result = await sender.Send(new GetCardsByAccountIdQuery(
                    id,
                    userId.Value,
                    httpContext.User.HasClaim("Department", "Account")));
                var response = result.Adapt<GetCardsByAccountIdResponse>();
                return Results.Ok(response);
            })
            .Produces<GetCardsByAccountIdResponse>()
            .WithName("GetCardsByAccountIdResponse")
            .WithDescription("GetCardsByAccountIdResponse")
            .WithSummary("GetCardsByAccountIdResponse")
            .RequireAuthorization();
    }
}