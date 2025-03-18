namespace Antithesis.SDK;

using System.Diagnostics;

// LOAD BEARING : All public method signatures in this class are load bearing for the CatalogGenerator.
// TODO : Determine if Guidance is Cataloged.
public static class Catalog
{
    // No Guidance

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Always(string idIsTheMessage, LocationInfo location) =>
        NoGuidanceHelper(AssertionMethodType.Always, idIsTheMessage, location);

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysOrUnreachable(string idIsTheMessage, LocationInfo location) =>
        NoGuidanceHelper(AssertionMethodType.AlwaysOrUnreachable, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Sometimes(string idIsTheMessage, LocationInfo location) =>
        NoGuidanceHelper(AssertionMethodType.Sometimes, idIsTheMessage, location);

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Unreachable(string idIsTheMessage, LocationInfo location) =>
        NoGuidanceHelper(AssertionMethodType.Unreachable, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Reachable(string idIsTheMessage, LocationInfo location) =>
        NoGuidanceHelper(AssertionMethodType.Reachable, idIsTheMessage, location);

    private static void NoGuidanceHelper(AssertionMethodType methodType, string idIsTheMessage, LocationInfo location)
    {
        if (string.IsNullOrEmpty(idIsTheMessage))
            throw new ArgumentNullException(nameof(idIsTheMessage));

        if (location == null)
            throw new ArgumentNullException(nameof(location));

        Sink.Write(AssertionInfo.ConstructForCatalogWrite(methodType, idIsTheMessage, location));
    }

    // Numeric Guidance

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysGreaterThan(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionMethodType.AlwaysGreaterThan, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysGreaterThanOrEqualTo(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionMethodType.AlwaysGreaterThanOrEqualTo, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysLessThan(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionMethodType.AlwaysLessThan, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysLessThanOrEqualTo(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionMethodType.AlwaysLessThanOrEqualTo, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesGreaterThan(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionMethodType.SometimesGreaterThan, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesGreaterThanOrEqualTo(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionMethodType.SometimesGreaterThanOrEqualTo, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesLessThan(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionMethodType.SometimesLessThan, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesLessThanOrEqualTo(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionMethodType.SometimesLessThanOrEqualTo, idIsTheMessage, location);

    // Boolean Guidance

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysSome(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionMethodType.AlwaysSome, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesAll(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionMethodType.SometimesAll, idIsTheMessage, location);

    private static void WithGuidanceHelper(AssertionMethodType methodType, string idIsTheMessage, LocationInfo location)
    {
        if (string.IsNullOrEmpty(idIsTheMessage))
            throw new ArgumentNullException(nameof(idIsTheMessage));

        if (location == null)
            throw new ArgumentNullException(nameof(location));

        Sink.Write(AssertionInfo.ConstructForCatalogWrite(methodType, idIsTheMessage, location));
        Sink.Write(GuidanceInfo.ConstructForCatalogWrite(methodType, idIsTheMessage, location));
    }
}