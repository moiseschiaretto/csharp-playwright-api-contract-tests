using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PublicApiContractTests.Utils;

public enum ValidationMode
{
    // Aceita campos extras na resposta (padrão para APIs públicas)
    AllowExtra,
    // Rejeita qualquer campo que não exista na amostra
    ExactSchema
}

// TESTE DE SCHEMA — valida ESTRUTURA e TIPOS a partir de uma amostra (fixture),
// sem comparar valores exatos. Mesma lógica do Joi + to-json-schema do projeto original:
// - campos da amostra são opcionais; se vierem, precisam ter o tipo certo
// - inteiro e decimal são tratados como numérico
// - strings de data ISO exigem data válida; strings "http..." exigem URI válida
public static partial class SchemaValidator
{
    public static void ValidateResponseWithJsonSchema(string responseJson, string expectedJson,
        ValidationMode mode = ValidationMode.AllowExtra)
    {
        using var actual = JsonDocument.Parse(responseJson);
        using var expected = JsonDocument.Parse(expectedJson);

        var errors = new List<string>();
        Validate(expected.RootElement, actual.RootElement, "$", mode, errors);

        Assert.That(errors, Is.Empty, $"Erro de validação de schema: {string.Join("; ", errors)}");
        TestContext.Progress.WriteLine("✅ Schema validado com sucesso (estrutura e tipos conferem)");
    }

    private static void Validate(JsonElement expected, JsonElement actual, string path,
        ValidationMode mode, List<string> errors)
    {
        switch (expected.ValueKind)
        {
            case JsonValueKind.Object:
                if (actual.ValueKind != JsonValueKind.Object)
                {
                    errors.Add($"{path}: esperado objeto, recebido {actual.ValueKind}");
                    return;
                }
                foreach (var property in expected.EnumerateObject())
                {
                    if (actual.TryGetProperty(property.Name, out var actualValue))
                    {
                        Validate(property.Value, actualValue, $"{path}.{property.Name}", mode, errors);
                    }
                }
                if (mode == ValidationMode.ExactSchema)
                {
                    foreach (var property in actual.EnumerateObject())
                    {
                        if (!expected.TryGetProperty(property.Name, out _))
                        {
                            errors.Add($"{path}.{property.Name}: campo não previsto no schema");
                        }
                    }
                }
                break;

            case JsonValueKind.Array:
                if (actual.ValueKind != JsonValueKind.Array)
                {
                    errors.Add($"{path}: esperado array, recebido {actual.ValueKind}");
                    return;
                }
                if (expected.GetArrayLength() > 0)
                {
                    var itemTemplate = expected[0];
                    var index = 0;
                    foreach (var item in actual.EnumerateArray())
                    {
                        Validate(itemTemplate, item, $"{path}[{index}]", mode, errors);
                        index++;
                    }
                }
                break;

            case JsonValueKind.String:
                if (actual.ValueKind != JsonValueKind.String)
                {
                    errors.Add($"{path}: esperado string, recebido {actual.ValueKind}");
                    return;
                }
                ValidateStringFormat(expected.GetString() ?? string.Empty, actual.GetString() ?? string.Empty, path, errors);
                break;

            case JsonValueKind.Number:
                if (actual.ValueKind != JsonValueKind.Number)
                {
                    errors.Add($"{path}: esperado número, recebido {actual.ValueKind}");
                }
                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                if (actual.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
                {
                    errors.Add($"{path}: esperado boolean, recebido {actual.ValueKind}");
                }
                break;

            default:
                // null na amostra aceita qualquer tipo
                break;
        }
    }

    private static void ValidateStringFormat(string sample, string value, string path, List<string> errors)
    {
        if (value.Length == 0)
        {
            return;
        }
        if (IsoDateRegex().IsMatch(sample) &&
            !DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _))
        {
            errors.Add($"{path}: esperado data ISO válida, recebido \"{value}\"");
        }
        if (sample.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
            !Uri.TryCreate(value, UriKind.Absolute, out _))
        {
            errors.Add($"{path}: esperado URI válida, recebido \"{value}\"");
        }
    }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}")]
    private static partial Regex IsoDateRegex();
}
