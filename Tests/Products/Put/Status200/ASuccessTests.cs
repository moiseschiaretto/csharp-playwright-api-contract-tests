namespace PublicApiContractTests.Tests.Products.Put.Status200;

[TestFixture]
public class ASuccessTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Products/Put/Status200";

    // PUT /products/1 - status 200
    [Test, Retry(2), Description("PUT /products/1 - status 200")]
    public async Task PutProduct_ReturnsStatus200()
    {
        var requestBody = FileHelper.LoadTestData(FolderPath, "ASuccess.json");

        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "PUT",
            EndPoint = $"{Endpoints.Products}/1",
            Token = TestEnvironment.Token,
            Body = requestBody,
            ExpectedStatus = 200,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "ASuccess.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);

        // Contrato: o título atualizado precisa voltar igual na resposta
        var title = System.Text.Json.JsonDocument.Parse(requestBody).RootElement.GetProperty("title").GetString();
        ContractValidator.ValidateResponse(result.Body, new { title });
    }
}
