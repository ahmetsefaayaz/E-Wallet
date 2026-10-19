using BuildingBlocks.Pagination;
using Carter;
using Identity.Api.Dtos;
using Mapster;
using MediatR;

namespace Identity.Api.Identity.Queries.GetUsers;

public record GetUsersResponse(PaginatedResult<UserDto> Users);
public class GetUsersEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (ISender sender, [AsParameters] PaginationRequest request) =>
        {
            var result = await sender.Send(new GetUsersQuery(request));
            var response = result.Adapt<GetUsersResponse>();
            return Results.Ok(response);
        })
        .Produces<GetUsersResponse>(StatusCodes.Status200OK)
        .WithName("GetUsersQuery")
        .WithDescription("GetUsersQuery")
        .WithSummary("GetUsersSummary")
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .RequireAuthorization("IdentityDepartment");
    }
}