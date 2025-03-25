namespace Antithesis.SDK;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;

// Adapted from https://andrewlock.net/creating-a-source-generator-part-2-testing-an-incremental-generator-with-snapshot-testing/
public sealed class CatalogGeneratorTest
{
    [ModuleInitializer]
    internal static void Initialize() =>
        VerifySourceGenerators.Initialize();

    [Theory]
    [MemberData(nameof(GetFiles))]
    public Task Files(string fileNameNoExtension, string sourceCode)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        var compilation = CSharpCompilation.Create(
            assemblyName: "SomeCompany.SomeProject",
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Dictionary<,>).Assembly.Location)
            },
            syntaxTrees: new[] { syntaxTree });

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new CatalogGenerator());
        driver = driver.RunGenerators(compilation);

        // Scrub the GeneratedCodeAttribute because it contains version information that will always change.
        return Verifier.Verify(driver)
            .UseDirectory(Path.Combine(nameof(CatalogGeneratorTest), "Verify"))
            .UseParameters(fileNameNoExtension)
            .ScrubLinesContaining("System.CodeDom.Compiler.GeneratedCode");
    }

    public static IEnumerable<object[]> GetFiles()
    {
        string directory = Path.Combine(ThisDirectory(), nameof(CatalogGeneratorTest));

        return Directory.GetFiles(directory)
            .Select(filePath => new object[] { Path.GetFileNameWithoutExtension(filePath), File.ReadAllText(filePath) });
    }

    private static string ThisDirectory([CallerFilePath] string? callerFilePath = null) =>
        Path.GetDirectoryName(callerFilePath)!;
}