using Account.Application.Account.Queries.GetAccountById;
using Account.Application.Dtos;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Accounts;

public record GetAccountByIdResponse(GetAccountDto Account);

public class GetAccountByIdEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("accounts/{id:guid}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new GetAccountByIdQuery(id));
            var response = result.Adapt<GetAccountByIdResponse>();
            return Results.Ok(response);
        })
            .Produces<GetAccountByIdResponse>()
            .WithName("GetAccountById")
            .WithDescription("Get account by id")
            .WithSummary("Get account by id");
    }
}