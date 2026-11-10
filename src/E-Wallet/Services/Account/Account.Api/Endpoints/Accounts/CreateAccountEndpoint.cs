using Account.Api.Extensions;
using Account.Application.Account.Commands.CreateAccount;
using Carter;
using Mapster;
using MediatR;
namespace Account.Api.Endpoints.Accounts;


public record CreateAccountResponse(Guid AccountId);

public class CreateAccountEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/accounts", async (ISender sender, HttpContext httpContext) =>
        {
            var userId = httpContext.User.GetCurrentUserId();
            if (userId is null)
                return Results.Unauthorized();
            var result = await sender.Send(new CreateAccountCommand(userId.Value));
            var response = result.Adapt<CreateAccountResponse>();
            return Results.Created($"/accounts/{response.AccountId}", response);
        })
        .WithName("CreateAccount")
        .WithDescription("Creates a new account")
        .Produces<CreateAccountResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}