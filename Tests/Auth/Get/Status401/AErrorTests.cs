namespace PublicApiContractTests.Tests.Auth.Get.Status401;

[TestFixture]
public class AErrorTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Auth/Get/Status401";

    // GET /auth/me sem token - status 401
    [Test, Retry(2), Description("GET /auth/me sem token - status 401")]
    public async Task GetAuthMe_WithoutToken_ReturnsStatus401()
    {
        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "GET",
            EndPoint = Endpoints.AuthMe,
            Token = null,
            ExpectedStatus = 401,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "AError.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);
    }
}
