using Carter;
using Mapster;
using MediatR;

namespace Notification.Api.Notification.Commands;

public record MarkAsReadResponse(bool IsSuccess);
public class MarkAsReadEndpoint: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("notifications/{id:guid}/mark-as-read", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new MarkAsReadCommand(id));
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