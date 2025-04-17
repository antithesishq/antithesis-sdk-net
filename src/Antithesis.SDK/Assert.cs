namespace Antithesis.SDK;

using System.Diagnostics;
using System.Text.Json.Nodes;

// LOAD BEARING : typeof(Assert).FullName and the "string idIsTheMessage" Parameters' names are load bearing for the CatalogGenerator.

/// <summary>
/// The Assert class enables defining <a href="https://antithesis.com/docs/using_antithesis/properties/">test properties</a>
/// about your program or <a href="https://antithesis.com/docs/getting_started/first_test/">workload</a>.
/// <br/><br/>
/// Each static method in this class takes a parameter called <c>idIsTheMessage</c>, which is a string literal identifier used to aggregate assertions.
/// Antithesis generates one test property per unique <c>idIsTheMessage</c>. This test property will be named <c>idIsTheMessage</c>
/// in the <a href="https://antithesis.com/docs/reports/triage/">triage report</a>.
/// <br/><br/>
/// Each static method also takes a parameter called <c>details</c>, which is an optional <c>JsonObject</c> reference of additional information provided
/// by the caller to add context to assertion passes and failures. The information that is logged will appear in the <c>logs</c> section of a
/// <a href="https://antithesis.com/docs/reports/triage/">triage report</a>.
/// </summary>
public static class Assert
{
    #region No Guidance

