namespace Antithesis.SDK;

using System.Diagnostics;

// LOAD BEARING : All public method signatures in this class are load bearing for the CatalogGenerator.
public static class Catalog
{
    // No Guidance

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Always(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.Always, idIsTheMessage, location);

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysOrUnreachable(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.AlwaysOrUnreachable, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Sometimes(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.Sometimes, idIsTheMessage, location);

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Unreachable(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.Unreachable, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Reachable(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.Reachable, idIsTheMessage, location);

    // Numeric Guidance

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysGreaterThan(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.AlwaysGreaterThan, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysGreaterThanOrEqualTo(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.AlwaysGreaterThanOrEqualTo, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysLessThan(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.AlwaysLessThan, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysLessThanOrEqualTo(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.AlwaysLessThanOrEqualTo, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesGreaterThan(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.SometimesGreaterThan, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesGreaterThanOrEqualTo(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.SometimesGreaterThanOrEqualTo, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesLessThan(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.SometimesLessThan, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesLessThanOrEqualTo(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.SometimesLessThanOrEqualTo, idIsTheMessage, location);

    // Boolean Guidance

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysSome(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.AlwaysSome, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesAll(string idIsTheMessage, LocationInfo location) =>
        Helper(AssertionMethodType.SometimesAll, idIsTheMessage, location);

    // Common

    private static void Helper(AssertionMethodType methodType, string idIsTheMessage, LocationInfo location)
    {
        if (string.IsNullOrEmpty(idIsTheMessage))
            throw new ArgumentNullException(nameof(idIsTheMessage));

        if (location == null)
            throw new ArgumentNullException(nameof(location));

        Sink.Write(AssertionInfo.ConstructForCatalogWrite(methodType, idIsTheMessage, location));
    }
}