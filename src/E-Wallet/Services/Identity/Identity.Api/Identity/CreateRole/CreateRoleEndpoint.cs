using Carter;
using Mapster;
using MediatR;

namespace Identity.Api.Identity.CreateRole;



public record CreateRoleRequest(string RoleName);
public record CreateRoleResponse(bool IsSuccess);


public class CreateRoleEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/roles", async (ISender sender, CreateRoleRequest request) =>
        {
            var result = await sender.Send(new CreateRoleCommand(request.RoleName));
            var response = result.Adapt<CreateRoleResponse>();
            return Results.Created("/roles", response);
        })
        .Produces<CreateRoleResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("CreateRole")
        .WithDescription("CreateRole")
        .WithSummary("CreateRole")
        .RequireAuthorization("IsAdmin");
    }
}