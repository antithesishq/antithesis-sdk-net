namespace Antithesis.SDK;

// LOAD BEARING : All public method signatures in this class are load bearing for the CatalogGenerator.

/// <summary>
/// The Catalog class is used by the Antithesis.SDK.SourceGenerators package to Catalog all method calls to Assert
/// so that Antithesis is aware of all assertions regardless of whether or not they are encountered during runtime.
/// <br/><br/>
/// You should not call this class directly; it is exclusively used by the Rosyln Analyzers contained in the
/// Antithesis.SDK.SourceGenerators package.
/// </summary>
public static class Catalog
{
    #region No Guidance

    /// <summary>
    /// Used by the Antithesis.SDK.SourceGenerators package to Catalog a corresponding Assert method call so that
    /// Antithesis is aware of the assertion regardless of whether or not it is encountered during runtime.
    /// </summary>
    /// <param name="message"><inheritdoc cref="Assert.NoGuidanceHelper" path="/param[@name='message']"/></param>
    /// <param name="location">Provides metadata related to the source code location of the corresponding Assert.</param>
    public static void Always(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.Always, message, location);
    }

    /// <inheritdoc cref="Always"/>
    public static void AlwaysOrUnreachable(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.AlwaysOrUnreachable, message, location);
    }

    /// <inheritdoc cref="Always"/>
    public static void Sometimes(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.Sometimes, message, location);
    }

    /// <inheritdoc cref="Always"/>
    public static void Unreachable(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.Unreachable, message, location);
    }

    /// <inheritdoc cref="Always"/>
    public static void Reachable(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.Reachable, message, location);
    }

    #endregion

    #region Numeric Guidance

    /// <inheritdoc cref="Always"/>
    public static void AlwaysGreaterThan(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.AlwaysGreaterThan, message, location);
    }

    /// <inheritdoc cref="Always"/>
    public static void AlwaysGreaterThanOrEqualTo(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.AlwaysGreaterThanOrEqualTo, message, location);
    }

    /// <inheritdoc cref="Always"/>
    public static void AlwaysLessThan(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.AlwaysLessThan, message, location);
    }

    /// <inheritdoc cref="Always"/>
    public static void AlwaysLessThanOrEqualTo(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.AlwaysLessThanOrEqualTo, message, location);
    }

    /// <inheritdoc cref="Always"/>
    public static void SometimesGreaterThan(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.SometimesGreaterThan, message, location);
    }

    /// <inheritdoc cref="Always"/>
    public static void SometimesGreaterThanOrEqualTo(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.SometimesGreaterThanOrEqualTo, message, location);
    }

    /// <inheritdoc cref="Always"/>
    public static void SometimesLessThan(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.SometimesLessThan, message, location);
    }

    /// <inheritdoc cref="Always"/>
    public static void SometimesLessThanOrEqualTo(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.SometimesLessThanOrEqualTo, message, location);
    }

    #endregion

    #region Boolean Guidance

    /// <inheritdoc cref="Always"/>
    public static void AlwaysSome(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.AlwaysSome, message, location);
    }

    /// <inheritdoc cref="Always"/>
    public static void SometimesAll(string message, LocationInfo location)
    {
        if (!Sink.IsNoop)
            Helper(AssertionMethodType.SometimesAll, message, location);
    }

    #endregion

    private static void Helper(AssertionMethodType methodType, string message, LocationInfo location)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException(nameof(message));

        if (location == null)
            throw new ArgumentNullException(nameof(location));

        Sink.Write(AssertionInfo.ConstructForCatalogWrite(methodType, message, location));
    }
}