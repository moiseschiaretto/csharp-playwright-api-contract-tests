namespace PublicApiContractTests.Tests.Products.Get.Status200;

[TestFixture]
public class BSuccessLimitTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Products/Get/Status200";

    // GET /products?limit=5&skip=10 - status 200
    [Test, Retry(2), Description("GET /products?limit=5&skip=10 - status 200")]
    public async Task GetProducts_WithLimitAndSkip_ReturnsStatus200()
    {
        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "GET",
            EndPoint = $"{Endpoints.Products}?limit=5&skip=10",
            ExpectedStatus = 200,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "BSuccessLimit.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);
    }
}
