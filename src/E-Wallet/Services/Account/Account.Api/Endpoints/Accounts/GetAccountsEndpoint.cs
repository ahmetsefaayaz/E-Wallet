using Account.Application.Account.Queries.GetAccounts;
using Account.Application.Dtos;
using BuildingBlocks.Pagination;
using Carter;
using Mapster;
using MediatR;

namespace Account.Api.Endpoints.Accounts;

public record GetAccountsResponse(PaginatedResult<GetAccountDto> PaginatedResult);
public class GetAccountsEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("accounts", async (ISender sender, [AsParameters] PaginationRequest request) =>
            {
                var result = await sender.Send(new GetAccountsQuery(request));
                var response = result.Adapt<GetAccountsResponse>();
                return Results.Ok(response);
            })
            .Produces<GetAccountsResponse>()
            .WithName("GetAccounts")
            .WithDescription("GetAccounts")
            .WithSummary("GetAccounts")
            .RequireAuthorization();
    }
}