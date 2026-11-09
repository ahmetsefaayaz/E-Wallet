using System.Net.Http.Json;
using EWallet.Client.Models.Identity;
using EWallet.Client.Services;

namespace EWallet.Client.Services.Identity;

public sealed class IdentityAdministrationApiClient
{
    private readonly HttpClient _httpClient;

    public IdentityAdministrationApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CreateClaimResponse> CreateClaimAsync(
        Guid userId,
        CreateClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            $"identity-service/users/{userId}/claims",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<CreateClaimResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The create-claim response was empty.");
    }

    public async Task<CreateRoleResponse> CreateRoleAsync(
        CreateRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "identity-service/roles",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<CreateRoleResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The create-role response was empty.");
    }

    public async Task<AddRoleResponse> AddRoleAsync(
        Guid userId,
        AddRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            $"identity-service/users/{userId}/roles",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<AddRoleResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The add-role response was empty.");
    }

    public async Task<CreateAddressResponse> CreateAddressAsync(
        CreateAddressRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "identity-service/users/address",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<CreateAddressResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The create-address response was empty.");
    }

    public async Task<UpdateAddressResponse> UpdateAddressAsync(
        UpdateAddressRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            "identity-service/users/address",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<UpdateAddressResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The update-address response was empty.");
    }

    public async Task<GetMyAddressResponse> GetMyAddressAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            "identity-service/users/me/address",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<GetMyAddressResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The my-address response was empty.");
    }
}
