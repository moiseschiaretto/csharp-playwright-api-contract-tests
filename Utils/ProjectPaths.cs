namespace PublicApiContractTests.Utils;

// Localiza a raiz do projeto (pasta do .csproj), porque o dotnet test
// executa a partir de bin/Debug/net8.0 e os recursos ficam na raiz.
public static class ProjectPaths
{
    public static string Root { get; } = FindRoot();

    public static string Fixtures => Path.Combine(Root, "Resources", "Fixtures");

    public static string Data => Path.Combine(Root, "Resources", "Data");

    public static string Logs => Path.Combine(Root, "Logs");

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (directory.GetFiles("*.csproj").Length > 0)
            {
                return directory.FullName;
            }
            directory = directory.Parent;
        }
        return Directory.GetCurrentDirectory();
    }
}
