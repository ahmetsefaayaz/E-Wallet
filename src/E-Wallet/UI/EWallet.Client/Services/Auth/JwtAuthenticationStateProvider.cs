using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace EWallet.Client.Services.Auth;

public sealed class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly TokenStorage _tokenStorage;

    public JwtAuthenticationStateProvider(TokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenStorage.GetTokenAsync();
        return CreateAuthenticationState(token);
    }

    public void NotifyUserAuthentication(string token)
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(CreateAuthenticationState(token)));
    }

    private static AuthenticationState CreateAuthenticationState(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            var claims = ReadClaims(token);
            var identity = new ClaimsIdentity(
                claims,
                authenticationType: "jwt",
                nameType: ClaimTypes.Name,
                roleType: ClaimTypes.Role);

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (FormatException)
        {
            return AnonymousState();
        }
        catch (JsonException)
        {
            return AnonymousState();
        }
    }

    private static IEnumerable<Claim> ReadClaims(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            throw new FormatException("The JWT format is invalid.");
        }

        var payload = parts[1]
            .Replace('-', '+')
            .Replace('_', '/');

        payload = payload.PadRight(
            payload.Length + (4 - payload.Length % 4) % 4,
            '=');

        var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        using var document = JsonDocument.Parse(json);

        foreach (var property in document.RootElement.EnumerateObject())
        {
            var claimType = property.Name switch
            {
                "role" => ClaimTypes.Role,
                "roles" => ClaimTypes.Role,
                "sub" => ClaimTypes.NameIdentifier,
                _ => property.Name
            };

            if (property.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var value in property.Value.EnumerateArray())
                {
                    if (value.ValueKind == JsonValueKind.String)
                    {
                        yield return new Claim(claimType, value.GetString()!);
                    }
                }

                continue;
            }

            if (property.Value.ValueKind == JsonValueKind.String)
            {
                yield return new Claim(claimType, property.Value.GetString()!);
            }
        }
    }

    private static AuthenticationState AnonymousState()
    {
        return new AuthenticationState(
            new ClaimsPrincipal(new ClaimsIdentity()));
    }
    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(AnonymousState()));
    }
}
