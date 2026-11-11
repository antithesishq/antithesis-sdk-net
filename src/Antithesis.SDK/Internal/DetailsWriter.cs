namespace Antithesis.SDK;

using System.Threading;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

internal static class DetailsWriter
{
    private static int nonFiniteReported;
    private static int unsafeIntegerReported;
    private static int nonFiniteOperandReported;

    internal static void ResetDiagnosticsForTesting()
    {
        Interlocked.Exchange(ref nonFiniteReported, 0);
        Interlocked.Exchange(ref unsafeIntegerReported, 0);
        Interlocked.Exchange(ref nonFiniteOperandReported, 0);
    }

    internal static void Write(IDetailsRecord message, ISink sink)
    {
        string? json;
        string? diagnostic = null;
        var warnings = Warnings.None;
        try
        {
            json = JsonSerializer.Serialize(message, message.GetType());
            warnings = message.Details?.Found ?? Warnings.None;
        }
        catch (Exception error) when (error is not OutOfMemoryException)
        {
            // A caller's JsonValue converter can throw exceptions beyond JsonException.
            // Keep sink writes outside this catch so transport failures are not mislabeled.
            diagnostic = Diagnostic("antithesis_error", 6001,
                $".NET SDK failed to serialize details for \"{message.Context}\": {error.Message}", message.Context);
            json = message.TryOmitDetails()
                ? JsonSerializer.Serialize(message, message.GetType())
                : null;
        }
        if (diagnostic is not null)
            sink.Write(diagnostic);
        else
        {
            if ((warnings & Warnings.NonFinite) != 0 && TryClaim(ref nonFiniteReported))
                sink.Write(Diagnostic("antithesis_warning", 6002,
                    $".NET SDK replaced non-finite numbers in details for \"{message.Context}\" with the strings \"NaN\", \"Infinity\" or \"-Infinity\"; later occurrences in this process are not reported",
                    message.Context));
            if ((warnings & Warnings.UnsafeInteger) != 0 && TryClaim(ref unsafeIntegerReported))
                sink.Write(Diagnostic("antithesis_warning", 6003,
                    $".NET SDK emitted integers beyond ±2^53 in details for \"{message.Context}\"; consumers that parse JSON numbers as doubles will round them; later occurrences in this process are not reported",
                    message.Context));
        }
        if (json is not null)
            sink.Write(json);
    }

    // Checked ahead of the watermark so a non-finite hit can neither take it nor be compared against it.
    internal static bool ShouldWriteNumericGuidance(bool maximize, string id, double left, double right, ISink sink)
    {
        if (double.IsFinite(left) && double.IsFinite(right))
            return GuidanceTracker.ShouldNumericWrite(maximize, id, left, right);
        if (TryClaim(ref nonFiniteOperandReported))
            sink.Write(Diagnostic("antithesis_warning", 6004,
                $".NET SDK skipped numeric guidance for \"{id}\" because an operand was not finite; later occurrences in this process are not reported",
                id));
        return false;
    }

    // Claims a once-per-process warning for this caller; the plain read spares later emissions the locked instruction.
    private static bool TryClaim(ref int reported) =>
        Volatile.Read(ref reported) == 0 && Interlocked.CompareExchange(ref reported, 1, 0) == 0;

    private static string Diagnostic(string key, int code, string message, string context) =>
        JsonSerializer.Serialize(new Dictionary<string, object> { [key] = new { code, message, context } });

    [Flags]
    internal enum Warnings { None = 0, NonFinite = 1, UnsafeInteger = 2 }

    // Beyond this, consumers that parse JSON numbers as doubles (JavaScript's safe integer range) round them.
    private const long MaxSafeInteger = (1L << 53) - 1;

    // The runtime types System.Text.Json gives leaves, so telling a leaf's kind is one reference comparison
    // rather than a type probe per candidate. Parsed text keeps its JsonElement; anything else holds a CLR value.
    private static readonly Type ElementLeaf = JsonNode.Parse("0")!.GetType();
    private static readonly Type DoubleLeaf = JsonValue.Create(0.0).GetType();
    private static readonly Type FloatLeaf = JsonValue.Create(0f).GetType();
    private static readonly Type LongLeaf = JsonValue.Create(0L).GetType();
    private static readonly Type ULongLeaf = JsonValue.Create(0UL).GetType();
    private static readonly Type IntLeaf = JsonValue.Create(0).GetType();
    private static readonly Type StringLeaf = JsonValue.Create("")!.GetType();
    private static readonly Type BoolLeaf = JsonValue.Create(false).GetType();

    // Writes the details leaf by leaf so each number is inspected as it goes, borrowing the caller's nodes
    // rather than reparenting them.
    internal static void WriteDetails(Utf8JsonWriter writer, object details, JsonSerializerOptions options, ref Warnings warnings)
    {
        switch (details)
        {
            case DetailsOverlay overlay:
                writer.WriteStartObject();
                foreach (var entry in overlay.Entries)
                {
                    writer.WritePropertyName(entry.Key);
                    WriteNode(writer, entry.Value, options, ref warnings);
                }
                writer.WriteEndObject();
                break;
            case JsonNode node:
                WriteNode(writer, node, options, ref warnings);
                break;
            default:
                JsonSerializer.Serialize(writer, details, details.GetType(), options);
                break;
        }
    }

