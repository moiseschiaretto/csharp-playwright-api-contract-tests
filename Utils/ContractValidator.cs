using System.Text.Json;

namespace PublicApiContractTests.Utils;

// TESTE DE CONTRATO — valida VALORES exatos de campos estáveis
// (ex.: o título enviado no POST precisa voltar igual na resposta).
// A resposta pode ter outros campos; só os informados em "expected" são comparados.
public static class ContractValidator
{
    public static void ValidateResponse(string responseJson, object expected)
    {
        using var actual = JsonDocument.Parse(responseJson);
        using var expectedDocument = JsonDocument.Parse(JsonSerializer.Serialize(expected));

        var errors = new List<string>();
        Compare(expectedDocument.RootElement, actual.RootElement, "$", errors);

        Assert.That(errors, Is.Empty, $"Erro de validação de contrato: {string.Join("; ", errors)}");
        TestContext.Progress.WriteLine("✅ Contrato validado com sucesso (valores conferem)");
    }

    public static void ValidateFieldIsNonEmptyString(string responseJson, string fieldName)
    {
        using var actual = JsonDocument.Parse(responseJson);
        Assert.That(actual.RootElement.TryGetProperty(fieldName, out var field), Is.True,
            $"Campo \"{fieldName}\" ausente na resposta");
        Assert.That(field.ValueKind, Is.EqualTo(JsonValueKind.String), $"Campo \"{fieldName}\" não é string");
        Assert.That(field.GetString(), Is.Not.Empty, $"Campo \"{fieldName}\" está vazio");
    }

    private static void Compare(JsonElement expected, JsonElement actual, string path, List<string> errors)
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
                    if (!actual.TryGetProperty(property.Name, out var actualValue))
                    {
                        errors.Add($"{path}.{property.Name}: campo ausente");
                        continue;
                    }
                    Compare(property.Value, actualValue, $"{path}.{property.Name}", errors);
                }
                break;

            case JsonValueKind.Array:
                if (actual.ValueKind != JsonValueKind.Array || actual.GetArrayLength() != expected.GetArrayLength())
                {
                    errors.Add($"{path}: array diferente do esperado");
                    return;
                }
                for (var i = 0; i < expected.GetArrayLength(); i++)
                {
                    Compare(expected[i], actual[i], $"{path}[{i}]", errors);
                }
                break;

            case JsonValueKind.Number:
                if (actual.ValueKind != JsonValueKind.Number || actual.GetDecimal() != expected.GetDecimal())
                {
                    errors.Add($"{path}: esperado {expected.GetRawText()}, recebido {actual.GetRawText()}");
                }
                break;

            case JsonValueKind.String:
                if (actual.ValueKind != JsonValueKind.String || actual.GetString() != expected.GetString())
                {
                    errors.Add($"{path}: esperado \"{expected.GetString()}\", recebido {actual.GetRawText()}");
                }
                break;

            default:
                if (actual.ValueKind != expected.ValueKind)
                {
                    errors.Add($"{path}: esperado {expected.GetRawText()}, recebido {actual.GetRawText()}");
                }
                break;
        }
    }
}
