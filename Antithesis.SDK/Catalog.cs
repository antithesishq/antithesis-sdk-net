namespace Antithesis.SDK;

using System.Diagnostics;

// LOAD BEARING : All public method signatures in this class are load bearing for the CatalogGenerator.

/// <summary>
/// The Catalog class is used by the Antithesis.SDK.SourceGenerator package to Catalog all method calls to Assert
/// so that Antithesis is aware of all assertions regardless of whether or not they are encountered during runtime.
/// If your project is referencing the Antithesis.SDK.SourceGenerator package, there is no need for you to call this
/// class directly.
/// </summary>
public static class Catalog
{
    #region No Guidance

#pragma warning disable 1573
    /// <summary>
    /// Used by the Antithesis.SDK.SourceGenerator package to Catalog a corresponding Assert method call so that
    /// Antithesis is aware of the assertion regardless of whether or not it is encountered during runtime.
    /// </summary>
    /// <inheritdoc cref="Assert.Always" path="/param"/>
    /// <param name="location">Provides metadata related to the source code location of the corresponding Assert.</param>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Always(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.Always, idIsTheMessage, location);
    }
#pragma warning restore 1573

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysOrUnreachable(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.AlwaysOrUnreachable, idIsTheMessage, location);
    }

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Sometimes(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.Sometimes, idIsTheMessage, location);
    }

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Unreachable(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.Unreachable, idIsTheMessage, location);
    }

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Reachable(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.Reachable, idIsTheMessage, location);
    }

    #endregion

    #region Numeric Guidance

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysGreaterThan(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.AlwaysGreaterThan, idIsTheMessage, location);
    }

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysGreaterThanOrEqualTo(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.AlwaysGreaterThanOrEqualTo, idIsTheMessage, location);
    }

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysLessThan(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.AlwaysLessThan, idIsTheMessage, location);
    }

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysLessThanOrEqualTo(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.AlwaysLessThanOrEqualTo, idIsTheMessage, location);
    }

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesGreaterThan(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.SometimesGreaterThan, idIsTheMessage, location);
    }

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesGreaterThanOrEqualTo(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.SometimesGreaterThanOrEqualTo, idIsTheMessage, location);
    }

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesLessThan(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.SometimesLessThan, idIsTheMessage, location);
    }

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesLessThanOrEqualTo(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.SometimesLessThanOrEqualTo, idIsTheMessage, location);
    }

    #endregion

    #region Boolean Guidance

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysSome(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.AlwaysSome, idIsTheMessage, location);
    }

    /// <inheritdoc cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesAll(string idIsTheMessage, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.SometimesAll, idIsTheMessage, location);
    }

    #endregion

    private static void Helper(AssertionMethodType methodType, string idIsTheMessage, LocationInfo location)
    {
        if (string.IsNullOrEmpty(idIsTheMessage))
            throw new ArgumentNullException(nameof(idIsTheMessage));

        if (location == null)
            throw new ArgumentNullException(nameof(location));

        Sink.Write(AssertionInfo.ConstructForCatalogWrite(methodType, idIsTheMessage, location));
    }
}