using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PublicApiContractTests.Utils;

public static class FileHelper
{
    private static readonly string[] HttpMethods = { "Get", "Post", "Put", "Delete", "Patch" };

    private static readonly JsonSerializerOptions IndentedJson = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    // Salva a response real em Logs/{folderPath}/{Recurso}_{Metodo}_{status}_{Cenario}_{yyyyMMdd_HHmm}.json
    // Exemplo: Logs/PublicApi/Products/Get/Status200/Products_Get_200_ASuccess_20261006_1935.json
    public static void SaveResponseToFile(string responseBody, string folderPath)
    {
        var parts = folderPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        var methodIndex = Array.FindIndex(parts, p => HttpMethods.Contains(p, StringComparer.OrdinalIgnoreCase));
        var method = methodIndex >= 0 ? parts[methodIndex] : "Unknown";
        var resource = methodIndex > 0 ? parts[methodIndex - 1] : "Resource";

        var statusFolder = parts.LastOrDefault(p => p.StartsWith("Status", StringComparison.OrdinalIgnoreCase)) ?? "Status";
        var status = statusFolder.Substring("Status".Length);

        var directory = Path.Combine(new[] { ProjectPaths.Logs }.Concat(parts).ToArray());
        Directory.CreateDirectory(directory);

        // Remove apenas os logs anteriores do mesmo cenário, preservando os demais da pasta
        var prefix = $"{resource}_{method}_{status}_{CurrentScenario()}_";
        foreach (var oldFile in Directory.GetFiles(directory, $"{prefix}*.json"))
        {
            File.Delete(oldFile);
        }

        var filePath = Path.Combine(directory, $"{prefix}{Timestamp()}.json");
        File.WriteAllText(filePath, PrettyPrint(responseBody), Encoding.UTF8);
    }

    // Carrega o "contrato" esperado (schema de amostra) em Resources/Fixtures
    public static string LoadExpectedData(string folderPath, string fileName)
    {
        return File.ReadAllText(Path.Combine(ProjectPaths.Fixtures, ToSystemPath(folderPath), fileName), Encoding.UTF8);
    }

    // Carrega a massa de teste (body da requisição) em Resources/Data
    public static string LoadTestData(string folderPath, string fileName)
    {
        return File.ReadAllText(Path.Combine(ProjectPaths.Data, ToSystemPath(folderPath), fileName), Encoding.UTF8);
    }

    public static string PrettyPrint(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(document.RootElement, IndentedJson);
        }
        catch (JsonException)
        {
            return json;
        }
    }

    private static string ToSystemPath(string folderPath) => folderPath.Replace('/', Path.DirectorySeparatorChar);

    // Nome do cenário a partir da classe de teste: ASuccessTests -> ASuccess
    private static string CurrentScenario()
    {
        var className = TestContext.CurrentContext.Test.ClassName ?? "Scenario";
        var shortName = className[(className.LastIndexOf('.') + 1)..];
        return shortName.EndsWith("Tests", StringComparison.Ordinal) ? shortName[..^"Tests".Length] : shortName;
    }

    // Data e hora no fuso de São Paulo, em formato ordenável
    private static string Timestamp()
    {
        var now = DateTime.UtcNow;
        foreach (var zoneId in new[] { "America/Sao_Paulo", "E. South America Standard Time" })
        {
            try
            {
                var zone = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(now, zone).ToString("yyyyMMdd_HHmm");
            }
            catch (Exception)
            {
                // tenta o próximo identificador de fuso
            }
        }
        return DateTime.Now.ToString("yyyyMMdd_HHmm");
    }
}
