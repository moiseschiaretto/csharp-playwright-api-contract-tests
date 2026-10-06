namespace PublicApiContractTests;

// Executa uma única vez, antes de todos os testes do projeto:
// autentica na DummyJSON e disponibiliza o accessToken para os testes.
[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public async Task ObtainTokenAsync()
    {
        TestEnvironment.Token = await AuthToken.RefreshTokenAsync();
    }
}
