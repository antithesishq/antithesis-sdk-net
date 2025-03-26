namespace Antithesis.SDK;

using System.Diagnostics;
using System.Text.Json.Nodes;

public static class Lifecycle
{
    // https://antithesis.com/docs/environment/the_antithesis_environment/#detecting-whether-you-are-running-within-antithesis
    private const string OutputDirectoryEnvironmentVariableName = "ANTITHESIS_OUTPUT_DIR";
    public static bool IsAntithesis { get; } = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(OutputDirectoryEnvironmentVariableName));

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SetupComplete(JsonObject? details = null)
    {
        if (!Sink.IsNoop)
            Sink.Write(SetupCompleteJson(details));
    }

    internal static JsonObject SetupCompleteJson(JsonObject? details = null) =>
        new JsonObject()
        {
            ["antithesis_setup"] = new JsonObject()
            {
                ["status"] = "complete",
                ["details"] = details
            }
        };

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SendEvent(string? name, JsonObject? details = null)
    {
        if (!Sink.IsNoop)
            Sink.Write(SendEventJson(name, details));
    }

    internal static JsonObject SendEventJson(string? name, JsonObject? details = null) =>
        new JsonObject()
        {
            [string.IsNullOrWhiteSpace(name) ? "anonymous" : name.Trim()] = details
        };
}