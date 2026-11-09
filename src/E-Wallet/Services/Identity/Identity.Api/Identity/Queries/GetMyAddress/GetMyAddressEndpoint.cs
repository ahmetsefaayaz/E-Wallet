using System.Security.Claims;
using Carter;
using Identity.Api.Dtos;
using Mapster;
using MediatR;

namespace Identity.Api.Identity.Queries.GetMyAddress;

public record GetMyAddressResponse(AddressDto Address);

public class GetMyAddressEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/me/address", async (ISender sender, ClaimsPrincipal user) =>
            {
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                  ?? user.FindFirst("sub")?.Value;

                if (!Guid.TryParse(userIdValue, out var userId))
                {
                    return Results.Unauthorized();
                }

                var result = await sender.Send(new GetMyAddressQuery(userId));
                var response = result.Adapt<GetMyAddressResponse>();
            
                return Results.Ok(response);
            })
            .Produces<GetMyAddressResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName("GetMyAddress")
            .WithDescription("Gets the authenticated user's address")
            .WithSummary("Gets my address")
            .RequireAuthorization(); 
    }
}