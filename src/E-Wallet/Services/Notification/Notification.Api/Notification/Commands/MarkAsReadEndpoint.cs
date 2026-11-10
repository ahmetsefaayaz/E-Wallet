using Notification.Api.Extensions;
using Carter;
using Mapster;
using MediatR;

namespace Notification.Api.Notification.Commands;

public record MarkAsReadResponse(bool IsSuccess);
public class MarkAsReadEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("notifications/{id:guid}/mark-as-read", async (ISender sender, Guid id, HttpContext httpContext) =>
        {
            var userId = httpContext.User.GetCurrentUserId();
            if (userId is null)
                return Results.Unauthorized();
            var result = await sender.Send(new MarkAsReadCommand(id, userId.Value));
            var response = result.Adapt<MarkAsReadResponse>();
            return Results.Ok(response);
        })
            .Produces<MarkAsReadResponse>()
            .WithName("MarkAsRead")
            .WithDescription("MarkAsRead")
            .WithSummary("MarkAsRead")
            .RequireAuthorization();
    }
}