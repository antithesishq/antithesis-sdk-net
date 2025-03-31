namespace Antithesis.SDK;

using System.Text.Json.Serialization;

internal class VersionInfoWrapper
{
    internal static VersionInfoWrapper Singleton { get; } = new();

    [JsonPropertyName("antithesis_sdk")]
    public VersionInfo Version { get; } = new();
}

internal class VersionInfo
{
    [JsonPropertyName("language")]
    public LanguageInfo Language { get; } = new();

    [JsonPropertyName("sdk_version")]
    public string SDKVersion { get; } = ThisAssembly.AssemblyInformationalVersion;

    [JsonPropertyName("protocol_version")]
    public string ProtocolVersion { get; } = "1.1.0";
}

internal class LanguageInfo
{
    [JsonPropertyName("name")]
    public string Name { get; } = "C#";

    [JsonPropertyName("version")]
    public string Version { get; } = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
}