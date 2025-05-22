//HintName: SomeCompany.SomeProject.Antithesis_SDK_CatalogGenerator.generated.cs
namespace SomeCompany.SomeProject.Antithesis_SDK_CatalogGenerator;

[global::System.CodeDom.Compiler.GeneratedCode("Antithesis.SDK.CatalogGenerator", "...SCRUBBED...")]
internal static class Catalog
{
    [global::System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Initialize()
    {
        global::Antithesis.SDK.Catalog.Always(
            "Constructor Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @".ctor", FilePath = @"file", BeginLine = 10, BeginColumn = 9});

        global::Antithesis.SDK.Catalog.AlwaysGreaterThan(
            "Ids Field1 Const",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"SomeMethodBlock", FilePath = @"file", BeginLine = 15, BeginColumn = 9});

        global::Antithesis.SDK.Catalog.Reachable(
            "SomeMethodBlock Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"SomeMethodBlock", FilePath = @"file", BeginLine = 16, BeginColumn = 9});

        global::Antithesis.SDK.Catalog.AlwaysSome(
            "SomeMethodExpression Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"SomeMethodExpression", FilePath = @"file", BeginLine = 20, BeginColumn = 9});

        global::Antithesis.SDK.Catalog.Sometimes(
            "SomeProperty Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"SomeProperty", FilePath = @"file", BeginLine = 26, BeginColumn = 13});

        global::Antithesis.SDK.Catalog.Sometimes(
            "Ids Field2 Const",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"SomeProperty", FilePath = @"file", BeginLine = 29, BeginColumn = 16});

        global::Antithesis.SDK.Catalog.Reachable(
            "Class Member Const Field",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"indexer", FilePath = @"file", BeginLine = 32, BeginColumn = 39}); 
    }
}