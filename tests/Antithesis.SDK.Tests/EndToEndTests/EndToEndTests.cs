namespace Antithesis.SDK;

using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;

public class EndToEndTests
{
    // Follow the Verify naming convention so that our gitignore applies; however, Verify includes
    // the test's name in addition to the class's, so we should never collide.
    private static readonly string _tempOutputFilePath =
        CurrentFile.Relative($"{nameof(EndToEndTests)}.received.txt");

    private static void DeleteTempOutputFile()
    {
        if (File.Exists(_tempOutputFilePath))
            File.Delete(_tempOutputFilePath);
    }

    [ModuleInitializer]
    internal static void Initialize() =>
        Environment.SetEnvironmentVariable(EnvironmentVariableNames.LocalSinkOutputFilePath, _tempOutputFilePath);

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

            return Verifier.VerifyFile(_tempOutputFilePath)
                .UseDirectory(RelativeThisDirectory("Verify"))
                .ScrubLinesWithReplace(s => s.StartsWith(_sdkSentinel)
                    ? _sdkVersionScrubber.Replace(s, "$1...SCRUBBED...$3") 
                    : s)
                .AddScrubber(sb =>
                {
                    // There is no "good" way to run Regex Match (or Replace) on a StringBuilder
                    // because internally StringBuilder uses a Linked List to optimize for Append but
                    // Regex requires / assumes O(1) indexing for performance.
                    string s = sb.ToString();

                    s = _stackTraceScrubber.Replace(s, "$1...SCRUBBED...$3");
                    
                    sb.Clear();
                    sb.Append(s);

                    return;
                });
        }
        finally { DeleteTempOutputFile(); }
    }

    private const string _sdkSentinel = @"{""antithesis_sdk"":{""language"":{""name"":""C#"",""version"":""";
    private static readonly Regex _sdkVersionScrubber = new(@"(version"":"")([^""]+)("")");

    private static readonly Regex _stackTraceScrubber = new(@"(stack_trace"":"")([^""]+)("")");

    private static string RelativeThisDirectory(string relativePath, [CallerFilePath] string? callerFilePath = null) =>
        Path.Combine(Path.GetDirectoryName(callerFilePath)!, relativePath);
}