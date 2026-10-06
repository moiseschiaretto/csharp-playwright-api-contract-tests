namespace PublicApiContractTests.Tests.Products.Post.Status201;

[TestFixture]
public class ASuccessTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Products/Post/Status201";

    // POST /products/add - status 201
    [Test, Retry(2), Description("POST /products/add - status 201")]
    public async Task PostProduct_ReturnsStatus201()
    {
        var requestBody = FileHelper.LoadTestData(FolderPath, "ASuccess.json");

        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "POST",
            EndPoint = Endpoints.ProductsAdd,
            Token = TestEnvironment.Token,
            Body = requestBody,
            ExpectedStatus = 201,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "ASuccess.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);

        // Contrato: o título enviado precisa voltar igual na resposta
        var title = System.Text.Json.JsonDocument.Parse(requestBody).RootElement.GetProperty("title").GetString();
        ContractValidator.ValidateResponse(result.Body, new { title });
    }
}
