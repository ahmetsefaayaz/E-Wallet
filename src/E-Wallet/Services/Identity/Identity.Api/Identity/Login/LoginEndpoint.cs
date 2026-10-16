using Carter;
using Mapster;
using MediatR;

namespace Identity.Api.Identity.Login;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token);
public class LoginEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (ISender sender, LoginRequest request) =>
        {
            var command = request.Adapt<LoginCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<LoginResponse>();
            return Results.Ok(response);
        })
        .WithName("Login")
        .WithDescription("Login")
        .WithSummary("Login")
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}