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

    private AssertionInfo(AssertionMethodType methodType, string idIsTheMessage, LocationInfo locationInfo)
    {
        AssertType = methodType.GetAssertType();
        DisplayType = methodType.GetDisplayType();
        Id = idIsTheMessage;
        Message = idIsTheMessage;
        Location = locationInfo;
        MustHit = DisplayType.GetMustHit();
    }

    // JsonStringEnumConverter<T> is only available in .NET 8+ (or by adding a dependency), and JsonStringEnumConverter (non-generic)
    // won't allow us to specify a different naming policy for different enums. Keep it simple: we're adding properties
    // that perform our desired enum to string conversions in the few places that we need them.

    [JsonPropertyName("assert_type")]
    public string AssertTypeConverted => AssertType.ToString().ToLowerInvariant();
    public AssertionAssertType AssertType { get; }

    [JsonPropertyName("display_type")]
    public string DisplayTypeConverted => DisplayType.ToString();
    public AssertionDisplayType DisplayType { get; }
    
    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("message")]
    public string Message { get; }
    
    [JsonPropertyName("location")]
    public LocationInfo Location { get; }

    [JsonPropertyName("must_hit")]
    public bool MustHit { get; }

    [JsonPropertyName("hit")]
    public bool Hit { get; init; }    

    [JsonPropertyName("condition")]
    public bool Condition { get; init; }

    [JsonPropertyName("details")]
    public JsonObject? Details { get; init; }
}