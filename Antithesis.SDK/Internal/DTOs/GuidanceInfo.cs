namespace Antithesis.SDK;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

internal class GuidanceInfoWrapper(GuidanceInfo guidance) 
{
    [JsonPropertyName("antithesis_guidance")]
    public GuidanceInfo Guidance => guidance;
}

internal class GuidanceInfo
{
    internal static GuidanceInfoWrapper ConstructForCatalogWrite(AssertionVerboseType verboseType, string idIsTheMessage, LocationInfo locationInfo) =>
        new(
            new(verboseType, idIsTheMessage, LocationTracker.GetOrAdd(idIsTheMessage, locationInfo))
            {
                Hit = false,
            });

    internal static GuidanceInfoWrapper ConstructForAssertWrite(AssertionVerboseType verboseType, string idIsTheMessage, JsonObject? data) =>
        new(
            new(verboseType, idIsTheMessage, LocationTracker.GetOrAdd(idIsTheMessage, LocationInfo.Unknown))
            {
                Hit = true,
                Data = data
            });

    private GuidanceInfo(AssertionVerboseType verboseType, string idIsTheMessage, LocationInfo locationInfo)
    {
        GuidanceType = verboseType.GetGuidanceType();
        GuidanceMaximize = verboseType.GetGuidanceMaximize();
        Id = idIsTheMessage;
        Message = idIsTheMessage;
        Location = locationInfo;
    }

    // JsonStringEnumConverter<T> is only available in .NET 8+ (or by adding a dependency), and JsonStringEnumConverter (non-generic)
    // won't allow us to specify a different naming policy for different enums. Keep it simple: we're adding properties
    // that perform our desired enum to string conversions in the few places that we need them.

    [JsonPropertyName("guidance_type")]
    public string GuidanceTypeConverted => GuidanceType.ToString().ToLowerInvariant();
    public GuidanceType GuidanceType { get; }

    [JsonPropertyName("maximize")]
    public bool GuidanceMaximize { get; }
    
    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("message")]
    public string Message { get; }
    
    [JsonPropertyName("location")]
    public LocationInfo Location { get; }

    [JsonPropertyName("hit")]
    public bool Hit { get; init; }

    [JsonPropertyName("guidance_data")]
    public JsonObject? Data { get; init; }
}