namespace Antithesis.SDK;

using Microsoft.CodeAnalysis;
using System.IO;

internal record class Caller(string? AssemblyName, string? ClassName, string? MethodName, Location Location)
{
    internal string ToGeneratedCode(string? projectDirectory)
    {
        // LinePosition.Line and Character both use 0-based indexing, so we add 1 to each in the generated code.
        var line = Location.GetLineSpan();
        var start = line.StartLinePosition;

        // Defaults are copied from the Antithesis Java SDK.
        string className = EmptyToNull(ClassName) ?? "class";
        string methodName = EmptyToNull(MethodName) ?? "function";
        string filePath = EmptyToNull(TryGetSolutionRelativeFilePath()) ?? "file";

        return $@"new global::Antithesis.SDK.LocationInfo() {{ ClassName = @""{className}"", MethodName = @""{methodName}"", FilePath = @""{filePath}"", BeginLine = {start.Line + 1}, BeginColumn = {start.Character + 1}}}";

        static string? EmptyToNull(string? s) => string.IsNullOrEmpty(s) ? null : s;

        string? TryGetSolutionRelativeFilePath()
        {
            if (string.IsNullOrEmpty(line.Path) || string.IsNullOrEmpty(projectDirectory))
                return line.Path;

            // If projectDirectory ends with a directory separator, Path.GetDirectoryName will return the projectDirectory
            // without the trailing separator, which is not what we want here.
            projectDirectory = projectDirectory!.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string solutionDirectory = Path.GetDirectoryName(projectDirectory);

            // We trim any leading directory separators to not give the false impression of an absolute path.
            return (!string.IsNullOrEmpty(solutionDirectory) && line.Path.Length > solutionDirectory.Length && line.Path.StartsWith(solutionDirectory))
                ? line.Path.Substring(solutionDirectory.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                : line.Path;
        }
    }
}

internal record class AssertInvocation(Caller Caller, string AssertMethodName, string? AssertIdIsTheMessage, DiagnosticId? DiagnosticId)
{
    internal Diagnostic ToDiagnostic() =>
        DiagnosticId != null
            ? Diagnostic.Create(DiagnosticDescriptors.ById[DiagnosticId.Value], Caller.Location)
            : throw new NotSupportedException();

    internal string ToGeneratedCode(string? projectDirectory)
    {
        const string nlIndent = "\n            ";

        bool hasError = DiagnosticId != null && DiagnosticId.Value.GetSeverity() == DiagnosticSeverity.Error;
        
        string prefix = hasError ? "/*" : string.Empty;
        string suffix = hasError ? "*/" : string.Empty;

        return $"{prefix}global::Antithesis.SDK.Catalog.{AssertMethodName}({nlIndent}{AssertIdIsTheMessage},{nlIndent}{Caller.ToGeneratedCode(projectDirectory)});{suffix}";
    }
}