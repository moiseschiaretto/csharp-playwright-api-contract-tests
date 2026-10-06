using System.Text.Json;
using Microsoft.Playwright;

namespace PublicApiContractTests.Utils;

// Autentica na API pública DummyJSON e guarda o accessToken em cache local
// (.token-cache.json, fora do versionamento).
public static class AuthToken
{
    public static async Task<string> RefreshTokenAsync()
    {
        TestContext.Progress.WriteLine($"🔑 Autenticando na DummyJSON como \"{TestEnvironment.Username}\"...");

        using var playwright = await Playwright.CreateAsync();
        var request = await playwright.APIRequest.NewContextAsync();

        try
        {
            var response = await request.PostAsync($"{TestEnvironment.BaseUrl}{Endpoints.AuthLogin}", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" },
                DataObject = new
                {
                    username = TestEnvironment.Username,
                    password = TestEnvironment.Password,
                    expiresInMins = 60
                }
            });

            if (!response.Ok)
            {
                throw new InvalidOperationException($"Falha ao autenticar: HTTP {response.Status}");
            }

            using var document = JsonDocument.Parse(await response.TextAsync());
            var token = document.RootElement.GetProperty("accessToken").GetString()
                ?? throw new InvalidOperationException("accessToken ausente na resposta de login");

            var cache = JsonSerializer.Serialize(
                new { accessToken = token, obtainedAt = DateTime.UtcNow.ToString("o") },
                new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(Path.Combine(ProjectPaths.Root, ".token-cache.json"), cache);

            TestContext.Progress.WriteLine("✅ Token obtido e salvo em .token-cache.json");
            return token;
        }
        finally
        {
            await request.DisposeAsync();
        }
    }
}
