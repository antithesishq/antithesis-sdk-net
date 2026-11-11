namespace Antithesis.SDK;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

// Details behaviour the cross-SDK conformance harness cannot reach: JsonNode
// ownership, converter call counts and attached contracts, opaque CLR values,
// float leaves, guidance overlays, and transport failures. The wire shape and
// the diagnostics for parsed and CLR-typed input are its job.
[Collection("Process-wide diagnostics")]
public class DetailsTests
{
    public DetailsTests() => DetailsWriter.ResetDiagnosticsForTesting();

    [Fact]
    public void ParentedDetailsCanBeReusedWithoutChangingOwnership()
    {
        var details = new JsonObject { ["answer"] = 42 };
        var parent = new JsonObject { ["child"] = details };
        var sink = new CaptureSink();
        for (int i = 0; i < 2; i++)
        {
            DetailsWriter.Write(Lifecycle.SetupCompleteJson(details), sink);
            XAssert.Equal(42, sink.Last["antithesis_setup"]!["details"]!["answer"]!.GetValue<int>());
            DetailsWriter.Write(Lifecycle.SendEventJson("reuse", details), sink);
            XAssert.Equal(42, sink.Last["reuse"]!["answer"]!.GetValue<int>());
        }
        XAssert.Same(parent, details.Parent);
        XAssert.Same(details, parent["child"]);
    }

    [Fact]
    public void SuccessfulDetailsSerializeOnce()
    {
        var value = new CountedValue();
        var details = new JsonObject { ["value"] = JsonValue.Create(value) };
        var sink = new CaptureSink();
        DetailsWriter.Write(Assertion(details), sink);
        XAssert.Equal(1, value.Calls);
        XAssert.Equal(42, sink.Last["antithesis_assert"]!["details"]!["value"]!.GetValue<int>());
    }

    [Fact]
    public void NonFiniteSinglesAreNamedWithoutTouchingTheCallersNode()
    {
        var details = new JsonObject { ["value"] = float.NaN, ["array"] = new JsonArray(float.NegativeInfinity, 1.5) };
        var sink = new CaptureSink();
        DetailsWriter.Write(Assertion(details), sink);
        XAssert.Equal(6002, sink.Records[0]["antithesis_warning"]!["code"]!.GetValue<int>());
        XAssert.Equal("-Infinity", sink.Last["antithesis_assert"]!["details"]!["array"]![0]!.GetValue<string>());
        XAssert.True(float.IsNaN(details["value"]!.GetValue<float>()));
    }

    [Fact]
    public void FailedGuidanceSerializesOnceAndDropsItsPacket()
    {
        var sink = new CaptureSink();
        var value = new CountedValue { Fail = true };
        var data = new JsonObject { ["left"] = JsonValue.Create(value), ["right"] = 0 };
        DetailsWriter.Write(GuidanceInfo.ConstructForAssertWrite(AssertionMethodType.AlwaysGreaterThan,
                "numeric guidance", data), sink);
        XAssert.Equal(1, value.Calls);
        XAssert.Single(sink.Records);
        XAssert.Equal(6001, sink.Records[0]["antithesis_error"]!["code"]!.GetValue<int>());
        XAssert.Equal("numeric guidance", sink.Records[0]["antithesis_error"]!["context"]!.GetValue<string>());
        XAssert.Null(data.Parent);
    }

    [Fact]
    public void FailedEventDetailsFallBackToTheEmptyPayloadAndSetupOmitsThem()
    {
        var details = new JsonObject { ["value"] = JsonValue.Create(new CountedValue { Fail = true }) };
        var sink = new CaptureSink();
        DetailsWriter.Write(Lifecycle.SendEventJson("evt", details), sink);
        DetailsWriter.Write(Lifecycle.SetupCompleteJson(details), sink);
        XAssert.Equal(4, sink.Records.Count);
        XAssert.Equal("evt", sink.Records[0]["antithesis_error"]!["context"]!.GetValue<string>());
        XAssert.Empty(sink.Records[1]["evt"]!.AsObject());
        XAssert.Equal("setup_complete", sink.Records[2]["antithesis_error"]!["context"]!.GetValue<string>());
        XAssert.Equal("complete", sink.Records[3]["antithesis_setup"]!["status"]!.GetValue<string>());
        XAssert.False(sink.Records[3]["antithesis_setup"]!.AsObject().ContainsKey("details"));
        XAssert.Single(details);
        XAssert.Null(details.Parent);
    }

    [Fact]
    public void OverlayFailureSerializesOnceWithoutMutatingDetails()
    {
        var value = new CountedValue { Fail = true };
        var details = new JsonObject { ["value"] = JsonValue.Create(value) };
        var guidance = new JsonObject { ["left"] = 2, ["right"] = 1 };
        var sink = new CaptureSink();
        DetailsWriter.Write(Assertion(new DetailsOverlay(details, guidance)), sink);
        XAssert.Equal(1, value.Calls);
        XAssert.Single(details);
        XAssert.Null(details.Parent);
    }

