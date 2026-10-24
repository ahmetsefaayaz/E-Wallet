using Account.Application.Account.Commands.CreateAccount;
using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Account.Api.Endpoints.Accounts;

public record CreateAccountRequest(Guid UserId);

public record CreateAccountResponse(Guid AccountId);

public class CreateAccountEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/accounts", async (ISender sender, CreateAccountRequest request) =>
        {
            var result = await sender.Send(new CreateAccountCommand(request.UserId));
            var response = result.Adapt<CreateAccountResponse>();
            return Results.Created($"/accounts/{response.AccountId}", response);
        })
        .WithName("CreateAccount")
        .WithDescription("Creates a new account")
        .Produces<CreateAccountResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}