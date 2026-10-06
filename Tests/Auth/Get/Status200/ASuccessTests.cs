namespace PublicApiContractTests.Tests.Auth.Get.Status200;

[TestFixture]
public class ASuccessTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Auth/Get/Status200";

    // GET /auth/me - status 200
    [Test, Retry(2), Description("GET /auth/me - status 200")]
    public async Task GetAuthMe_ReturnsStatus200()
    {
        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "GET",
            EndPoint = Endpoints.AuthMe,
            Token = TestEnvironment.Token,
            ExpectedStatus = 200,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "ASuccess.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);
    }
}
