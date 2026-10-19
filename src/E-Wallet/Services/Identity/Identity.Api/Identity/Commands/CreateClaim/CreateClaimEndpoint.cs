using Carter;
using Mapster;
using MediatR;

namespace Identity.Api.Identity.Commands.CreateClaim;

public record CreateClaimRequest(string Type, string Value);
public record CreateClaimResponse(bool IsSuccess);
public class CreateClaimEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/{id:guid}/claims", async (ISender sender, Guid id, CreateClaimRequest request) =>
        {
            var result = await sender.Send(new CreateClaimCommand(id, request.Type, request.Value));
            var response = result.Adapt<CreateClaimResponse>();
            return Results.Ok(response);
        })
        .WithName("CreateClaim")
        .Produces<CreateClaimResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("CreateClaim")
        .WithSummary("CreateClaim")
        .RequireAuthorization("IdentityDepartment");
    }
}