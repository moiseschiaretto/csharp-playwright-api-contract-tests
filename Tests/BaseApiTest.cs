using Allure.NUnit;
using Microsoft.Playwright;

namespace PublicApiContractTests.Tests;

// Classe base: cria o Playwright e o contexto de requisições de API (APIRequestContext)
// uma vez por classe de teste, e libera os recursos ao final.
// O atributo [AllureNUnit] na classe base vale para todas as classes de teste que herdam dela.
[AllureNUnit]
public abstract class BaseApiTest
{
    private IPlaywright _playwright = null!;

    protected IAPIRequestContext Request { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task CreateRequestContextAsync()
    {
        _playwright = await Playwright.CreateAsync();
        Request = await _playwright.APIRequest.NewContextAsync();
    }

    [OneTimeTearDown]
    public async Task DisposeRequestContextAsync()
    {
        await Request.DisposeAsync();
        _playwright.Dispose();
    }
}