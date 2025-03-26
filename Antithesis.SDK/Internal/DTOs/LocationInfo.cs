namespace Antithesis.SDK;

using System.Text.Json.Serialization;

// LOAD BEARING : All public property names in this class are load bearing for the CatalogGenerator.
public class LocationInfo
{
    internal static LocationInfo Unknown { get; } = new();

    [JsonPropertyName("class")]
    public string ClassName { get; init; } = "class";

    [JsonPropertyName("function")]
    public string FunctionName { get; init; } = "function";

    [JsonPropertyName("file")]
    public string FilePath { get; init; } = "file";

    [JsonPropertyName("begin_line")]
    public int BeginLine { get; init; }

    [JsonPropertyName("begin_column")]
    public int BeginColumn { get; init; }
}