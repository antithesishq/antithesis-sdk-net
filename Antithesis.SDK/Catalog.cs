namespace Antithesis.SDK;

using System.Diagnostics;

// TODO : Determine if Guidance is Cataloged.
public static class Catalog
{
    // No Guidance

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Always(string idIsTheMessage, LocationInfo location) =>
        NoGuidanceHelper(AssertionVerboseType.Always, idIsTheMessage, location);

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysOrUnreachable(string idIsTheMessage, LocationInfo location) =>
        NoGuidanceHelper(AssertionVerboseType.AlwaysOrUnreachable, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Sometimes(string idIsTheMessage, LocationInfo location) =>
        NoGuidanceHelper(AssertionVerboseType.Sometimes, idIsTheMessage, location);

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Unreachable(string idIsTheMessage, LocationInfo location) =>
        NoGuidanceHelper(AssertionVerboseType.Unreachable, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Reachable(string idIsTheMessage, LocationInfo location) =>
        NoGuidanceHelper(AssertionVerboseType.Reachable, idIsTheMessage, location);

    private static void NoGuidanceHelper(AssertionVerboseType verboseType, string idIsTheMessage, LocationInfo location)
    {
        if (string.IsNullOrEmpty(idIsTheMessage))
            throw new ArgumentNullException(nameof(idIsTheMessage));

        if (location == null)
            throw new ArgumentNullException(nameof(location));

        Sink.Write(AssertionInfo.ConstructForCatalogWrite(verboseType, idIsTheMessage, location));
    }

    // Numeric Guidance

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysGreaterThan(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionVerboseType.AlwaysGreaterThan, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysGreaterThanOrEqualTo(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionVerboseType.AlwaysGreaterThanOrEqualTo, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysLessThan(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionVerboseType.AlwaysLessThan, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysLessThanOrEqualTo(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionVerboseType.AlwaysLessThanOrEqualTo, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesGreaterThan(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionVerboseType.SometimesGreaterThan, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesGreaterThanOrEqualTo(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionVerboseType.SometimesGreaterThanOrEqualTo, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesLessThan(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionVerboseType.SometimesLessThan, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesLessThanOrEqualTo(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionVerboseType.SometimesLessThanOrEqualTo, idIsTheMessage, location);

    // Boolean Guidance

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysSome(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionVerboseType.AlwaysSome, idIsTheMessage, location);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesAll(string idIsTheMessage, LocationInfo location) =>
        WithGuidanceHelper(AssertionVerboseType.SometimesAll, idIsTheMessage, location);

    private static void WithGuidanceHelper(AssertionVerboseType verboseType, string idIsTheMessage, LocationInfo location)
    {
        if (string.IsNullOrEmpty(idIsTheMessage))
            throw new ArgumentNullException(nameof(idIsTheMessage));

        if (location == null)
            throw new ArgumentNullException(nameof(location));

        Sink.Write(AssertionInfo.ConstructForCatalogWrite(verboseType, idIsTheMessage, location));
        Sink.Write(GuidanceInfo.ConstructForCatalogWrite(verboseType, idIsTheMessage, location));
    }
}