using Carter;
using Identity.Api.Dtos;
using Identity.Api.Identity.Queries.GetUserId;
using Mapster;
using MediatR;

namespace Identity.Api.Identity.Commands.UpdateAddress;

public record UpdateAddressRequest(AddressDto Address);
public record UpdateAddressResponse(bool IsSuccess);

public class UpdateAddressEndpoint
    : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/users/address", async (UpdateAddressRequest request, IGetUserId getUserId, ISender sender) =>
        {
            var id = await getUserId.GetUserIdFromContext();
            var result = await sender.Send(new UpdateAddressCommand(id, request.Address));
            var response = result.Adapt<UpdateAddressResponse>();
            return Results.Ok(response);
        })
            .WithName("UpdateAddress")
            .WithDescription("Updates an address by given ID.")
            .WithSummary("Updates an address by given ID.")
            .Produces<UpdateAddressResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}