    /// <summary>
    /// Assert that <c>condition</c> is true every time this method is called, <b><i>and</i></b> that it is called at least once
    /// (i.e. the corresponding test property will fail if the assertion is never encountered).
    /// <br/><br/>
    /// The corresponding test property will be viewable in the <c>Antithesis SDK: Always assertions</c> group of your triage report.
    /// </summary>
    /// <inheritdoc cref="NoGuidanceHelper"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Always(bool condition, string idIsTheMessage, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            NoGuidanceHelper(AssertionMethodType.Always, condition, idIsTheMessage, details);
    }

    /// <summary>
    /// Assert that <c>condition</c> is true every time this method is called. The corresponding test property will pass if the assertion is never encountered.
    /// <br/><br/>
    /// The corresponding test property will be viewable in the <c>Antithesis SDK: Always assertions</c> group of your triage report.
    /// </summary>
    /// <inheritdoc cref="NoGuidanceHelper"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysOrUnreachable(bool condition, string idIsTheMessage, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            NoGuidanceHelper(AssertionMethodType.AlwaysOrUnreachable, condition, idIsTheMessage, details);
    }

    /// <summary>
    /// Assert that <c>condition</c> is true at least one time that this method was called. The corresponding test property will fail if the assertion is never encountered.
    /// <br/><br/>
    /// This test property will be viewable in the <c>Antithesis SDK: Sometimes assertions</c> group of your triage report.
    /// </summary>
    /// <inheritdoc cref="NoGuidanceHelper"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Sometimes(bool condition, string idIsTheMessage, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            NoGuidanceHelper(AssertionMethodType.Sometimes, condition, idIsTheMessage, details);
    }

    /// <summary>
    /// Assert that a line of code is never reached. The corresponding test property will pass if and only if the assertion is never encountered.
    /// <br/><br/>
    /// This test property will be viewable in the <c>Antithesis SDK: Reachability assertions</c> group of your triage report.
    /// </summary>
    /// <inheritdoc cref="NoGuidanceHelper"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Unreachable(string idIsTheMessage, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            NoGuidanceHelper(AssertionMethodType.Unreachable, false, idIsTheMessage, details);
    }

    /// <summary>
    /// Assert that a line of code is reached at least once. The corresponding test property will fail if the assertion is never encountered.
    /// <br/><br/>
    /// This test property will be viewable in the <c>Antithesis SDK: Reachability assertions</c> group of your triage report.
    /// </summary>
    /// <inheritdoc cref="NoGuidanceHelper"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Reachable(string idIsTheMessage, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            NoGuidanceHelper(AssertionMethodType.Reachable, true, idIsTheMessage, details);
    }

    /// <param name="methodType">The AssertionMethodType of the caller.</param>
    /// <param name="condition">The condition being asserted.</param>
    /// <param name="idIsTheMessage">
    ///     A unique string identifier of the assertion. Provides context for assertion passes and failures and is intended to be human-readable.
    ///     <b><i>Must be provided as a string literal or as a reference to a publicly accessible const field.</i></b>
    /// </param>
    /// <param name="details">Optional additional details to provide greater context for assertion passes and failures.</param>
    private static void NoGuidanceHelper(AssertionMethodType methodType, bool condition, string idIsTheMessage, JsonObject? details)
    {
        if (string.IsNullOrEmpty(idIsTheMessage))
            throw new ArgumentNullException(nameof(idIsTheMessage));

        if (AssertionTracker.ShouldWrite(idIsTheMessage, condition))
            Sink.Write(AssertionInfo.ConstructForAssertWrite(methodType, idIsTheMessage, condition, SetStackTrace(condition, details)));
    }

    #endregion

    #region Numeric Guidance

    /// <summary>
    /// <c>AlwaysGreaterThan(left, right, ...)</c> is mostly equivalent to <c>Always(left &gt; right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysGreaterThan<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.AlwaysGreaterThan, compareTo => compareTo > 0,
                left, right, idIsTheMessage, details);
        }
    }

    /// <summary>
    /// <c>AlwaysGreaterThanOrEqualTo(left, right, ...)</c> is mostly equivalent to <c>Always(left &gt;= right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysGreaterThanOrEqualTo<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.AlwaysGreaterThanOrEqualTo, compareTo => compareTo >= 0,
                left, right, idIsTheMessage, details);
        }
    }

    /// <summary>
    /// <c>AlwaysLessThan(left, right, ...)</c> is mostly equivalent to <c>Always(left &lt; right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysLessThan<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.AlwaysLessThan, compareTo => compareTo < 0,
                left, right, idIsTheMessage, details);
        }
    }

    /// <summary>
    /// <c>AlwaysLessThanOrEqualTo(left, right, ...)</c> is mostly equivalent to <c>Always(left &lt;= right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysLessThanOrEqualTo<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.AlwaysLessThanOrEqualTo, compareTo => compareTo <= 0,
                left, right, idIsTheMessage, details);
        }
    }

    /// <summary>
    /// <c>SometimesGreaterThan(left, right, ...)</c> is mostly equivalent to <c>Sometimes(left &gt; right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Sometimes"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesGreaterThan<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.SometimesGreaterThan, compareTo => compareTo > 0,
                left, right, idIsTheMessage, details);
        }
    }

    /// <summary>
    /// <c>SometimesGreaterThanOrEqualTo(left, right, ...)</c> is mostly equivalent to <c>Sometimes(left &gt;= right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Sometimes"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesGreaterThanOrEqualTo<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.SometimesGreaterThanOrEqualTo, compareTo => compareTo >= 0,
                left, right, idIsTheMessage, details);
        }
    }

    /// <summary>
    /// <c>SometimesLessThan(left, right, ...)</c> is mostly equivalent to <c>Sometimes(left &lt; right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Sometimes"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesLessThan<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.SometimesLessThan, compareTo => compareTo < 0,
                left, right, idIsTheMessage, details);
        }
    }

    /// <summary>
    /// <c>SometimesLessThanOrEqualTo(left, right, ...)</c> is mostly equivalent to <c>Sometimes(left &lt;= right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Sometimes"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesLessThanOrEqualTo<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.SometimesLessThanOrEqualTo, compareTo => compareTo <= 0,
                left, right, idIsTheMessage, details);
        }
    }

    /// <typeparam name="T">The numeric type that we are comparing.</typeparam>
    /// <param name="methodType">The AssertionMethodType of the caller.</param>
    /// <param name="compareToOperation">The operation to apply to the result of left.CompareTo(right).</param>
    /// <param name="left">The left operand of the comparison.</param>
    /// <param name="right">The right operand of the comparison.</param>
    /// <param name="idIsTheMessage"><inheritdoc cref="NoGuidanceHelper" path="/param[@name='idIsTheMessage']"/></param>
    /// <param name="details"><inheritdoc cref="NoGuidanceHelper" path="/param[@name='details']"/></param>
    private static void NumericGuidanceHelper<T>(AssertionMethodType methodType, Func<int, bool> compareToOperation,
            T left, T right, string idIsTheMessage, JsonObject? details)
        where T : struct, IComparable<T>, IConvertible
    {
        if (string.IsNullOrEmpty(idIsTheMessage))
            throw new ArgumentNullException(nameof(idIsTheMessage));

        bool condition = compareToOperation(left.CompareTo(right));

        var leftDouble = TryConvertToDouble(left);
        var rightDouble = TryConvertToDouble(right);

        static (bool Success, double Value) TryConvertToDouble(T value)
        {
            try { return (true, Convert.ToDouble(value)); }
            catch (FormatException) { return (false, default); }
            catch (InvalidCastException) { return (false, default); }
            catch (OverflowException) { return (false, default); }
        }

        bool converted = leftDouble.Success && rightDouble.Success;

        if (AssertionTracker.ShouldWrite(idIsTheMessage, condition))
            Sink.Write(AssertionInfo.ConstructForAssertWrite(methodType, idIsTheMessage, condition, SetStackTrace(condition, SetGuidanceData(details))));

        if (converted && GuidanceTracker.ShouldNumericWrite(methodType.GetGuidanceMaximize(), idIsTheMessage, leftDouble.Value, rightDouble.Value))
            Sink.Write(GuidanceInfo.ConstructForAssertWrite(methodType, idIsTheMessage, SetGuidanceData(null)));

        JsonObject? SetGuidanceData(JsonObject? json)
        {
            if (converted)
            {
                json ??= new();

                json["left"] = leftDouble.Value;
                json["right"] = rightDouble.Value;
            }

            return json;
        }
    }

    #endregion

    #region  Boolean Guidance

    /// <summary>
    /// <c>AlwaysSome({ ["key1"] = bool1, ["key2"] = bool2 }, ...)</c> is similar to <c>Always(bool1 || bool2, ...)</c> but additionally provides
    /// Antithesis visibility into the keys and values of <c>conditions</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="BooleanGuidanceHelper"/>
    /// <seealso cref="Always"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysSome(IReadOnlyDictionary<string, bool> conditions, string idIsTheMessage, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            BooleanGuidanceHelper(AssertionMethodType.AlwaysSome, values => values.Any(v => v), conditions, idIsTheMessage, details);
    }

    /// <summary>
    /// <c>SometimesAll({ ["key1"] = bool1, ["key2"] = bool2 }, ...)</c> is similar to <c>Sometimes(bool1 &amp;&amp; bool2, ...)</c> but additionally provides
    /// Antithesis visibility into the keys and values of <c>conditions</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="BooleanGuidanceHelper"/>
    /// <seealso cref="Sometimes"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesAll(IReadOnlyDictionary<string, bool> conditions, string idIsTheMessage, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            BooleanGuidanceHelper(AssertionMethodType.SometimesAll, values => !values.Any(v => !v), conditions, idIsTheMessage, details);
    }

    /// <param name="methodType">The AssertionMethodType of the caller.</param>
    /// <param name="operation">The operation to apply to conditions.Values.</param>
    /// <param name="conditions">The collection of conditions to-be evaluated, represented as a Dictionary of bools keyed by strings.</param>
    /// <param name="idIsTheMessage"><inheritdoc cref="NoGuidanceHelper" path="/param[@name='idIsTheMessage']"/></param>
    /// <param name="details"><inheritdoc cref="NoGuidanceHelper" path="/param[@name='details']"/></param>
    private static void BooleanGuidanceHelper(AssertionMethodType methodType, Func<IEnumerable<bool>, bool> operation,
        IReadOnlyDictionary<string, bool> conditions, string idIsTheMessage, JsonObject? details)
    {
        if (conditions == null)
            throw new ArgumentNullException(nameof(conditions));

        if (string.IsNullOrEmpty(idIsTheMessage))
            throw new ArgumentNullException(nameof(idIsTheMessage));

        bool condition = operation(conditions.Values);

        if (AssertionTracker.ShouldWrite(idIsTheMessage, condition))
            Sink.Write(AssertionInfo.ConstructForAssertWrite(methodType, idIsTheMessage, condition, SetStackTrace(condition, SetGuidanceData(details))));

        if (GuidanceTracker.ShouldBooleanWrite())
            Sink.Write(GuidanceInfo.ConstructForAssertWrite(methodType, idIsTheMessage, SetGuidanceData(null)));

        JsonObject? SetGuidanceData(JsonObject? json)
        {
            if (conditions.Count > 0)
            {
                json ??= new();

                foreach (var kvp in conditions)
                    json[kvp.Key] = kvp.Value;
            }

            return json;
        }
    }

    #endregion

    private static JsonObject? SetStackTrace(bool condition, JsonObject? json)
    {
        // TODO : Revisit this for AssertType.Sometimes and Reachability.
        //
        // Do not call the potentially expensive Environment.StackTrace for passing Assertions; however,
        // since AssertionTracker.ShouldWrite only returns true for the first pass and the first fail, this
        // is probably a premature optimization.
        if (condition)
            return json;

        json ??= new();

        json["stack_trace"] = Environment.StackTrace;

        return json;
    }
}