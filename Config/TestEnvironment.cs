namespace PublicApiContractTests.Config;

// Variáveis de ambiente lidas do arquivo .env (base URL, credenciais e token).
// Chama-se TestEnvironment para não conflitar com a classe System.Environment do .NET.
public static class TestEnvironment
{
    static TestEnvironment()
    {
        var envFile = Path.Combine(ProjectPaths.Root, ".env");
        if (File.Exists(envFile))
        {
            DotNetEnv.Env.Load(envFile);
        }
    }

    public static string BaseUrl => Read("API_BASE_URL", "https://dummyjson.com");

    public static string Username => Read("AUTH_USERNAME", "emilys");

    public static string Password => Read("AUTH_PASSWORD", "emilyspass");

    // Preenchido automaticamente pelo GlobalSetup no início da execução
    public static string Token { get; set; } = string.Empty;

    public const string TokenInvalid = "token-invalido-de-proposito-para-teste-negativo";

    private static string Read(string key, string defaultValue)
    {
        var value = System.Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }
}
