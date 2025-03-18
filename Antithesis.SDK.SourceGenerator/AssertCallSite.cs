namespace Antithesis.SDK;

using Microsoft.CodeAnalysis;

internal record class Caller(string? AssemblyName, string? ClassName, string? MethodName, Location Location)
{
    internal string ToGeneratedCode()
    {
        // LinePosition.Line and Character both use 0-based indexing, so we add 1 to each in the generated code.
        var line = Location.GetLineSpan();
        var start = line.StartLinePosition;

        string className = EmptyToNull(ClassName) ?? "class";
        string functionName = EmptyToNull(MethodName) ?? "function";
        string fileName = EmptyToNull(line.Path) ?? "file";

        return $@"new global::Antithesis.SDK.LocationInfo() {{ ClassName = @""{className}"", FunctionName = @""{functionName}"", FileName = @""{fileName}"", BeginLine = {start.Line + 1}, BeginColumn = {start.Character + 1}}}";

        static string? EmptyToNull(string? s) => string.IsNullOrEmpty(s) ? null : s;
    }
}

internal record class AssertCallSite(Caller Caller, string AssertMethodName, string? AssertIdIsTheMessage, DiagnosticId? DiagnosticId)
{
    internal Diagnostic ToDiagnostic() =>
        DiagnosticId != null
            ? Diagnostic.Create(DiagnosticDescriptors.ById[DiagnosticId.Value], Caller.Location)
            : throw new NotSupportedException();

    internal string ToGeneratedCode()
    {
        const string nlIndent = "\n            ";
        
        string prefix = DiagnosticId != null ? "/*" : string.Empty;
        string suffix = DiagnosticId != null ? "*/" : string.Empty; 

        return $"{prefix}global::Antithesis.SDK.Catalog.{AssertMethodName}({nlIndent}{AssertIdIsTheMessage},{nlIndent}{Caller.ToGeneratedCode()});{suffix}";
    }
}