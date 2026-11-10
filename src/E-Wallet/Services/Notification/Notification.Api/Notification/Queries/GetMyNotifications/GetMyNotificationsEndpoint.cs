using Notification.Api.Extensions;
using Carter;
using Mapster;
using MediatR;

namespace Notification.Api.Notification.Queries.GetMyNotifications;

public record GetMyNotificationsResponse(IEnumerable<Entities.Notification> Notifications);

public class GetMyNotificationsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/notifications/me", async (
                ISender sender, 
                HttpContext httpContext) =>
            {
                var userId = httpContext.User.GetCurrentUserId();
                if (userId is null)
                {
                    return Results.Unauthorized();
                }

                var result = await sender.Send(new GetMyNotificationsQuery(userId.Value));
                var response = result.Adapt<GetMyNotificationsResponse>();
            
                return Results.Ok(response);
            })
            .WithName("GetMyNotifications")
            .WithDescription("Gets the authenticated user's notifications")
            .WithSummary("Gets my notifications")
            .Produces<GetMyNotificationsResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }
}