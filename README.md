# Antithesis .NET SDK

This package contains types for .NET applications to integrate with [Antithesis](https://antithesis.com).
* The `Assert` class methods define [test properties](https://antithesis.com/docs/introduction/how_antithesis_works/#how-does-antithesis-identify-bugs/)
about your software or [workload](https://antithesis.com/docs/product/writing_tests/test_templates/first_test/).
* The `Random` class methods source entropy directly from the Antithesis platform: raw 64-bit values via `GetRandom()`
and structured list picks via `RandomChoice()`.
* The `AntithesisRandom` class is a subclass of `System.Random` that encapsulates Antithesis's steering choices,
making it easy for .NET code that uses `System.Random` to be included in Antithesis-guided exploration.
* The `Lifecycle` class methods inform the Antithesis environment that particular test phases or milestones have been
reached.

For general usage guidance see the [Antithesis .NET SDK Documentation](https://antithesis.com/docs/using_antithesis/sdk/dotnet/).

### Notes

This .NET 6.0+ Antithesis.SDK package's only dependency is the Antithesis.SDK.SourceGenerators package. The
Antithesis.SDK.SourceGenerators package contains a .NET Incremental Source Generator that adds a C# file
to each Assembly that references it. The file contains a ModuleInitializer that calls Antithesis.SDK.Catalog
for each Antithesis.SDK.Assert method call found during compilation. This informs Antithesis of every assertion
regardless of whether the assertion is encountered during runtime.