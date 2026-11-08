using System.Net.Http.Json;
using EWallet.Client.Models.Accounts;
using EWallet.Client.Services;

namespace EWallet.Client.Services.Accounts;

public sealed class AccountApiClient
{
    private readonly HttpClient _httpClient;

    public AccountApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GetAccountsResponse> GetAccountsAsync(
        int pageIndex = 0,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var requestUri =
            $"account-service/accounts?PageIndex={pageIndex}&PageSize={pageSize}";

        using var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var accounts = await response.Content.ReadFromJsonAsync<GetAccountsResponse>(
            cancellationToken);

        return accounts
            ?? throw new InvalidOperationException(
                "The accounts response was empty.");
    }

    public async Task<CreateAccountResponse> CreateAccountAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsync(
            "account-service/accounts",
            content: null,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<CreateAccountResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The create-account response was empty.");
    }

    public async Task<GetAccountByIdResponse> GetAccountByIdAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"account-service/accounts/{accountId}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<GetAccountByIdResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The account response was empty.");
    }

    public async Task<DeleteAccountResponse> DeleteAccountAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync(
            $"account-service/accounts/{accountId}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<DeleteAccountResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The delete-account response was empty.");
    }

    public async Task<GetCardsByAccountIdResponse> GetCardsByAccountIdAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"account-service/accounts/{accountId}/cards",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<GetCardsByAccountIdResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The cards response was empty.");
    }

    public async Task<CreateCardResponse> CreateCardAsync(
        Guid accountId,
        CreateCardRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            $"account-service/accounts/{accountId}/cards",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<CreateCardResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The create-card response was empty.");
    }

    public async Task<UpdateCardResponse> UpdateCardAsync(
        Guid accountId,
        Guid cardId,
        UpdateCardRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            $"account-service/accounts/{accountId}/cards/{cardId}",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<UpdateCardResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The update-card response was empty.");
    }

    public async Task<DeleteCardResponse> DeleteCardAsync(
        Guid accountId,
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync(
            $"account-service/accounts/{accountId}/cards/{cardId}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await ApiException.CreateAsync(response, cancellationToken);
        }

        var result =
            await response.Content.ReadFromJsonAsync<DeleteCardResponse>(
                cancellationToken);

        return result
            ?? throw new InvalidOperationException(
                "The delete-card response was empty.");
    }
}