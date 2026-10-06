namespace PublicApiContractTests.Tests.Products.Get.Status200;

[TestFixture]
public class ESuccessCategoriesTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Products/Get/Status200";

    // GET /products/categories - status 200
    [Test, Retry(2), Description("GET /products/categories - status 200")]
    public async Task GetProductCategories_ReturnsStatus200()
    {
        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "GET",
            EndPoint = Endpoints.ProductsCategories,
            ExpectedStatus = 200,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "ESuccessCategories.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);
    }
}
