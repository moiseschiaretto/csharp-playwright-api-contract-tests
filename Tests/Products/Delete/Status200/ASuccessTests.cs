namespace PublicApiContractTests.Tests.Products.Delete.Status200;

[TestFixture]
public class ASuccessTests : BaseApiTest
{
    private const string FolderPath = "PublicApi/Products/Delete/Status200";

    // DELETE /products/1 - status 200
    [Test, Retry(2), Description("DELETE /products/1 - status 200")]
    public async Task DeleteProduct_ReturnsStatus200()
    {
        var result = await RequestHelper.MakeRequestAsync(Request, new ApiRequestOptions
        {
            Method = "DELETE",
            EndPoint = $"{Endpoints.Products}/1",
            Token = TestEnvironment.Token,
            ExpectedStatus = 200,
            ShowResponseLog = false,
            SaveResponseLog = FolderPath
        });

        var expectedShape = FileHelper.LoadExpectedData(FolderPath, "ASuccess.json");
        SchemaValidator.ValidateResponseWithJsonSchema(result.Body, expectedShape, ValidationMode.AllowExtra);

        // Contrato: a exclusão precisa ser confirmada na resposta
        ContractValidator.ValidateResponse(result.Body, new { isDeleted = true });
    }
}
