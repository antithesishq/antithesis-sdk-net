namespace Antithesis.SDK;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

internal class GuidanceInfoWrapper(GuidanceInfo guidance) : IDetailsRecord
{
    [JsonPropertyName("antithesis_guidance")]
    public GuidanceInfo Guidance => guidance;

    [JsonIgnore]
    public string Context => guidance.Id;
    [JsonIgnore]
    public InspectedDetails? Details => guidance.Data;
    // A guidance hit without its data cannot be consumed by the fuzzer.
    public bool TryOmitDetails() => false;
}

internal class GuidanceInfo
{
    internal static GuidanceInfoWrapper ConstructForCatalogWrite(AssertionMethodType methodType, string idIsTheMessage, LocationInfo locationInfo) =>
        new(
            new(methodType, idIsTheMessage, LocationTracker.GetOrAdd(idIsTheMessage, locationInfo))
            {
                Hit = false,
                // A declaration describes a guidance point; it carries no data.
                Data = null
            });

    internal static GuidanceInfoWrapper ConstructForAssertWrite(AssertionMethodType methodType, string idIsTheMessage, JsonObject? data) =>
        new(
            new(methodType, idIsTheMessage, LocationTracker.GetOrAdd(idIsTheMessage, LocationInfo.Unknown))
            {
                Hit = true,
                Data = InspectedDetails.From(data)
            });

    internal static GuidanceInfoWrapper ConstructForRawWrite(GuidanceType guidanceType, bool maximize,
            string id, string message, LocationInfo locationInfo, bool hit, JsonObject? data) =>
        new(
            new(guidanceType, maximize, id, message, locationInfo)
            {
                Hit = hit,
                // A declaration describes a guidance point; it carries no data.
                Data = hit ? InspectedDetails.From(data) : null
            });

    private GuidanceInfo(AssertionMethodType methodType, string idIsTheMessage, LocationInfo locationInfo)
        : this(methodType.GetGuidanceType(), methodType.GetGuidanceMaximize(), idIsTheMessage, idIsTheMessage, locationInfo) { }

    private GuidanceInfo(GuidanceType guidanceType, bool maximize, string id, string message, LocationInfo locationInfo)
    {
        GuidanceType = guidanceType;
        GuidanceMaximize = maximize;
        Id = id;
        Message = message;
        Location = locationInfo;
    }

    // JsonStringEnumConverter<T> is only available in .NET 8+ (or by adding a dependency), and JsonStringEnumConverter (non-generic)
    // won't allow us to specify a different naming policy for different enums. Keep it simple: we're adding properties
    // that perform our desired enum to string conversions in the few places that we need them.

    [JsonPropertyName("guidance_type")]
    public string GuidanceTypeConverted => GuidanceType.ToString().ToLowerInvariant();
    [JsonIgnore]
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
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InspectedDetails? Data { get; init; }
}
