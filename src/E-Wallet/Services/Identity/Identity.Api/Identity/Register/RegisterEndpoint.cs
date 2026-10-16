using Carter;
using Mapster;
using MediatR;

namespace Identity.Api.Identity.Register;

public record RegisterRequest(string UserName, string Email, string Password, string ConfirmPassword);
public class RegisterEndpoint:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users", async (ISender sender, RegisterRequest request) =>
        {
            var command = request.Adapt<RegisterCommand>();
            var result = await sender.Send(command);
            return Results.Created("/users", result);
        })
        .WithName("Register")
        .Produces<RegisterCommandResult>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithDescription("Register")
        .WithSummary("Register");
    }
}