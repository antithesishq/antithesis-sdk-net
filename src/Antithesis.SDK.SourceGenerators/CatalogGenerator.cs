namespace Antithesis.SDK;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Threading;

[Generator(LanguageNames.CSharp)]
public sealed class CatalogGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var projectDirectory = context.AnalyzerConfigOptionsProvider
            .Select(TryGetProjectDirectory);

        var assertInvocations = context.SyntaxProvider
            .CreateSyntaxProvider(
                IsPossibleAssertInvocation,
                TransformAssertInvocation)
            .Where(assertInvocation => assertInvocation != null)
            .Collect();

        var projectAssertInvocations = projectDirectory.Combine(assertInvocations);

        context.RegisterImplementationSourceOutput(projectAssertInvocations, SourceOutput);
    }

    // For the magic keys: https://stackoverflow.com/questions/65070796/source-generator-information-about-referencing-project
    private const string ProjectDirectoryMagicKey1 = "build_property.MSBuildProjectDirectory";
    private const string ProjectDirectoryMagicKey2 = "build_property.projectdir";

    private static string? TryGetProjectDirectory(AnalyzerConfigOptionsProvider provider, CancellationToken _) =>
        provider.GlobalOptions.TryGetValue(ProjectDirectoryMagicKey1, out string? projectDirectory1)
            ? projectDirectory1
            : (provider.GlobalOptions.TryGetValue(ProjectDirectoryMagicKey2, out string? projectDirectory2)
                ? projectDirectory2
                : null);

    private static bool IsPossibleAssertInvocation(SyntaxNode node, CancellationToken _) =>
        node is InvocationExpressionSyntax invocation
            && invocation.ArgumentList.Arguments.Count > 0
            && _assertMethodNames.Contains(GetPossibleAssertMethodName(invocation) ?? string.Empty);

    // MemberAccessExpressionSyntax example (far more common):
    // Assert.Always(true, "Id");
    //
    // IdentifierNameSyntax example:
    // using static Assert;
    // Always(true, "Id");
    private static string? GetPossibleAssertMethodName(InvocationExpressionSyntax invocation) =>
        invocation.Expression is MemberAccessExpressionSyntax memberAccess
            ? memberAccess.Name.Identifier.ValueText
            : (invocation.Expression is IdentifierNameSyntax identifier
                ? identifier.ToString()
                : null);

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

    private static AssertInvocation? TransformAssertInvocation(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var assertInvocation = (InvocationExpressionSyntax)context.Node;
        var assertMethod = GetAssertMethodSymbol(context, cancellationToken, assertInvocation);

        if (assertMethod == null)
            return null;

        int assertArgumentsSpecified = assertInvocation.ArgumentList.Arguments.Count(arg => !string.IsNullOrWhiteSpace(arg.ToString()));
        int assertParametersRequired = assertMethod.Parameters.Count(p => !p.HasExplicitDefaultValue);

        if (assertArgumentsSpecified < assertParametersRequired)
            return null;

        (string? callerClassName, string? callerMethodName) = GetAssertCallerClassAndMethodNames(assertInvocation);

        (string? assertIdIsTheMessage, DiagnosticId? diagnosticId) =
            GetAssertIdIsTheMessageOrDiagnosticId(context, cancellationToken, assertInvocation, assertMethod);

        return new AssertInvocation(
            new Caller(context.SemanticModel.Compilation.AssemblyName, callerClassName, callerMethodName,
                LocationSlim.FromLocation(assertInvocation.GetLocation())),
            GetPossibleAssertMethodName(assertInvocation)!, assertIdIsTheMessage, diagnosticId);
    }

    private static IMethodSymbol? GetAssertMethodSymbol(GeneratorSyntaxContext context, CancellationToken cancellationToken,
        InvocationExpressionSyntax assertInvocation)
    {
        var symbolInfo = context.SemanticModel.GetSymbolInfo(assertInvocation, cancellationToken);

        if (symbolInfo.Symbol != null)
        {
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

            return IsAssertMethod(methodSymbol) ? methodSymbol : null;
        }

        // When writing some of our Tests, we found that our CSharpCompilation configuration (and our lack of knowledge of how to use
        // it properly) would result in calls to generic Assert methods (i.e. the Guidance related methods) having
        // SemanticModel.GetSymbolInfo.Symbol == null with CandidateSymbols having one OverloadResolutionFailure. We've chosen to fallback
        // to CandidateSymbols to make those tests "work", and because doing so should not cause any negative impacts (and possibly have
        // other positive impacts related to developer experience with Diagnostics).

        // Avoid LINQ because source generation is performance sensitive. No benchmarking was performed, so this may be a premature
        // optimization; however, the LINQ version offers little readability or code length benefits anyways.

        foreach (var candidateSymbol in symbolInfo.CandidateSymbols)
        {
            var methodSymbol = candidateSymbol as IMethodSymbol;

            if (IsAssertMethod(methodSymbol))
                return methodSymbol;
        }

        return null;

        static bool IsAssertMethod(IMethodSymbol? symbol) =>
            symbol != null
                && symbol.ContainingType.Name == "Assert"
                && symbol.ContainingNamespace.Name == "SDK"
                && symbol.ContainingNamespace.ContainingNamespace.Name == "Antithesis"
                && symbol.ContainingAssembly.Name == "Antithesis.SDK";
    }

    private static (string? CallerClassName, string? CallerMethodName) GetAssertCallerClassAndMethodNames(InvocationExpressionSyntax assertInvocation)
    {
        const string dotnetConstructorName = ".ctor";
        const string dotnetIndexerName = "indexer";

        string? callerMethodName = null;

        var callerCrawlParent = assertInvocation.Parent;

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

    private static (string? IdIsTheMessage, DiagnosticId? DiagnosticId) GetAssertIdIsTheMessageOrDiagnosticId(
        GeneratorSyntaxContext context, CancellationToken cancellationToken,
        InvocationExpressionSyntax assertInvocation, IMethodSymbol assertMethod)
    {
        int idIsTheMessageOrdinal = assertMethod.Parameters.Single(parameter => parameter.Name == "idIsTheMessage").Ordinal;

        if (assertInvocation.ArgumentList.Arguments.Count <= idIsTheMessageOrdinal)
            return (null, DiagnosticId.IdIsTheMessageSymbolAmbiguous);

        var idIsTheMessageArgument = assertInvocation.ArgumentList.Arguments[idIsTheMessageOrdinal];

        // We support the following two ways of specifying the unique idIsTheMessage for an Assertion:
        // 1. A literal string passed in-line as an argument to the Assert "string idIsTheMessage" Parameter.
        // 2. A reference to a constant field with public or internal accessibility.

        var idIsTheMessageLiteral = idIsTheMessageArgument.ChildNodes().SingleOrDefault(n => n is LiteralExpressionSyntax);

        if (idIsTheMessageLiteral != null)
        {
            // Other relevant SyntaxKinds include DefaultLiteralExpression and NullLiteralExpression.
            return idIsTheMessageLiteral.IsKind(SyntaxKind.StringLiteralExpression)
                ? (idIsTheMessageLiteral.ToString(), null)
                : (null, DiagnosticId.IdIsTheMessageMustBeNonNullLiteralOrConstField);
        }

        var idIsTheMessageReference = idIsTheMessageArgument.ChildNodes()
            .SingleOrDefault(n => n is IdentifierNameSyntax or MemberAccessExpressionSyntax);

        if (idIsTheMessageReference == null)
            return (null, DiagnosticId.IdIsTheMessageMustBeNonNullLiteralOrConstField);

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
            return (null, DiagnosticId.IdIsTheMessageMustBeNonNullLiteralOrConstField);

        return (GetSymbolFullName(idIsTheMessageField), null);
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

    private static void SourceOutput(SourceProductionContext context,
        (string? ProjectDirectory, ImmutableArray<AssertInvocation?> AssertInvocations) provider)
    {
        foreach (var assertCallSite in provider.AssertInvocations.Where(ai => ai!.DiagnosticId != null))
            context.ReportDiagnostic(assertCallSite!.ToDiagnostic());

        // We include AssertInvocations with DiagnosticIds because ToGeneratedCode will comment out those lines.
        foreach (var assemblyAssertInvocations in provider.AssertInvocations.GroupBy(ai => ai!.Caller.AssemblyName))
        {
            string assemblyName = assemblyAssertInvocations.Key!;
            string antithesisSuffix = typeof(CatalogGenerator).FullName.Replace(".", "_");

            string fileName = $"{assemblyName}.{antithesisSuffix}.generated.cs";

            string source =
$@"namespace {assemblyName}.{antithesisSuffix};

[global::System.CodeDom.Compiler.GeneratedCode(""{typeof(CatalogGenerator).FullName}"", ""{ThisAssembly.AssemblyInformationalVersion}"")]
internal static class Catalog
{{
    [global::System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Initialize()
    {{
        {string.Join("\n\n        ", assemblyAssertInvocations.Select(ai => ai!.ToGeneratedCode(provider.ProjectDirectory)))} 
    }}
}}";

            context.AddSource(fileName, source);
        }
    }
}