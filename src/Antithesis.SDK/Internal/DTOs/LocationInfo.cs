namespace Antithesis.SDK;

using System.Text.Json.Serialization;

// LOAD BEARING : All public property names in this class are load bearing for the CatalogGenerator.

/// <summary>
/// An init-only data transfer object for metadata related to the source code location of an assertion.
/// If your project is referencing the Antithesis.SDK.SourceGenerator package, there is no need for you to construct this class directly.
/// </summary>
/// <remarks>
/// Default values are copied from the Antithesis Java SDK.
/// </remarks>
public class LocationInfo
{
    internal static LocationInfo Unknown { get; } = new();

    /// <summary>
    /// The name of the class containing the assertion.
    /// </summary>
    [JsonPropertyName("class")]
    public string ClassName { get; init; } = "class";

    /// <summary>
    /// The name of the method containing the assertion.
    /// </summary>
    [JsonPropertyName("function")]
    public string MethodName { get; init; } = "function";

    /// <summary>
    /// The path of the file containing the assertion. Antithesis.SDK.SourceGenerator attempts to set this to the solution-relative
    /// file path; however, there are circumstances when this will be set to the absolute file path.
    /// </summary>
    [JsonPropertyName("file")]
    public string FilePath { get; init; } = "file";

    /// <summary>
    /// The 1-indexed line number where the assertion begins.
    /// </summary>
    [JsonPropertyName("begin_line")]
    public int BeginLine { get; init; }

    /// <summary>
    /// The 1-indexed column number where the assertion begins.
    /// </summary>
    [JsonPropertyName("begin_column")]
    public int BeginColumn { get; init; }
}