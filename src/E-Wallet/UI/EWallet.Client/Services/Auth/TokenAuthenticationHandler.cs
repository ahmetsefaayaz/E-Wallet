using System.Net.Http.Headers;

namespace EWallet.Client.Services.Auth;

public sealed class TokenAuthenticationHandler : DelegatingHandler
{
    private readonly TokenStorage _tokenStorage;

    public TokenAuthenticationHandler(TokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenStorage.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}