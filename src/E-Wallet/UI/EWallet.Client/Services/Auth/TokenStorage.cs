using Microsoft.JSInterop;

namespace EWallet.Client.Services.Auth;

public sealed class TokenStorage
{
    private const string TokenKey = "ewallet_access_token";

    private readonly IJSRuntime _jsRuntime;

    public TokenStorage(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask SetTokenAsync(string token)
    {
        return _jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            TokenKey,
            token);
    }

    public ValueTask<string?> GetTokenAsync()
    {
        return _jsRuntime.InvokeAsync<string?>(
            "localStorage.getItem",
            TokenKey);
    }

    public ValueTask RemoveTokenAsync()
    {
        return _jsRuntime.InvokeVoidAsync(
            "localStorage.removeItem",
            TokenKey);
    }
}