namespace PublicApiContractTests.Tests.Auth.Post.Status400;

[TestFixture]
public class AErrorTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Auth/Post/Status400";

    // POST /auth/login com senha incorreta - status 400
    [Test, Retry(2), Description("POST /auth/login com senha incorreta - status 400")]
    public async Task PostAuthLogin_WrongPassword_ReturnsStatus400()
    {
        var requestBody = System.Text.Json.JsonSerializer.Serialize(new
        {
            username = TestEnvironment.Username,
            password = "senha-errada-de-proposito"
        });

        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "POST",
            EndPoint = Endpoints.AuthLogin,
            Body = requestBody,
            ExpectedStatus = 400,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "AError.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);
    }
}
