using System.Net.Http.Json;
using EWallet.Client.Models.Notifications;

namespace EWallet.Client.Services.Notifications;

public sealed class NotificationApiClient
{
    private readonly HttpClient _httpClient;

    public NotificationApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GetMyNotificationsResponse> GetMyNotificationsAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            "notification-service/notifications/me",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<GetMyNotificationsResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The notifications response was empty.");
    }

    public async Task<MarkAsReadResponse> MarkAsReadAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsync(
            $"notification-service/notifications/{notificationId}/mark-as-read",
            content: null,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<MarkAsReadResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The mark-as-read response was empty.");
    }

    public async Task<GetNotificationsByUserIdResponse>
        GetNotificationsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"notification-service/users/{userId}/notifications",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<
                GetNotificationsByUserIdResponse>(cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The user notifications response was empty.");
    }
}
