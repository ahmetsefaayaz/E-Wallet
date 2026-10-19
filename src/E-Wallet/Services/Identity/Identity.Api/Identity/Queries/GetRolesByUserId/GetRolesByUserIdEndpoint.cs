using Carter;
using Mapster;
using MediatR;

namespace Identity.Api.Identity.Queries.GetRolesByUserId;

public record GetRolesByUserIdResponse(IEnumerable<string> Roles);
public class GetRolesByUserIdEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{id:guid}/roles", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new GetRolesByUserIdQuery(id));
            var response = result.Adapt<GetRolesByUserIdResponse>();
            return Results.Ok(response);
        })
        .Produces<GetRolesByUserIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("GetRolesByUserId")
        .WithDescription("Get roles by user id")
        .WithSummary("Get roles by user id")
        .RequireAuthorization("IdentityDepartment");
    }
}