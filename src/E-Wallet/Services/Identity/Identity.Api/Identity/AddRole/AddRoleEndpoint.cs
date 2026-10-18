using Carter;
using Mapster;
using MediatR;

namespace Identity.Api.Identity.AddRole;



public record AddRoleRequest(string RoleName);
public record AddRoleResponse(bool IsSuccess);
public class AddRoleEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/{id:guid}/roles", async (Guid id, ISender sender, AddRoleRequest request) =>
            {
                var result = await sender.Send(new AddRoleCommand(id, request.RoleName));
                var response = result.Adapt<AddRoleResponse>();
                return Results.Ok(response);

            })
            .Produces<AddRoleResponse>(StatusCodes.Status200OK)
            .WithName("AddRole")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("AddRole")
            .WithSummary("AddRole")
            .RequireAuthorization("IsAdmin");
    }
}