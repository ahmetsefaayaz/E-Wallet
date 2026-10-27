using Carter;
using Mapster;
using MediatR;

namespace Notification.Api.Notification.Queries.GetNotificationsByUserId;


public record GetNotificationByUserIdResponse(IEnumerable<Entities.Notification> Notifications);
public class GetNotificationsByUserIdEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{userId:guid}/notifications", async (ISender sender, Guid userId) =>
        {
            var result = await sender.Send(new GetNotificationsByUserIdQuery(userId));
            var response = result.Adapt<GetNotificationByUserIdResponse>();
            return Results.Ok(response);
        })
        .WithName("GetNotificationByUserId")
        .WithDescription("Gets a notification by user")
        .WithSummary("Gets a notification by user")
        .Produces<GetNotificationByUserIdResponse>()
        .RequireAuthorization();
    }
}