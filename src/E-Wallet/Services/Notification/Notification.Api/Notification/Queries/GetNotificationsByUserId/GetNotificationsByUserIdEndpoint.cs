using Notification.Api.Extensions;
using Carter;
using Mapster;
using MediatR;

namespace Notification.Api.Notification.Queries.GetNotificationsByUserId;


public record GetNotificationByUserIdResponse(IEnumerable<Entities.Notification> Notifications);
public class GetNotificationsByUserIdEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{userId:guid}/notifications", async (ISender sender, Guid userId, HttpContext httpContext) =>
        {
            var currentUserId = httpContext.User.GetCurrentUserId();
            if (currentUserId is null)
                return Results.Unauthorized();
            var result = await sender.Send(new GetNotificationsByUserIdQuery(
                userId,
                currentUserId.Value,
                httpContext.User.HasClaim("Department", "Notification")));
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