    [Fact]
    public void GuidanceOverlayPreservesCallerDataAndSkipsOverriddenValues()
    {
        var overridden = new CountedValue { Fail = true };
        var nested = new JsonObject { ["child"] = 7 };
        var details = new JsonObject { ["left"] = JsonValue.Create(overridden), ["nested"] = nested };
        var guidance = new JsonObject { ["left"] = 2, ["right"] = 1 };
        var sink = new CaptureSink();
        DetailsWriter.Write(Assertion(new DetailsOverlay(details, guidance)), sink);
        XAssert.Single(sink.Records);
        JsonNode result = sink.Last["antithesis_assert"]!["details"]!;
        XAssert.Equal(2, result["left"]!.GetValue<int>());
        XAssert.Equal(1, result["right"]!.GetValue<int>());
        XAssert.Equal(7, result["nested"]!["child"]!.GetValue<int>());
        XAssert.Equal(0, overridden.Calls);
        XAssert.False(details.ContainsKey("right"));
        XAssert.Same(details, nested.Parent);
        XAssert.Null(guidance.Parent);
    }

    [Fact]
    public void TransportFailureIsNotReportedAsSerializationFailure()
    {
        var sink = new ThrowingSink();
        XAssert.Throws<InvalidOperationException>(() => DetailsWriter.Write(Assertion(null), sink));
        XAssert.Equal(1, sink.Calls);
    }

    [Fact]
    public void OpaqueNonFiniteGetterIsNotRetried()
    {
        var value = new ChangingValue();
        var sink = new CaptureSink();
        DetailsWriter.Write(Assertion(new JsonObject { ["normalized"] = double.NaN, ["value"] = JsonValue.Create(value) }), sink);
        XAssert.Equal(1, value.Calls);
        XAssert.False(sink.Last["antithesis_assert"]!.AsObject().ContainsKey("details"));
    }

    [Fact]
    public void OpaqueNumbersAreNotInspected()
    {
        var sink = new CaptureSink();
        var value = new { Floating = 9007199254740992.0, Integer = 9007199254740993L };
        DetailsWriter.Write(Assertion(new JsonObject { ["value"] = JsonValue.Create(value) }), sink);
        XAssert.Single(sink.Records);
        XAssert.Equal(9007199254740993L,
            sink.Last["antithesis_assert"]!["details"]!["value"]!["Integer"]!.GetValue<long>());
    }

    [Fact]
    public void OpaqueNumericValuesAreInspectedWhenPossible()
    {
        var sink = new CaptureSink();
        var details = new JsonObject
        {
            ["nonFinite"] = JsonValue.Create<object>(double.NaN),
            ["integer"] = JsonValue.Create<object>(9007199254740993L),
        };
        DetailsWriter.Write(Assertion(details), sink);
        XAssert.Equal(3, sink.Records.Count);
        XAssert.Equal(6002, sink.Records[0]["antithesis_warning"]!["code"]!.GetValue<int>());
        XAssert.Equal(6003, sink.Records[1]["antithesis_warning"]!["code"]!.GetValue<int>());
        XAssert.Equal("NaN", sink.Last["antithesis_assert"]!["details"]!["nonFinite"]!.GetValue<string>());
        XAssert.Equal(9007199254740993L, sink.Last["antithesis_assert"]!["details"]!["integer"]!.GetValue<long>());
    }

    private sealed class ChangingValue
    {
        internal int Calls;
        public double Value => ++Calls == 1 ? double.NaN : 42;
    }

    private static AssertionInfoWrapper Assertion(object? details) =>
        AssertionInfo.ConstructForAssertWrite(AssertionMethodType.Always, "assertion context", false, details);

    private sealed class CaptureSink : ISink
    {
        internal List<JsonNode> Records { get; } = new();
        internal JsonNode Last => Records[^1];
        public void Write(string message) => Records.Add(JsonNode.Parse(message)!);
    }

    private sealed class ThrowingSink : ISink
    {
        internal int Calls;
        public void Write(string message) { Calls++; throw new InvalidOperationException("transport failed"); }
    }

    [JsonConverter(typeof(CountedConverter))]
    private sealed class CountedValue
    {
        internal int Calls;
        internal bool Fail;
    }

    private sealed class CountedConverter : JsonConverter<CountedValue>
    {
        public CountedConverter() { }
        public override CountedValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            throw new NotSupportedException();
        public override void Write(Utf8JsonWriter writer, CountedValue value, JsonSerializerOptions options)
        {
            value.Calls++;
            if (value.Fail) throw new JsonException("deliberate failure");
            writer.WriteNumberValue(42);
        }
    }
}

// Resetting the process-wide warning flag must not race another test's emission.
[CollectionDefinition("Process-wide diagnostics", DisableParallelization = true)]
public class ProcessWideDiagnosticsCollection;
