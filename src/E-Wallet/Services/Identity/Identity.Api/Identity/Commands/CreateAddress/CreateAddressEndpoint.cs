using Carter;
using Identity.Api.Dtos;
using Identity.Api.Identity.Queries.GetUserId;
using Mapster;
using MediatR;

namespace Identity.Api.Identity.Commands.CreateAddress;

public record CreateAddressRequest(AddressDto Address);

public record CreateAddressResponse(bool IsSuccess);

public class CreateAddressEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("users/address", async (ISender sender, IGetUserId getUserId, CreateAddressRequest request) =>
        {
                var id = await getUserId.GetUserIdFromContext();
                var result = await sender.Send(new CreateAddressCommand(id, request.Address));
                var response = result.Adapt<CreateAddressResponse>();
                return Results.Ok(response);
        })
        .Produces<CreateAddressResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithName("CreateAddress")
        .WithDescription("Creates a new address")
        .WithSummary("Creates a new address")
        .RequireAuthorization();
        
    }
}