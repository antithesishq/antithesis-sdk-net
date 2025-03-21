namespace Antithesis.SDK;

using System.Diagnostics;
using System.Text.Json.Nodes;

// LOAD BEARING : typeof(Assert).FullName and the "string idIsTheMessage" Parameters' names are load bearing for the CatalogGenerator.
public static class Assert
{
    // No Guidance

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Always(bool condition, string idIsTheMessage, JsonObject? details = default) =>
        NoGuidanceHelper(AssertionMethodType.Always, condition, idIsTheMessage, details);

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysOrUnreachable(bool condition, string idIsTheMessage, JsonObject? details = default) =>
        NoGuidanceHelper(AssertionMethodType.AlwaysOrUnreachable, condition, idIsTheMessage, details);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Sometimes(bool condition, string idIsTheMessage, JsonObject? details = default) =>
        NoGuidanceHelper(AssertionMethodType.Sometimes, condition, idIsTheMessage, details);

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Unreachable(string idIsTheMessage, JsonObject? details = default) =>
        NoGuidanceHelper(AssertionMethodType.Unreachable, false, idIsTheMessage, details);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void Reachable(string idIsTheMessage, JsonObject? details = default) =>
        NoGuidanceHelper(AssertionMethodType.Reachable, true, idIsTheMessage, details);

    private static void NoGuidanceHelper(AssertionMethodType methodType, bool condition, string idIsTheMessage, JsonObject? details)
    {
        if (string.IsNullOrEmpty(idIsTheMessage))
            throw new ArgumentNullException(nameof(idIsTheMessage));

        if (AssertionTracker.ShouldWrite(idIsTheMessage, condition))
            Sink.Write(AssertionInfo.ConstructForAssertWrite(methodType, idIsTheMessage, condition, SetStackTrace(condition, details)));
    }

    // Numeric Guidance

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysGreaterThan<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
            where T : struct, IComparable, IConvertible =>
        NumericGuidanceHelper(AssertionMethodType.AlwaysGreaterThan, compareTo => compareTo > 0,
            left, right, idIsTheMessage, details);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysGreaterThanOrEqualTo<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
            where T : struct, IComparable, IConvertible =>
        NumericGuidanceHelper(AssertionMethodType.AlwaysGreaterThanOrEqualTo, compareTo => compareTo >= 0,
            left, right, idIsTheMessage, details);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysLessThan<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
            where T : struct, IComparable, IConvertible =>
        NumericGuidanceHelper(AssertionMethodType.AlwaysLessThan, compareTo => compareTo < 0,
            left, right, idIsTheMessage, details);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysLessThanOrEqualTo<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
            where T : struct, IComparable, IConvertible =>
        NumericGuidanceHelper(AssertionMethodType.AlwaysLessThanOrEqualTo, compareTo => compareTo <= 0,
            left, right, idIsTheMessage, details);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesGreaterThan<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
            where T : struct, IComparable, IConvertible =>
        NumericGuidanceHelper(AssertionMethodType.SometimesGreaterThan, compareTo => compareTo > 0,
            left, right, idIsTheMessage, details);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesGreaterThanOrEqualTo<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
            where T : struct, IComparable, IConvertible =>
        NumericGuidanceHelper(AssertionMethodType.SometimesGreaterThanOrEqualTo, compareTo => compareTo >= 0,
            left, right, idIsTheMessage, details);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesLessThan<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
            where T : struct, IComparable, IConvertible =>
        NumericGuidanceHelper(AssertionMethodType.SometimesLessThan, compareTo => compareTo < 0,
            left, right, idIsTheMessage, details);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesLessThanOrEqualTo<T>(T left, T right, string idIsTheMessage, JsonObject? details = default)
            where T : struct, IComparable, IConvertible =>
        NumericGuidanceHelper(AssertionMethodType.SometimesLessThanOrEqualTo, compareTo => compareTo <= 0,
            left, right, idIsTheMessage, details);

    private static void NumericGuidanceHelper<T>(AssertionMethodType methodType, Func<int, bool> compareToOperation,
            T left, T right, string idIsTheMessage, JsonObject? details)
        where T : struct, IComparable, IConvertible
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

    // Boolean Guidance

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void AlwaysSome(IReadOnlyDictionary<string, bool> conditions, string idIsTheMessage, JsonObject? details = default) =>
        BooleanGuidanceHelper(AssertionMethodType.AlwaysSome, values => values.Any(v => v), conditions, idIsTheMessage, details);
    
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SometimesAll(IReadOnlyDictionary<string, bool> conditions, string idIsTheMessage, JsonObject? details = default) =>
        BooleanGuidanceHelper(AssertionMethodType.SometimesAll, values => !values.Any(v => !v), conditions, idIsTheMessage, details);

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

    // Common

    private static JsonObject? SetStackTrace(bool condition, JsonObject? json)
    {
        // Do not call the potentially expensive Environment.StackTrace for passing Assertions.
        if (condition)
            return json;

        json ??= new();

        json["stack_trace"] = Environment.StackTrace;

        return json;
    }
}