﻿//HintName: SomeCompany.SomeProject.Antithesis_SDK_CatalogGenerator.generated.cs
namespace SomeCompany.SomeProject.Antithesis_SDK_CatalogGenerator;

[global::System.CodeDom.Compiler.GeneratedCode("Antithesis.SDK.CatalogGenerator", "...SCRUBBED...")]
internal static class Catalog
{
    [global::System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Initialize()
    {
        global::Antithesis.SDK.Catalog.Always(
            "Constructor Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @".ctor", FilePath = @"file", BeginLine = 10, BeginColumn = 13});

        global::Antithesis.SDK.Catalog.AlwaysGreaterThan(
            global::SomeCompany.SomeProject.Ids.Field1,
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"SomeMethodBlock", FilePath = @"file", BeginLine = 15, BeginColumn = 13});

        global::Antithesis.SDK.Catalog.Reachable(
            "SomeMethodBlock Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"SomeMethodBlock", FilePath = @"file", BeginLine = 16, BeginColumn = 13});

        global::Antithesis.SDK.Catalog.AlwaysSome(
            "SomeMethodExpression Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"SomeMethodExpression", FilePath = @"file", BeginLine = 20, BeginColumn = 13});

        global::Antithesis.SDK.Catalog.Sometimes(
            "SomeProperty Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"SomeProperty", FilePath = @"file", BeginLine = 26, BeginColumn = 17});

        global::Antithesis.SDK.Catalog.Sometimes(
            global::SomeCompany.SomeProject.Ids.Field2,
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"SomeProperty", FilePath = @"file", BeginLine = 29, BeginColumn = 20});

        global::Antithesis.SDK.Catalog.Reachable(
            global::SomeCompany.SomeProject.SomeClass.Id,
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"indexer", FilePath = @"file", BeginLine = 32, BeginColumn = 43});

        global::Antithesis.SDK.Catalog.Always(
            "Destructor Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"Finalize", FilePath = @"file", BeginLine = 38, BeginColumn = 13});

        global::Antithesis.SDK.Catalog.Always(
            "Conversion Operator Literal 1",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"byte", FilePath = @"file", BeginLine = 43, BeginColumn = 13});

        global::Antithesis.SDK.Catalog.Always(
            "Conversion Operator Literal 2",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"System.String", FilePath = @"file", BeginLine = 50, BeginColumn = 13});

        global::Antithesis.SDK.Catalog.Always(
            "Operator Overload Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"==", FilePath = @"file", BeginLine = 57, BeginColumn = 13});

        global::Antithesis.SDK.Catalog.Always(
            "Event Add Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"SomeEvent", FilePath = @"file", BeginLine = 64, BeginColumn = 19});

        global::Antithesis.SDK.Catalog.Always(
            "Event Remove Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeClass", MethodName = @"SomeEvent", FilePath = @"file", BeginLine = 65, BeginColumn = 22});

        global::Antithesis.SDK.Catalog.Always(
            "SomeStruct SomeMethod Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeStruct", MethodName = @"SomeMethod", FilePath = @"file", BeginLine = 71, BeginColumn = 37});

        global::Antithesis.SDK.Catalog.Always(
            "SomeRecord SomeMethod Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeRecord", MethodName = @"SomeMethod", FilePath = @"file", BeginLine = 78, BeginColumn = 13});

        global::Antithesis.SDK.Catalog.Always(
            "SomeRecord SomeMethodUsingNamedArguments Normal Order Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeRecord", MethodName = @"SomeMethodUsingNamedArguments", FilePath = @"file", BeginLine = 83, BeginColumn = 13});

        global::Antithesis.SDK.Catalog.Always(
            "SomeRecord SomeMethodUsingNamedArguments Reversed Order Literal",
            new global::Antithesis.SDK.LocationInfo() { ClassName = @"SomeRecord", MethodName = @"SomeMethodUsingNamedArguments", FilePath = @"file", BeginLine = 84, BeginColumn = 13}); 
    }
}