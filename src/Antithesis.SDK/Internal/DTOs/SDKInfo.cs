namespace Antithesis.SDK;

using System.Text.Json.Serialization;

internal class SDKInfoWrapper
{
    internal SDKInfoWrapper(Type sinkType) => Info = new(sinkType);

    [JsonPropertyName("antithesis_sdk")]
    public SDKInfo Info { get; }
}

internal class SDKInfo
{
    internal SDKInfo(Type sinkType) => SinkTypeFullName = sinkType.FullName;

    [JsonPropertyName("language")]
    public LanguageInfo Language { get; } = new();

    [JsonPropertyName("sdk_version")]
    public string SDKVersion { get; } = ThisAssembly.AssemblyInformationalVersion;

    [JsonPropertyName("protocol_version")]
    public string ProtocolVersion { get; } = "1.1.0";

    [JsonPropertyName("sink_type")]
    public string? SinkTypeFullName { get; }
}

internal class LanguageInfo
{
    [JsonPropertyName("name")]
    public string Name { get; } = "C#";

    [JsonPropertyName("version")]
    public string Version { get; } = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
}