namespace PublicApiContractTests.Tests.Auth.Post.Status200;

[TestFixture]
public class ASuccessTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Auth/Post/Status200";

    // POST /auth/login - status 200
    [Test, Retry(2), Description("POST /auth/login - status 200")]
    public async Task PostAuthLogin_ReturnsStatus200()
    {
        // Credenciais vêm do .env (usuário de demonstração publicado pela DummyJSON)
        var requestBody = System.Text.Json.JsonSerializer.Serialize(new
        {
            username = TestEnvironment.Username,
            password = TestEnvironment.Password,
            expiresInMins = 60
        });

        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "POST",
            EndPoint = Endpoints.AuthLogin,
            Body = requestBody,
            ExpectedStatus = 200,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "ASuccess.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);

        ContractValidator.ValidateResponse(result.Body, new { username = TestEnvironment.Username });
        ContractValidator.ValidateFieldIsNonEmptyString(result.Body, "accessToken");
    }
}
