namespace PublicApiContractTests.Tests.Products.Get.Status200;

[TestFixture]
public class FSuccessByCategoryTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Products/Get/Status200";

    // GET /products/category/smartphones - status 200
    [Test, Retry(2), Description("GET /products/category/smartphones - status 200")]
    public async Task GetProductsByCategory_ReturnsStatus200()
    {
        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "GET",
            EndPoint = Endpoints.ProductsByCategory.Replace("{category}", "smartphones"),
            ExpectedStatus = 200,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "FSuccessByCategory.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);
    }
}
