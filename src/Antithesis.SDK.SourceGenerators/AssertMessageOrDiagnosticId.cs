namespace Antithesis.SDK;

using Microsoft.CodeAnalysis.CSharp;

internal record AssertMessageOrDiagnosticId
{
    internal AssertMessageOrDiagnosticId(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            DiagnosticId = SDK.DiagnosticId.MessageMustContainNonWhiteSpace;
        else if (message.Any(c => char.IsWhiteSpace(c) && c != ' '))
            DiagnosticId = SDK.DiagnosticId.MessageWhiteSpaceMustBeSpaceChar;
        else
            Message = message;
    }

    internal AssertMessageOrDiagnosticId(DiagnosticId diagnosticId) =>
        DiagnosticId = diagnosticId;

    internal string? Message { get; }
    internal DiagnosticId? DiagnosticId { get; }

    internal string? MessageLiteral => !string.IsNullOrEmpty(Message)
        ? SymbolDisplay.FormatLiteral(Message!, true)
        : null;
}