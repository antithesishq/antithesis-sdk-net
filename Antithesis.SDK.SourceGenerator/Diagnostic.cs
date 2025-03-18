namespace Antithesis.SDK;

using Microsoft.CodeAnalysis;

// LOAD BEARING : These enum integer values are used as Diagnostic Ids for which any change is breaking.
internal enum DiagnosticId
{
    SyntaxNotSupported = 1,
    IdIsTheMessageSymbolNotFound = 2,
    IdIsTheMessageSymbolAmbiguous = 3,
    IdIsTheMessageMustBeAccessible = 4,
    IdIsTheMessageMustBeLiteralOrConstField = 5
}

internal static class DiagnosticDescriptors
{
    // Diagnostic Ids should be < 15 characters long. ANTITHESIS is 10 characters long, and we follow it with 3 digits.
    private const string _idPrefix = "ANTITHESIS";

    internal static IReadOnlyDictionary<DiagnosticId, DiagnosticDescriptor> ById { get; } =
        new Dictionary<DiagnosticId, DiagnosticDescriptor>()
        {
            [DiagnosticId.SyntaxNotSupported] =
                Construct(
                    DiagnosticId.SyntaxNotSupported,
                    "This syntax is not supported.",
                    "This syntax is not supported. Please contact Antithesis.",
                    "InternalError"),

            [DiagnosticId.IdIsTheMessageSymbolNotFound] =
                Construct(
                    DiagnosticId.IdIsTheMessageSymbolNotFound,
                    "A symbol could not be found.",
                    "A symbol could not be found. Please contact Antithesis.",
                    "InternalError"),

            [DiagnosticId.IdIsTheMessageSymbolAmbiguous] =
                Construct(
                    DiagnosticId.IdIsTheMessageSymbolAmbiguous,
                    @"""idIsTheMessage"" is ambiguous.",
                    @"The expression passed as an argument to this Assert ""idIsTheMessage"" parameter is ambiguous.",
                    "Catalog"),

            [DiagnosticId.IdIsTheMessageMustBeAccessible] =
                Construct(
                    DiagnosticId.IdIsTheMessageMustBeAccessible,
                    @"""idIsTheMessage"" must be accessible.",
                    @"The expression passed as an argument to any Assert ""idIsTheMessage"" parameter must accessible to public or internal.",
                    "Catalog"),

            [DiagnosticId.IdIsTheMessageMustBeLiteralOrConstField] =
                Construct(
                    DiagnosticId.IdIsTheMessageMustBeLiteralOrConstField,
                    @"""idIsTheMessage"" must be a literal or a const field.",
                    @"The expression passed as an argument to any Assert ""idIsTheMessage"" parameter must be a literal or a reference to a const field.",
                    "Catalog")
        };

    private static DiagnosticDescriptor Construct(DiagnosticId id, string title, string message, string subCategory) =>
        new(
            $"{_idPrefix}{(int)id:000}",
            title,
            message,
            $"{nameof(Antithesis)}.{subCategory}",
            DiagnosticSeverity.Error,
            true);
}