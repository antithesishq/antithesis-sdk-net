namespace Antithesis.SDK;

using Microsoft.CodeAnalysis;

// LOAD BEARING : These enum integer values are used as Diagnostic Ids for which any change is breaking.
internal enum DiagnosticId
{
    // Error 0##
    Unspecified = 0,
    SyntaxNotSupported = 1,
    IdIsTheMessageSymbolNotFound = 2,
    IdIsTheMessageSymbolAmbiguous = 3,
    IdIsTheMessageMustBeAccessible = 4,
    IdIsTheMessageMustBeNonNullLiteralOrConstField = 5,

    // Warning 1##

    // Info 2##
    CompilerConstantNotDefined = 200, // Not yet (possibly ever) implemented.

    // Hidden 3##
}

internal static class DiagnosticIdExtensions
{
    internal static DiagnosticSeverity GetSeverity(this DiagnosticId id) => (int)id switch
    {
        < 100 => DiagnosticSeverity.Error,
        >= 100 and < 200 => DiagnosticSeverity.Warning,
        >= 200 and < 300 => DiagnosticSeverity.Info,
        >= 300 => DiagnosticSeverity.Hidden
    };
}

internal static class DiagnosticDescriptors
{
    // Diagnostic Ids should be < 15 characters long. ANTITHESIS is 10 characters long, and we follow it with 3 digits.
    private const string _idPrefix = "ANTITHESIS";

    internal static IReadOnlyDictionary<DiagnosticId, DiagnosticDescriptor> ById { get; } =
        new Dictionary<DiagnosticId, DiagnosticDescriptor>()
        {
            [DiagnosticId.Unspecified] =
                Construct(
                    DiagnosticId.Unspecified,
                    "DiagnosticId unspecified.",
                    "The DiagnosticId enum was not specified in the Source Generator. Please contact Antithesis.",
                    "InternalError"),

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

            [DiagnosticId.IdIsTheMessageMustBeNonNullLiteralOrConstField] =
                Construct(
                    DiagnosticId.IdIsTheMessageMustBeNonNullLiteralOrConstField,
                    @"""idIsTheMessage"" must be a non-null literal or a const field.",
                    @"The expression passed as an argument to any Assert ""idIsTheMessage"" parameter must be a non-null literal or a reference to a const field.",
                    "Catalog"),
            
            [DiagnosticId.CompilerConstantNotDefined] =
                Construct(
                    DiagnosticId.CompilerConstantNotDefined,
                    @"ANTITHESIS constant not defined.",
                    @"This project does not define the ANTITHESIS compiler constant; therefore, all Assert method call sites will be removed during build.",
                    "Configuration")
        };

    private static DiagnosticDescriptor Construct(DiagnosticId id, string title, string message, string subCategory) =>
        new(
            $"{_idPrefix}{(int)id:000}",
            title,
            message,
            $"{nameof(Antithesis)}.{subCategory}",
            id.GetSeverity(),
            true);
}