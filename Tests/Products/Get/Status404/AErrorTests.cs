namespace PublicApiContractTests.Tests.Products.Get.Status404;

[TestFixture]
public class AErrorTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Products/Get/Status404";

    // GET /products/999999 - status 404
    [Test, Retry(2), Description("GET /products/999999 - status 404")]
    public async Task GetProductById_NotFound_ReturnsStatus404()
    {
        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "GET",
            EndPoint = $"{Endpoints.Products}/999999",
            ExpectedStatus = 404,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "AError.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);
    }
}
