namespace Antithesis.SDK;

using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;

public class EndToEndTest
{
    // Follow the Verify naming convention so that our gitignore applies; however, Verify includes
    // the test's name in addition to the class's, so we should never collide.
    private static readonly string _tempOutputFilePath =
        CurrentFile.Relative($"{nameof(EndToEndTest)}.received.txt");

    private static void DeleteTempOutputFile()
    {
        if (File.Exists(_tempOutputFilePath))
            File.Delete(_tempOutputFilePath);
    }

    [ModuleInitializer]
    internal static void Initialize() =>
        Environment.SetEnvironmentVariable(Sink.LocalSink.FilePathEnvironmentVariableName, _tempOutputFilePath);

    [Fact]
    public Task SomeCompanySomeConsole()
    {
        try
        {
            DeleteTempOutputFile();

            // Because 1) CatalogGenerator uses ModuleInitializer, 2) the Antithesis.SDK heavily
            // uses statics for Sink, AssertionTracker, GuidanceTracker, etc., and 3) .NET cannot
            // unload an Assembly, we can only perform one test with this Assembly.
            AppDomain.CurrentDomain.ExecuteAssembly("SomeCompany.SomeConsole.dll");

            // We preserve as much of the antithesis_sdk JSONL as possible for minimal effort.
            const string sdkSentinel = "{\"antithesis_sdk\":{\"language\":{\"name\":\"C#\",\"version\":\".NET";

            return Verifier.VerifyFile(_tempOutputFilePath)
                .UseDirectory(nameof(EndToEndTest))
                .ScrubLinesWithReplace(s => s.StartsWith(sdkSentinel) ? (sdkSentinel + " ... SCRUBBED VERSION INFO") : s);
        }
        finally { DeleteTempOutputFile(); }
    }
}