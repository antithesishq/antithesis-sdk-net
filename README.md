# Antithesis .NET SDK

This package contains types for .NET applications to use to integrate with [Antithesis](https://antithesis.com).
* The `Assert` class enables defining [test properties](https://antithesis.com/docs/using_antithesis/properties/)
about your program or [workload](https://antithesis.com/docs/getting_started/first_test/).
* The `Lifecycle` class contains methods used to inform Antithesis that particular test phases or milestones have been reached.
* The `Random` class is a subclass of `System.Random` that encapsulates Antithesis's deterministic and reproducible
random number generator.

For general usage guidance see the [Antithesis .NET SDK Documentation](https://antithesis.com/docs/using_antithesis/sdk/dotnet_sdk/).

### Notes

This .NET 6.0+ Antithesis.SDK package's only dependency is the Antithesis.SDK.SourceGenerators package. The
Antithesis.SDK.SourceGenerators package contains a .NET Incremental Source Generator that adds a C# file
to each Assembly that references it. The file contains a ModuleInitializer that calls Antithesis.SDK.Catalog
for each Antithesis.SDK.Assert method call found during compilation. This informs Antithesis of every assertion
regardless of whether or not the assertion is encountered during runtime.