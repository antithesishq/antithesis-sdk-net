namespace Antithesis.SDK;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Threading;

[Generator(LanguageNames.CSharp)]
public sealed class CatalogGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var assertCallSites = context.SyntaxProvider
            .CreateSyntaxProvider(
                IsPossibleAssertCallSite,
                TransformAssertCallSite)
            .Where(assertCallSite => assertCallSite != null)
            .Collect();

        context.RegisterImplementationSourceOutput(assertCallSites, SourceOutput);
    }

    private static bool IsPossibleAssertCallSite(SyntaxNode node, CancellationToken _) =>
        node is MemberAccessExpressionSyntax memberAccess && _assertMethodNames.Contains(memberAccess.Name.Identifier.ValueText);

    private static readonly HashSet<string> _assertMethodNames = new(
    [
        "Always",
        "AlwaysOrUnreachable",
        "AlwaysGreaterThan",
        "AlwaysGreaterThanOrEqualTo",
        "AlwaysLessThan",
        "AlwaysLessThanOrEqualTo",
        "AlwaysSome",
        "Sometimes",
        "SometimesGreaterThan",
        "SometimesGreaterThanOrEqualTo",
        "SometimesLessThan",
        "SometimesLessThanOrEqualTo",
        "SometimesAll",
        "Unreachable",
        "Reachable"
    ]);

    private static AssertCallSite? TransformAssertCallSite(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var assertMethodCall = (MemberAccessExpressionSyntax)context.Node;

        if (context.SemanticModel.GetSymbolInfo(assertMethodCall, cancellationToken).Symbol is not IMethodSymbol assertMethod)
            return null;

        // We don't check the Assembly Name because that would make testing much more difficult for little added value.
        if (assertMethod.ContainingType.Name != "Assert"
            || assertMethod.ContainingNamespace.Name != "SDK"
            || assertMethod.ContainingNamespace.ContainingNamespace.Name != "Antithesis")
        {
            return null;
        }

        (string? callerClassName, string? callerMethodName) = GetCallerClassAndMethodNames();
        (string? assertIdIsTheMessage, DiagnosticId? diagnosticId) = GetAssertIdIsTheMessageOrDiagnosticId();

        return new AssertCallSite(
            new Caller(context.SemanticModel.Compilation.AssemblyName, callerClassName, callerMethodName, assertMethodCall.GetLocation()),
            assertMethodCall.Name.Identifier.ValueText, assertIdIsTheMessage, diagnosticId);

        (string? CallerClassName, string? CallerMethodName) GetCallerClassAndMethodNames()
        {
            const string dotnetConstructorName = ".ctor";
            const string dotnetIndexerName = "indexer";

            string? callerMethodName = null;

            var callerCrawlParent = assertMethodCall.Parent;

            while (callerCrawlParent != null)
            {
                if (callerCrawlParent is ClassDeclarationSyntax @class)
                    return (@class.Identifier.ValueText, callerMethodName);

                if (string.IsNullOrEmpty(callerMethodName))
                {
                    if (callerCrawlParent is ConstructorDeclarationSyntax constructor)
                        callerMethodName = dotnetConstructorName;
                    else if (callerCrawlParent is IndexerDeclarationSyntax indexer)
                        callerMethodName = dotnetIndexerName;
                    else if (callerCrawlParent is MethodDeclarationSyntax method)
                        callerMethodName = method.Identifier.ValueText;
                    else if (callerCrawlParent is PropertyDeclarationSyntax property)
                        callerMethodName = property.Identifier.ValueText;
                }

                callerCrawlParent = callerCrawlParent.Parent;
            }

            return (null, callerMethodName);
        }

        (string? IdIsTheMessage, DiagnosticId? DiagnosticId) GetAssertIdIsTheMessageOrDiagnosticId()
        {
            if (assertMethodCall.Parent is not InvocationExpressionSyntax assertMethodInvocation)
                return (null, DiagnosticId.SyntaxNotSupported);

            int idIsTheMessageOrdinal = assertMethod.Parameters.Single(parameter => parameter.Name == "idIsTheMessage").Ordinal;

            if (assertMethodInvocation.ArgumentList.Arguments.Count <= idIsTheMessageOrdinal)
                return (null, DiagnosticId.IdIsTheMessageSymbolAmbiguous);

            var idIsTheMessageArgument = assertMethodInvocation.ArgumentList.Arguments[idIsTheMessageOrdinal];

            // We support the following two ways of specifying the unique idIsTheMessage for an Assertion:
            // 1. A literal string passed in-line as an argument to the Assert "string idIsTheMessage" Parameter.
            // 2. A reference to a constant field with public or internal accessibility.

            var idIsTheMessageLiteral = idIsTheMessageArgument.ChildNodes().SingleOrDefault(n => n is LiteralExpressionSyntax);

            if (idIsTheMessageLiteral != null)
                return (idIsTheMessageLiteral.ToString(), null);

            var idIsTheMessageReference = idIsTheMessageArgument.ChildNodes()
                .SingleOrDefault(n => n is IdentifierNameSyntax or MemberAccessExpressionSyntax);

            if (idIsTheMessageReference == null)
                return (null, DiagnosticId.IdIsTheMessageMustBeLiteralOrConstField);
       
            var idIsTheMessageMember = context.SemanticModel.GetSymbolInfo(idIsTheMessageReference, cancellationToken);
            
            if (idIsTheMessageMember.Symbol == null)
            {
                if (idIsTheMessageMember.CandidateSymbols.Length == 0)
                    return (null, DiagnosticId.IdIsTheMessageSymbolNotFound);
                else if (idIsTheMessageMember.CandidateSymbols.Any(IsSymbolAccessible))
                    return (null, DiagnosticId.IdIsTheMessageSymbolAmbiguous);
                else
                    return (null, DiagnosticId.IdIsTheMessageMustBeAccessible);
            }

            if (!IsSymbolAccessible(idIsTheMessageMember.Symbol))
                return (null, DiagnosticId.IdIsTheMessageMustBeAccessible);

            if (idIsTheMessageMember.Symbol is not IFieldSymbol idIsTheMessageField || !idIsTheMessageField.IsConst)
                return (null, DiagnosticId.IdIsTheMessageMustBeLiteralOrConstField);
            
            return (GetSymbolFullName(idIsTheMessageField), null);
        }
    }

    private static bool IsSymbolAccessible(ISymbol symbol)
    {
        if (symbol.DeclaredAccessibility is not (Accessibility.NotApplicable or Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal))
            return false;

        if (symbol.ContainingSymbol != null)
            return IsSymbolAccessible(symbol.ContainingSymbol);

        return true;
    }

    private static string GetSymbolFullName(ISymbol symbol)
    {
        // TODO : Make sure to create valid C# identifiers (e.g. if escaping is required for keywords in the name).

        var namePartsReverse = new List<string> { symbol.Name };

        var containingType = symbol.ContainingType;

        while (true)
        {
            namePartsReverse.Add(containingType.Name);

            if (containingType.ContainingType != null)
                containingType = containingType.ContainingType;
            else
                break;
        }

        var containingNamespace = containingType.ContainingNamespace;

        while (containingNamespace != null && !containingNamespace.IsGlobalNamespace)
        {
            namePartsReverse.Add(containingNamespace.Name);

            containingNamespace = containingNamespace.ContainingNamespace;
        }

        namePartsReverse.Reverse();

        return "global::" + string.Join(".", namePartsReverse);
    }

    private static void SourceOutput(SourceProductionContext context, ImmutableArray<AssertCallSite?> assertCallSites)
    {
        foreach (var assertCallSite in assertCallSites.Where(acs => acs!.DiagnosticId != null))
            context.ReportDiagnostic(assertCallSite!.ToDiagnostic());

        // We include AssertCallSites with DiagnosticIds because ToGeneratedCode will comment out those lines.
        foreach (var assemblyAssertCallSites in assertCallSites.GroupBy(acs => acs!.Caller.AssemblyName))
        {
            string assemblyName = assemblyAssertCallSites.Key!;
            string antithesisSuffix = typeof(CatalogGenerator).FullName.Replace(".", "_");

            string fileName = $"{assemblyName}.{antithesisSuffix}.generated.cs";

            string source =
$@"#if !ANTITHESIS && !ANTITHESIS_REMOVE
#define ANTITHESIS
#endif

namespace {assemblyName}.{antithesisSuffix};

[global::System.CodeDom.Compiler.GeneratedCode(""{typeof(CatalogGenerator).FullName}"", ""{ThisAssembly.AssemblyInformationalVersion}"")]
internal static class Catalog
{{
    [global::System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Initialize()
    {{
        {string.Join("\n\n        ", assemblyAssertCallSites.Select(acs => acs!.ToGeneratedCode()))} 
    }}
}}";

            context.AddSource(fileName, source);
        }       
    }
}