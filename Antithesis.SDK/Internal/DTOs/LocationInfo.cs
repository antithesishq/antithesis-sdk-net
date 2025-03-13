namespace Antithesis.SDK;

using System.Text.Json.Serialization;

public class LocationInfo
{
    internal static LocationInfo Unknown { get; } = new();

    [JsonPropertyName("class")]
    public string ClassName { get; init; } = "class";

    [JsonPropertyName("function")]
    public string FunctionName { get; init; } = "function";

    [JsonPropertyName("file")]
    public string FileName { get; init; } = "file";

    [JsonPropertyName("begin_line")]
    public int BeginLine { get; init; }

    [JsonPropertyName("begin_column")]
    public int BeginColumn { get; init; }
}