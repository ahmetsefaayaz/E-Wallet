using Account.Application.Account.Commands.DeleteAccount;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Accounts;

public record DeleteAccountResponse(bool IsSuccess);

public class DeleteAccountEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/accounts/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteAccountCommand(id));
            var response = result.Adapt<DeleteAccountResponse>();
            return Results.Ok(response);
        })
        .WithName("DeleteAccount")
        .Produces<DeleteAccountResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithDescription("Delete account")
        .WithSummary("Delete account")
        .RequireAuthorization("IsAdmin");
    }
}