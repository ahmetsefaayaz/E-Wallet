using Carter;
using Mapster;
using MediatR;

namespace Notification.Api.Notification.Queries.GetNotificationById;

public record GetNotificationByIdResponse(Entities.Notification Notification);
public class GetNotificationByIdEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("notifications/{id:guid}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new GetNotificationByIdQuery(id));
            var response = result.Adapt<GetNotificationByIdResponse>();
            return Results.Ok(response);
        })
        .Produces<GetNotificationByIdResponse>()
        .WithName("GetNotificationById")
        .WithDescription("Gets a notification by id")
        .WithSummary("Gets a notification by id")
        .RequireAuthorization();
        
    }
}