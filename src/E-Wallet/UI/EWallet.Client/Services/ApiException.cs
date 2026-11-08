using System.Net;
using System.Text.Json;

namespace EWallet.Client.Services;

public sealed class ApiException : Exception
{
    public ApiException(
        string message,
        HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }

    public static async Task<ApiException> CreateAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        var responseBody = await response.Content.ReadAsStringAsync(
            cancellationToken);

        var message = ExtractMessage(responseBody);

        return new ApiException(message, response.StatusCode);
    }

    private static string ExtractMessage(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return "The request failed.";
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            if (root.TryGetProperty("detail", out var detail) &&
                detail.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(detail.GetString()))
            {
                return detail.GetString()!;
            }

            if (root.TryGetProperty("title", out var title) &&
                title.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(title.GetString()))
            {
                return title.GetString()!;
            }
        }
        catch (JsonException)
        {
            // The response was not ProblemDetails JSON.
        }

        return responseBody;
    }
}