using System.Net.Http.Json;
using EWallet.Client.Models.Identity;
using EWallet.Client.Services;

namespace EWallet.Client.Services.Identity;

public sealed class UsersApiClient
{
    private readonly HttpClient _httpClient;

    public UsersApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GetUsersResponse> GetUsersAsync(
        int pageIndex = 0,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var requestUri =
            $"identity-service/users?PageIndex={pageIndex}&PageSize={pageSize}";

        using var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result = await response.Content.ReadFromJsonAsync<GetUsersResponse>(
            cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The users response was empty.");
    }

    public async Task<GetRolesByUserIdResponse> GetRolesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"identity-service/users/{userId}/roles",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<GetRolesByUserIdResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The roles response was empty.");
    }
}
