namespace EWallet.Client.Models.Notifications;

public sealed class GetMyNotificationsResponse
{
    public List<NotificationItem> Notifications { get; set; } = [];
}

public sealed class GetNotificationsByUserIdResponse
{
    public List<NotificationItem> Notifications { get; set; } = [];
}

public sealed class NotificationItem
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid AccountId { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public string? Type { get; set; }

    public int Channel { get; set; }

    public bool IsRead { get; set; }

    public Guid? ReferenceId { get; set; }

    public DateTime CreatedAt { get; set; }
}

public sealed class MarkAsReadResponse
{
    public bool IsSuccess { get; set; }
}
