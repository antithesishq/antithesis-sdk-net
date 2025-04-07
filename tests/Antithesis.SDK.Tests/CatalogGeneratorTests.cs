namespace Antithesis.SDK;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;

// Adapted from https://andrewlock.net/creating-a-source-generator-part-2-testing-an-incremental-generator-with-snapshot-testing/
public sealed class CatalogGeneratorTests
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
            references: CreateReferences(
                // Several of these Assemblies overlap because we are being thorough regarding Types in Assert's methods' signatures.
                typeof(object),
                typeof(IComparable<>),
                typeof(IConvertible),
                typeof(IReadOnlyDictionary<,>),
                typeof(Dictionary<,>),
                typeof(System.Text.Json.Nodes.JsonObject),
                typeof(Antithesis.SDK.Assert)),
            syntaxTrees: new[] { syntaxTree });
        
        static IEnumerable<MetadataReference> CreateReferences(params Type[] types) =>
            types.Select(type => type.Assembly.Location)
                .Distinct()
                .Select(path => MetadataReference.CreateFromFile(path));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new CatalogGenerator());
        driver = driver.RunGenerators(compilation);

        // Scrub the GeneratedCodeAttribute because it contains version information that will always change.
        return Verifier.Verify(driver)
            .UseDirectory(Path.Combine(nameof(CatalogGeneratorTests), "Verify"))
            .UseParameters(fileNameNoExtension)
            .ScrubLinesWithReplace(s => s.StartsWith(_generatedCodeSentinel)
                ? _generatedCodeVersionScrubber.Replace(s, "$1...SCRUBBED...$3") 
                : s);
    }

    private const string _generatedCodeSentinel = @"[global::System.CodeDom.Compiler.GeneratedCode(""Antithesis.SDK.CatalogGenerator"", """;
    private static readonly Regex _generatedCodeVersionScrubber = new(@"(""Antithesis.SDK.CatalogGenerator"", "")([^""]+)("")");

    public static IEnumerable<object[]> GetFiles()
    {
        string directory = Path.Combine(ThisDirectory(), nameof(CatalogGeneratorTests));

        return Directory.GetFiles(directory)
            .Select(filePath => new object[] { Path.GetFileNameWithoutExtension(filePath), File.ReadAllText(filePath) });
    }

    private static string ThisDirectory([CallerFilePath] string? callerFilePath = null) =>
        Path.GetDirectoryName(callerFilePath)!;
}