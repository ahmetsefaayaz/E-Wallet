using System.Net.Http.Json;
using EWallet.Client.Models.Auth;

namespace EWallet.Client.Services.Auth;

public sealed class AuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "identity-service/login",
            request,
            cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(
            cancellationToken);

        if (loginResponse is null || string.IsNullOrWhiteSpace(loginResponse.Token))
        {
            throw new InvalidOperationException(
                "The login response did not contain a valid token.");
        }

        return loginResponse;
    }
    public async Task<Guid> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "identity-service/users",
            request,
            cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result = await response.Content.ReadFromJsonAsync<RegisterResponse>(
            cancellationToken);

        if (result is null || result.Id == Guid.Empty)
        {
            throw new InvalidOperationException(
                "The registration response did not contain a valid user ID.");
        }

        return result.Id;
    }
}