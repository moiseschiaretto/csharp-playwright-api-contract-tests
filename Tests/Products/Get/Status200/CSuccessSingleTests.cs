namespace PublicApiContractTests.Tests.Products.Get.Status200;

[TestFixture]
public class CSuccessSingleTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Products/Get/Status200";

    // GET /products/1 - status 200
    [Test, Retry(2), Description("GET /products/1 - status 200")]
    public async Task GetProductById_ReturnsStatus200()
    {
        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "GET",
            EndPoint = $"{Endpoints.Products}/1",
            ExpectedStatus = 200,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "CSuccessSingle.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);
    }
}
