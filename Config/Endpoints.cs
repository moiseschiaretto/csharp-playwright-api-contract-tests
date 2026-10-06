namespace PublicApiContractTests.Config;

// Constantes de endpoints da API pública DummyJSON (https://dummyjson.com/docs)
public static class Endpoints
{
    public const string AuthLogin = "/auth/login";
    public const string AuthMe = "/auth/me";

    public const string Products = "/products";
    public const string ProductsAdd = "/products/add";
    public const string ProductsSearch = "/products/search";
    public const string ProductsCategories = "/products/categories";
    public const string ProductsByCategory = "/products/category/{category}";
}
