using System.Text.Json;
using Microsoft.Playwright;

namespace PublicApiContractTests.Utils;

// Parâmetros de uma requisição (equivalente ao objeto de configuração do makeRequest)
public sealed class ApiRequestOptions
{
    public required string Method { get; init; }
    public required string EndPoint { get; init; }
    public required int ExpectedStatus { get; init; }
    public string BaseUrl { get; init; } = TestEnvironment.BaseUrl;
    public string? Token { get; init; }
    public string? Body { get; init; }
    public Dictionary<string, string> Headers { get; init; } = new();
    public bool ValidateStatus { get; init; } = true;
    public bool ShowResponseLog { get; init; }
    public string? SaveResponseLog { get; init; }
}

public sealed record ApiResult(IAPIResponse Response, int Status, string Body);

public static class RequestHelper
{
    // Método único para todos os verbos HTTP (GET, POST, PUT, DELETE, PATCH)
    public static async Task<ApiResult> MakeRequestAsync(IAPIRequestContext request, ApiRequestOptions options)
    {
        var headers = new Dictionary<string, string> { ["Accept"] = "application/json" };
        if (!string.IsNullOrEmpty(options.Token))
        {
            headers["Authorization"] = $"Bearer {options.Token}";
        }
        if (options.Body is not null)
        {
            headers["Content-Type"] = "application/json";
        }
        foreach (var header in options.Headers)
        {
            headers[header.Key] = header.Value;
        }

        var requestOptions = new APIRequestContextOptions
        {
            Method = options.Method.ToUpperInvariant(),
            Headers = headers
        };
        if (options.Body is not null)
        {
            requestOptions.DataObject = JsonSerializer.Deserialize<JsonElement>(options.Body);
        }

        var url = $"{options.BaseUrl}{options.EndPoint}";
        IAPIResponse response;
        try
        {
            response = await request.FetchAsync(url, requestOptions);
        }
        catch (PlaywrightException ex)
        {
            TestContext.Progress.WriteLine($"Erro ao realizar a requisição {options.Method}: {ex.Message}");
            throw;
        }

        var body = await response.TextAsync();

        if (options.ShowResponseLog)
        {
            TestContext.Progress.WriteLine("\n=== RESPONSE LOG ===");
            TestContext.Progress.WriteLine($"{options.Method} {url}");
            TestContext.Progress.WriteLine($"Status: {response.Status}");
            TestContext.Progress.WriteLine(FileHelper.PrettyPrint(body));
            TestContext.Progress.WriteLine("==================\n");
        }

        if (options.SaveResponseLog is not null)
        {
            FileHelper.SaveResponseToFile(body, options.SaveResponseLog);
        }

        if (options.ValidateStatus)
        {
            Assert.That(response.Status, Is.EqualTo(options.ExpectedStatus),
                $"Status inesperado em {options.Method} {url}. Body: {body}");
        }

        return new ApiResult(response, response.Status, body);
    }
}
