namespace Antithesis.SDK;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.IO;

// Adapted from (also renamed in order to not collide with the existing Antithesis.SDK.LocationInfo):
// https://andrewlock.net/creating-a-source-generator-part-9-avoiding-performance-pitfalls-in-incremental-generators/#6-take-care-with-diagnostics
internal record LocationSlim(string? FilePath, TextSpan TextSpan, LinePositionSpan LineSpan)
{
    internal static LocationSlim FromLocation(Location location) =>
        new(location.SourceTree?.FilePath, location.SourceSpan, location.GetLineSpan().Span);

    internal Location ToLocation() =>
        FilePath != null
            ? Location.Create(FilePath, TextSpan, LineSpan)
            : Location.None;
}

// AssemblyName is used to Group AssertInvocations when generating source code files.
internal record class Caller(string? AssemblyName, string? ClassName, string? MethodName, LocationSlim Location)
{
    internal string ToGeneratedCode(string? projectDirectory)
    {
        // Defaults are copied from the Antithesis Java SDK.
        string className = EmptyToNull(ClassName) ?? "class";
        string methodName = EmptyToNull(MethodName) ?? "function";
        string relativeFilePath = EmptyToNull(TryGetSolutionRelativeFilePath()) ?? "file";

        // LinePosition.Line and Character both use 0-based indexing, so we add 1 to each in the generated code.
        var start = Location.LineSpan.Start;
        int beginLine = start.Line + 1;
        int beginColumn = start.Character + 1;

        return $@"new global::Antithesis.SDK.LocationInfo() {{ ClassName = @""{className}"", MethodName = @""{methodName}"", FilePath = @""{relativeFilePath}"", BeginLine = {beginLine}, BeginColumn = {beginColumn}}}";

        static string? EmptyToNull(string? s) => string.IsNullOrEmpty(s) ? null : s;

        string? TryGetSolutionRelativeFilePath()
        {
            string? filePath = Location.FilePath;

            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(projectDirectory))
                return filePath;

            // If projectDirectory ends with a directory separator, Path.GetDirectoryName will return the projectDirectory
            // without the trailing separator, which is not what we want here.
            projectDirectory = projectDirectory!.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string solutionDirectory = Path.GetDirectoryName(projectDirectory);

            // We trim any leading directory separators to not give the false impression of an absolute path.
            bool isSolutionRelative = !string.IsNullOrEmpty(solutionDirectory)
                && filePath!.Length > solutionDirectory.Length
                && filePath.StartsWith(solutionDirectory);

            return isSolutionRelative
                ? filePath!.Substring(solutionDirectory.Length)
                    .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                : filePath;
        }
    }
}

internal record class AssertInvocation(Caller Caller, string AssertMethodName, string? AssertMessage, DiagnosticId? DiagnosticId)
{
    internal Diagnostic ToDiagnostic() =>
        DiagnosticId != null
            ? Diagnostic.Create(DiagnosticDescriptors.ById[DiagnosticId.Value], Caller.Location.ToLocation())
            : throw new NotSupportedException();

    internal string ToGeneratedCode(string? projectDirectory)
    {
        const string nlIndent = "\n            ";

        bool hasError = DiagnosticId != null && DiagnosticId.Value.GetSeverity() == DiagnosticSeverity.Error;
        
        string prefix = hasError ? "/*" : string.Empty;
        string suffix = hasError ? "*/" : string.Empty;

        string? assertMessageLiteral = !string.IsNullOrEmpty(AssertMessage)
            ? SymbolDisplay.FormatLiteral(AssertMessage!, true)
            : null;

        return $"{prefix}global::Antithesis.SDK.Catalog.{AssertMethodName}({nlIndent}{assertMessageLiteral},{nlIndent}{Caller.ToGeneratedCode(projectDirectory)});{suffix}";
    }
}