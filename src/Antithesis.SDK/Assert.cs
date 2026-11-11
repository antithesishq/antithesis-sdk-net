namespace Antithesis.SDK;

using System.Text.Json.Nodes;

// LOAD BEARING : typeof(Assert).FullName and the "string message" Parameters' names are load bearing for the CatalogGenerator.

/// <summary>
/// The Assert class enables defining <a href="https://antithesis.com/docs/properties_assertions/properties/">test properties</a>
/// about your program or <a href="https://antithesis.com/docs/test_templates/first_test/">workload</a>.
/// <br/><br/>
/// Each static method in this class takes a parameter called <c>message</c>, which is a string literal identifier used to aggregate assertions.
/// Antithesis generates one test property per unique <c>message</c>. This test property will be named <c>message</c>
/// in the <a href="https://antithesis.com/docs/reports/">triage report</a>.
/// <br/><br/>
/// Each static method also takes a parameter called <c>details</c>, which is an optional <c>JsonObject</c> reference of additional information provided
/// by the caller to add context to assertion passes and failures. The information that is logged will appear in the <c>logs</c> section of a
/// <a href="https://antithesis.com/docs/reports/">triage report</a>.
/// Null omits details; an explicit empty object is preserved. Argument expressions are evaluated on every call, even when the assertion does not emit.
/// </summary>
/// <remarks>If you include opaque CLR objects in your assertion details, the SDK may be unable to detect unsupported output from their serializers and emit the corresponding diagnostics.</remarks>
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
    public static void Always(bool condition, string message, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            NoGuidanceHelper(AssertionMethodType.Always, condition, message, details);
    }

    /// <summary>
    /// Assert that <c>condition</c> is true every time this method is called. The corresponding test property will pass if the assertion is never encountered.
    /// <br/><br/>
    /// The corresponding test property will be viewable in the <c>Antithesis SDK: Always assertions</c> group of your triage report.
    /// </summary>
    /// <inheritdoc cref="NoGuidanceHelper"/>
    public static void AlwaysOrUnreachable(bool condition, string message, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            NoGuidanceHelper(AssertionMethodType.AlwaysOrUnreachable, condition, message, details);
    }

    /// <summary>
    /// Assert that <c>condition</c> is true at least one time that this method was called. The corresponding test property will fail if the assertion is never encountered.
    /// <br/><br/>
    /// This test property will be viewable in the <c>Antithesis SDK: Sometimes assertions</c> group of your triage report.
    /// </summary>
    /// <inheritdoc cref="NoGuidanceHelper"/>
    public static void Sometimes(bool condition, string message, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            NoGuidanceHelper(AssertionMethodType.Sometimes, condition, message, details);
    }

    /// <summary>
    /// Assert that a line of code is never reached. The corresponding test property will pass if and only if the assertion is never encountered.
    /// <br/><br/>
    /// This test property will be viewable in the <c>Antithesis SDK: Reachability assertions</c> group of your triage report.
    /// </summary>
    /// <inheritdoc cref="NoGuidanceHelper"/>
    public static void Unreachable(string message, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            NoGuidanceHelper(AssertionMethodType.Unreachable, false, message, details);
    }

    /// <summary>
    /// Assert that a line of code is reached at least once. The corresponding test property will fail if the assertion is never encountered.
    /// <br/><br/>
    /// This test property will be viewable in the <c>Antithesis SDK: Reachability assertions</c> group of your triage report.
    /// </summary>
    /// <inheritdoc cref="NoGuidanceHelper"/>
    public static void Reachable(string message, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            NoGuidanceHelper(AssertionMethodType.Reachable, true, message, details);
    }

    /// <param name="methodType">The AssertionMethodType of the caller.</param>
    /// <param name="condition">The condition being asserted.</param>
    /// <param name="message">
    ///     A unique string identifier of the assertion. Provides context for assertion passes and failures and is intended to be human-readable.
    ///     <b><i>Must be provided as a string literal or as a reference to a publicly accessible const field.</i></b>
    /// </param>
    /// <param name="details">Optional additional details. Null omits details; an empty object is preserved.
    /// Serialization failures produce an antithesis_error record and the assertion still emits without details.</param>
    private static void NoGuidanceHelper(AssertionMethodType methodType, bool condition, string message, JsonObject? details)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException(nameof(message));

        if (AssertionTracker.ShouldWrite(message, condition))
            Sink.Write(AssertionInfo.ConstructForAssertWrite(methodType, message, condition, details));
    }

    #endregion

    #region Raw

    /// <summary>
    /// A low-level method designed for use by third-party frameworks that produce Antithesis-style
    /// assertions with caller-supplied catalog metadata; regular users should call the assertion
    /// methods above instead.
    /// <br/><br/>
    /// Catalog entries (<c>hit</c> = false) are always written without details. Hit entries are written only for
    /// the first passing and the first failing call per <c>id</c>.
    /// </summary>
    /// <param name="assertType">One of <c>"always"</c>, <c>"sometimes"</c>, or <c>"reachability"</c>.</param>
    /// <param name="displayType">One of <c>"Always"</c>, <c>"AlwaysOrUnreachable"</c>, <c>"Sometimes"</c>, <c>"Reachable"</c>, or <c>"Unreachable"</c>.</param>
    /// <param name="className">The name of the class containing the assertion.</param>
    /// <param name="functionName">The name of the method containing the assertion.</param>
    /// <param name="filePath">The path of the file containing the assertion.</param>
    /// <param name="beginLine">The 1-indexed line number where the assertion begins.</param>
    /// <param name="beginColumn">The 1-indexed column number where the assertion begins.</param>
    /// <param name="id">The unique identifier of the assertion, used to aggregate assertions and gate emission.</param>
    /// <param name="condition">The condition being asserted.</param>
    /// <param name="message">The human-readable name of the corresponding test property.</param>
    /// <param name="details">Optional additional details. Null omits details; an empty object is preserved.
    /// Serialization failures produce an antithesis_error record and the assertion still emits without details.</param>
    /// <param name="hit">Whether the assertion was evaluated (<c>true</c>) or is being cataloged (<c>false</c>).</param>
    /// <param name="mustHit">Whether the corresponding test property fails if the assertion is never encountered.</param>
    /// <param name="guidanceDetails">The <c>{left, right}</c> or named-bool pair a rich assertion derives
    /// from its operands, overlaid on <c>details</c> exactly as the rich assertions overlay it — generated
    /// fields override user-supplied fields of the same name. Null for a plain assertion.</param>
    public static void Raw(
        string assertType,
        string displayType,
        string className,
        string functionName,
        string filePath,
        int beginLine,
        int beginColumn,
        string id,
        bool condition,
        string message,
        JsonObject? details,
        bool hit,
        bool mustHit,
        JsonObject? guidanceDetails = null)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        if (Sink.IsNoop)
            return;

        if (hit && !AssertionTracker.ShouldWrite(id, condition))
            return;

        object? emitted = guidanceDetails is null
            ? details
            : details is null ? guidanceDetails : new DetailsOverlay(details, guidanceDetails);

        Sink.Write(AssertionInfo.ConstructForRawWrite(
            assertType, displayType, id, message,
            RawLocation(className, functionName, filePath, beginLine, beginColumn),
            mustHit, hit, condition, emitted));
    }

    /// <summary>
    /// A low-level method designed for use by third-party frameworks that produce Antithesis-style
    /// numeric guidance with caller-supplied catalog metadata; regular users should call the numeric
    /// guidance methods below instead.
    /// </summary>
    /// <param name="maximize">Whether a larger <c>left - right</c> is the improvement being sought.
    /// Fixed by the first call per <c>id</c>.</param>
    /// <param name="left">The left operand of the comparison.</param>
    /// <param name="right">The right operand of the comparison.</param>
    /// <param name="className"><inheritdoc cref="Raw" path="/param[@name='className']"/></param>
    /// <param name="functionName"><inheritdoc cref="Raw" path="/param[@name='functionName']"/></param>
    /// <param name="filePath"><inheritdoc cref="Raw" path="/param[@name='filePath']"/></param>
    /// <param name="beginLine"><inheritdoc cref="Raw" path="/param[@name='beginLine']"/></param>
    /// <param name="beginColumn"><inheritdoc cref="Raw" path="/param[@name='beginColumn']"/></param>
    /// <param name="id">The unique identifier of the guidance, used to key the tracked extremum.</param>
    /// <param name="message"><inheritdoc cref="Raw" path="/param[@name='message']"/></param>
    /// <param name="hit">Whether the guidance was evaluated (<c>true</c>) or is being cataloged (<c>false</c>).</param>
    public static void RawNumericGuidance<T>(
        bool maximize,
        T left,
        T right,
        string className,
        string functionName,
        string filePath,
        int beginLine,
        int beginColumn,
        string id,
        string message,
        bool hit)
        where T : struct, IConvertible
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        if (Sink.IsNoop)
            return;

        JsonObject? data = null;

        if (hit)
        {
            var leftDouble = TryConvertToDouble(left);
            var rightDouble = TryConvertToDouble(right);

            // An operand the runtime cannot express as a double yields no
            // guidance, as it does on the NumericGuidanceHelper path.
            if (!leftDouble.Success || !rightDouble.Success)
                return;

            if (!Sink.ShouldWriteNumericGuidance(maximize, id, leftDouble.Value, rightDouble.Value))
                return;

            data = new JsonObject
            {
                ["left"] = leftDouble.Value,
                ["right"] = rightDouble.Value
            };
        }

        Sink.Write(GuidanceInfo.ConstructForRawWrite(GuidanceType.Numeric, maximize, id, message,
            RawLocation(className, functionName, filePath, beginLine, beginColumn), hit, data));
    }

    /// <summary>
    /// A low-level method designed for use by third-party frameworks that produce Antithesis-style
    /// boolean guidance with caller-supplied catalog metadata; regular users should call
    /// <see cref="AlwaysSome"/> or <see cref="SometimesAll"/> instead.
    /// </summary>
    /// <param name="maximize">Whether every condition being true is the outcome being sought.</param>
    /// <param name="conditions">The conditions the guidance describes, as a dictionary of bools keyed by strings.</param>
    /// <param name="className"><inheritdoc cref="Raw" path="/param[@name='className']"/></param>
    /// <param name="functionName"><inheritdoc cref="Raw" path="/param[@name='functionName']"/></param>
    /// <param name="filePath"><inheritdoc cref="Raw" path="/param[@name='filePath']"/></param>
    /// <param name="beginLine"><inheritdoc cref="Raw" path="/param[@name='beginLine']"/></param>
    /// <param name="beginColumn"><inheritdoc cref="Raw" path="/param[@name='beginColumn']"/></param>
    /// <param name="id">The unique identifier of the guidance.</param>
    /// <param name="message"><inheritdoc cref="Raw" path="/param[@name='message']"/></param>
    /// <param name="hit">Whether the guidance was evaluated (<c>true</c>) or is being cataloged (<c>false</c>).</param>
    public static void RawBooleanGuidance(
        bool maximize,
        IReadOnlyDictionary<string, bool> conditions,
        string className,
        string functionName,
        string filePath,
        int beginLine,
        int beginColumn,
        string id,
        string message,
        bool hit)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        if (Sink.IsNoop)
            return;

        JsonObject? data = null;

        if (hit)
        {
            if (conditions == null)
                throw new ArgumentNullException(nameof(conditions));

            // An empty set of conditions is still a payload: {}, not null.
            data = new JsonObject();
            foreach (var kvp in conditions)
                data[kvp.Key] = kvp.Value;
        }

        Sink.Write(GuidanceInfo.ConstructForRawWrite(GuidanceType.Boolean, maximize, id, message,
            RawLocation(className, functionName, filePath, beginLine, beginColumn), hit, data));
    }

    private static LocationInfo RawLocation(string className, string functionName, string filePath,
            int beginLine, int beginColumn) =>
        new()
        {
            ClassName = className,
            MethodName = functionName,
            FilePath = filePath,
            BeginLine = beginLine,
            BeginColumn = beginColumn
        };

    /// <summary>
    /// The operand as a double, and whether the runtime could express it as one. Shared by the
    /// numeric guidance methods and <see cref="RawNumericGuidance"/> so both measure the same gap.
    /// </summary>
    private static (bool Success, double Value) TryConvertToDouble<T>(T value)
        where T : IConvertible
    {
        try { return (true, value.ToDouble(null)); }
        catch (FormatException) { return (false, default); }
        catch (InvalidCastException) { return (false, default); }
        catch (OverflowException) { return (false, default); }
    }

    #endregion

    #region Numeric Guidance

    /// <summary>
    /// <c>AlwaysGreaterThan(left, right, ...)</c> is mostly equivalent to <c>Always(left &gt; right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Always"/>
    public static void AlwaysGreaterThan<T>(T left, T right, string message, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.AlwaysGreaterThan, compareTo => compareTo > 0,
                left, right, message, details);
        }
    }

    /// <summary>
    /// <c>AlwaysGreaterThanOrEqualTo(left, right, ...)</c> is mostly equivalent to <c>Always(left &gt;= right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Always"/>
    public static void AlwaysGreaterThanOrEqualTo<T>(T left, T right, string message, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.AlwaysGreaterThanOrEqualTo, compareTo => compareTo >= 0,
                left, right, message, details);
        }
    }

    /// <summary>
    /// <c>AlwaysLessThan(left, right, ...)</c> is mostly equivalent to <c>Always(left &lt; right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Always"/>
    public static void AlwaysLessThan<T>(T left, T right, string message, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.AlwaysLessThan, compareTo => compareTo < 0,
                left, right, message, details);
        }
    }

    /// <summary>
    /// <c>AlwaysLessThanOrEqualTo(left, right, ...)</c> is mostly equivalent to <c>Always(left &lt;= right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Always"/>
    public static void AlwaysLessThanOrEqualTo<T>(T left, T right, string message, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.AlwaysLessThanOrEqualTo, compareTo => compareTo <= 0,
                left, right, message, details);
        }
    }

    /// <summary>
    /// <c>SometimesGreaterThan(left, right, ...)</c> is mostly equivalent to <c>Sometimes(left &gt; right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Sometimes"/>
    public static void SometimesGreaterThan<T>(T left, T right, string message, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.SometimesGreaterThan, compareTo => compareTo > 0,
                left, right, message, details);
        }
    }

    /// <summary>
    /// <c>SometimesGreaterThanOrEqualTo(left, right, ...)</c> is mostly equivalent to <c>Sometimes(left &gt;= right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Sometimes"/>
    public static void SometimesGreaterThanOrEqualTo<T>(T left, T right, string message, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.SometimesGreaterThanOrEqualTo, compareTo => compareTo >= 0,
                left, right, message, details);
        }
    }

    /// <summary>
    /// <c>SometimesLessThan(left, right, ...)</c> is mostly equivalent to <c>Sometimes(left &lt; right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Sometimes"/>
    public static void SometimesLessThan<T>(T left, T right, string message, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.SometimesLessThan, compareTo => compareTo < 0,
                left, right, message, details);
        }
    }

    /// <summary>
    /// <c>SometimesLessThanOrEqualTo(left, right, ...)</c> is mostly equivalent to <c>Sometimes(left &lt;= right, ...)</c> but additionally
    /// provides Antithesis visibility into the value of <c>left</c> and <c>right</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="NumericGuidanceHelper"/>
    /// <seealso cref="Sometimes"/>
    public static void SometimesLessThanOrEqualTo<T>(T left, T right, string message, JsonObject? details = default)
        where T : struct, IComparable<T>, IConvertible
    {
        if (!Sink.IsNoop)
        {
            NumericGuidanceHelper(AssertionMethodType.SometimesLessThanOrEqualTo, compareTo => compareTo <= 0,
                left, right, message, details);
        }
    }

    /// <typeparam name="T">The numeric type that we are comparing.</typeparam>
    /// <param name="methodType">The AssertionMethodType of the caller.</param>
    /// <param name="compareToOperation">The operation to apply to the result of left.CompareTo(right).</param>
    /// <param name="left">The left operand of the comparison.</param>
    /// <param name="right">The right operand of the comparison.</param>
    /// <param name="message"><inheritdoc cref="NoGuidanceHelper" path="/param[@name='message']"/></param>
    /// <param name="details"><inheritdoc cref="NoGuidanceHelper" path="/param[@name='details']"/></param>
    private static void NumericGuidanceHelper<T>(AssertionMethodType methodType, Func<int, bool> compareToOperation,
            T left, T right, string message, JsonObject? details)
        where T : struct, IComparable<T>, IConvertible
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException(nameof(message));

        bool condition = compareToOperation(left.CompareTo(right));

        var leftDouble = TryConvertToDouble(left);
        var rightDouble = TryConvertToDouble(right);

        bool converted = leftDouble.Success && rightDouble.Success;
        JsonObject? guidance = null;

        if (AssertionTracker.ShouldWrite(message, condition))
            Sink.Write(AssertionInfo.ConstructForAssertWrite(methodType, message, condition, MergeGuidanceData(details)));

        if (converted && Sink.ShouldWriteNumericGuidance(methodType.GetGuidanceMaximize(), message, leftDouble.Value, rightDouble.Value))
            Sink.Write(GuidanceInfo.ConstructForAssertWrite(methodType, message, (JsonObject?)MergeGuidanceData(null)));

        object? MergeGuidanceData(JsonObject? json)
        {
            if (converted)
            {
                guidance ??= new JsonObject
                {
                    ["left"] = leftDouble.Value,
                    ["right"] = rightDouble.Value
                };
                return json is null ? guidance : new DetailsOverlay(json, guidance);
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
    public static void AlwaysSome(IReadOnlyDictionary<string, bool> conditions, string message, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            BooleanGuidanceHelper(AssertionMethodType.AlwaysSome, values => values.Any(v => v), conditions, message, details);
    }

    /// <summary>
    /// <c>SometimesAll({ ["key1"] = bool1, ["key2"] = bool2 }, ...)</c> is similar to <c>Sometimes(bool1 &amp;&amp; bool2, ...)</c> but additionally provides
    /// Antithesis visibility into the keys and values of <c>conditions</c> by merging them into the assertion's details.
    /// </summary>
    /// <inheritdoc cref="BooleanGuidanceHelper"/>
    /// <seealso cref="Sometimes"/>
    public static void SometimesAll(IReadOnlyDictionary<string, bool> conditions, string message, JsonObject? details = default)
    {
        if (!Sink.IsNoop)
            BooleanGuidanceHelper(AssertionMethodType.SometimesAll, values => values.All(v => v), conditions, message, details);
    }

    /// <param name="methodType">The AssertionMethodType of the caller.</param>
    /// <param name="operation">The operation to apply to conditions.Values.</param>
    /// <param name="conditions">The collection of conditions to-be evaluated, represented as a Dictionary of bools keyed by strings.</param>
    /// <param name="message"><inheritdoc cref="NoGuidanceHelper" path="/param[@name='message']"/></param>
    /// <param name="details"><inheritdoc cref="NoGuidanceHelper" path="/param[@name='details']"/></param>
    private static void BooleanGuidanceHelper(AssertionMethodType methodType, Func<IEnumerable<bool>, bool> operation,
        IReadOnlyDictionary<string, bool> conditions, string message, JsonObject? details)
    {
        if (conditions == null)
            throw new ArgumentNullException(nameof(conditions));

        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException(nameof(message));

        bool condition = operation(conditions.Values);
        JsonObject? guidance = null;

        if (AssertionTracker.ShouldWrite(message, condition))
            Sink.Write(AssertionInfo.ConstructForAssertWrite(methodType, message, condition, MergeGuidanceData(details)));

        if (GuidanceTracker.ShouldBooleanWrite())
            Sink.Write(GuidanceInfo.ConstructForAssertWrite(methodType, message, (JsonObject?)MergeGuidanceData(null)));

        object? MergeGuidanceData(JsonObject? json)
        {
            if (guidance is null)
            {
                guidance = new JsonObject();
                foreach (var kvp in conditions)
                    guidance[kvp.Key] = kvp.Value;
            }
            return json is null ? guidance : new DetailsOverlay(json, guidance);
        }
    }

    #endregion
}
