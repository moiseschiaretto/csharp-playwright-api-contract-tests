namespace PublicApiContractTests.Tests.Products.Get.Status200;

[TestFixture]
public class DSuccessSearchTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Products/Get/Status200";

    // GET /products/search?q=phone - status 200
    [Test, Retry(2), Description("GET /products/search?q=phone - status 200")]
    public async Task SearchProducts_ReturnsStatus200()
    {
        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "GET",
            EndPoint = $"{Endpoints.ProductsSearch}?q=phone",
            ExpectedStatus = 200,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "DSuccessSearch.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);
    }
}