    // Dispatches on the runtime type first: leaves are most of a tree, and matching them as JsonValue would walk the
    // type hierarchy on each. Only the numeric leaf types are read (GetValue<T> is a generic virtual call and not
    // cheap); every other leaf is written by its own converter, as the default serialization would.
    private static void WriteNode(Utf8JsonWriter writer, JsonNode? node, JsonSerializerOptions options, ref Warnings warnings)
    {
        if (node is null)
        {
            writer.WriteNullValue();
            return;
        }
        Type type = node.GetType();
        if (type == StringLeaf || type == IntLeaf || type == BoolLeaf)
            node.WriteTo(writer, options);
        else if (type == DoubleLeaf)
        {
            double d = node.GetValue<double>();
            if (!WriteNonFinite(writer, d, ref warnings))
                writer.WriteNumberValue(d);
        }
        else if (type == LongLeaf)
        {
            long n = node.GetValue<long>();
            if (!IsSafe(n))
                warnings |= Warnings.UnsafeInteger;
            writer.WriteNumberValue(n);
        }
        else if (type == ElementLeaf)
        {
            JsonElement element = node.GetValue<JsonElement>();
            if (!IsSafe(element))
                warnings |= Warnings.UnsafeInteger;
            element.WriteTo(writer);
        }
        else if (type == FloatLeaf)
        {
            float f = node.GetValue<float>();
            if (!WriteNonFinite(writer, f, ref warnings))
                writer.WriteNumberValue(f);
        }
        else if (type == ULongLeaf)
        {
            ulong u = node.GetValue<ulong>();
            if (u > (ulong)MaxSafeInteger)
                warnings |= Warnings.UnsafeInteger;
            writer.WriteNumberValue(u);
        }
        else if (node is JsonObject obj)
        {
            writer.WriteStartObject();
            // Positional access; enumerating would box an enumerator per object.
            for (int i = 0, count = obj.Count; i < count; i++)
            {
                var entry = obj.GetAt(i);
                writer.WritePropertyName(entry.Key);
                WriteNode(writer, entry.Value, options, ref warnings);
            }
            writer.WriteEndObject();
        }
        else if (node is JsonArray array)
        {
            writer.WriteStartArray();
            for (int i = 0, count = array.Count; i < count; i++)
                WriteNode(writer, array[i], options, ref warnings);
            writer.WriteEndArray();
        }
        else if (node is JsonValue value)
        {
            // Some other CLR type. The probes test the held value's type and never run its converter.
            if (value.TryGetValue(out double d))
            {
                if (WriteNonFinite(writer, d, ref warnings))
                    return;
            }
            else if (value.TryGetValue(out float f))
            {
                if (WriteNonFinite(writer, f, ref warnings))
                    return;
            }
            else if (value.TryGetValue(out long n))
            {
                if (!IsSafe(n))
                    warnings |= Warnings.UnsafeInteger;
            }
            else if (value.TryGetValue(out ulong u))
            {
                if (u > (ulong)MaxSafeInteger)
                    warnings |= Warnings.UnsafeInteger;
            }
            else if (value.TryGetValue(out decimal m))
            {
                // Scale-zero decimals write integer tokens, just like parsed integer leaves.
                if ((decimal.GetBits(m)[3] & 0x00ff0000) == 0 && (m < -MaxSafeInteger || m > MaxSafeInteger))
                    warnings |= Warnings.UnsafeInteger;
            }
            else if (value.TryGetValue(out JsonElement element))
            {
                if (!IsSafe(element))
                    warnings |= Warnings.UnsafeInteger;
            }
            value.WriteTo(writer, options);
        }
        else
            node.WriteTo(writer, options);
    }

    // A non-finite number is written as its name; returns whether it was.
    private static bool WriteNonFinite(Utf8JsonWriter writer, double d, ref Warnings warnings)
    {
        if (double.IsFinite(d))
            return false;
        warnings |= Warnings.NonFinite;
        writer.WriteStringValue(double.IsNaN(d) ? "NaN" : d > 0 ? "Infinity" : "-Infinity");
        return true;
    }

    private static bool IsSafe(long n) => -MaxSafeInteger <= n && n <= MaxSafeInteger;

    // Parsed text is written back verbatim, so only integer text can leave the safe range. Text TryGetInt64 rejects
    // is float text when its magnitude is below 2^53 (integer text that small parses), otherwise either float text
    // or an integer beyond the long range; only that rare case needs the text itself.
    private static bool IsSafe(JsonElement number)
    {
        if (number.ValueKind != JsonValueKind.Number)
            return true;
        if (number.TryGetInt64(out long n))
            return IsSafe(n);
        if (number.TryGetDouble(out double d) && Math.Abs(d) <= MaxSafeInteger)
            return true;
        return number.GetRawText().AsSpan().IndexOfAny('.', 'e', 'E') >= 0;
    }
}

// Details as the serializer sees them: written through DetailsWriter so numbers are inspected as they are written,
// with the findings read back after serialization. Records hold their details as this type so the serializer binds
// the converter to the property rather than looking it up from the value's runtime type on every emission.
[JsonConverter(typeof(InspectedDetailsConverter))]
internal sealed class InspectedDetails(object details)
{
    internal static InspectedDetails? From(object? details) => details is null ? null : new(details);

    internal object Details => details;
    internal DetailsWriter.Warnings Found;
}

internal sealed class InspectedDetailsConverter : JsonConverter<InspectedDetails>
{
    public override InspectedDetails Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException();

    public override void Write(Utf8JsonWriter writer, InspectedDetails value, JsonSerializerOptions options)
    {
        var warnings = DetailsWriter.Warnings.None;
        DetailsWriter.WriteDetails(writer, value.Details, options, ref warnings);
        value.Found = warnings;
    }
}
