namespace Antithesis.SDK;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

internal class AssertionInfoWrapper(AssertionInfo assertion)
{
    [JsonPropertyName("antithesis_assert")]
    public AssertionInfo Assertion => assertion;
}

internal class AssertionInfo
{
    internal static AssertionInfoWrapper ConstructForCatalogWrite(AssertionMethodType methodType, string idIsTheMessage, LocationInfo locationInfo) =>
        new(
            new(methodType, idIsTheMessage, LocationTracker.GetOrAdd(idIsTheMessage, locationInfo))
            {
                Hit = false,
                Condition = false
            });

    internal static AssertionInfoWrapper ConstructForAssertWrite(AssertionMethodType methodType, string idIsTheMessage, bool condition, JsonObject? details) =>
        new(
            new(methodType, idIsTheMessage, LocationTracker.GetOrAdd(idIsTheMessage, LocationInfo.Unknown))
            {
                Hit = true,
                Condition = condition,
                Details = details
            });

    internal static AssertionInfoWrapper ConstructForRawWrite(string assertType, string displayType,
            string id, string message, LocationInfo locationInfo, bool mustHit, bool hit, bool condition, JsonObject? details) =>
        new(
            new()
            {
                AssertTypeRaw = assertType,
                DisplayTypeRaw = displayType,
                Id = id,
                Message = message,
                Location = locationInfo,
                MustHit = mustHit,
                Hit = hit,
                Condition = condition,
                Details = details
            });

    private AssertionInfo(AssertionMethodType methodType, string idIsTheMessage, LocationInfo locationInfo)
    {
        AssertTypeRaw = methodType.GetAssertTypeRaw();
        DisplayTypeRaw = methodType.GetDisplayTypeRaw();
        Id = idIsTheMessage;
        Message = idIsTheMessage;
        Location = locationInfo;
        MustHit = methodType.GetDisplayType().GetMustHit();
    }

    private AssertionInfo() { }

    [JsonPropertyName("assert_type")]
    public string AssertTypeRaw { get; private init; } = null!;

    [JsonPropertyName("display_type")]
    public string DisplayTypeRaw { get; private init; } = null!;

    [JsonPropertyName("id")]
    public string Id { get; private init; } = null!;

    [JsonPropertyName("message")]
    public string Message { get; private init; } = null!;

    [JsonPropertyName("location")]
    public LocationInfo Location { get; private init;} = null!;

    [JsonPropertyName("must_hit")]
    public bool MustHit { get; private init;}

    [JsonPropertyName("hit")]
    public bool Hit { get; private init; }

    [JsonPropertyName("condition")]
    public bool Condition { get; private init; }

    [JsonPropertyName("details")]
    public JsonObject? Details { get; private init; }
}