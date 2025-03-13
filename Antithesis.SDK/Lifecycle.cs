namespace Antithesis.SDK;

using System.Diagnostics;
using System.Text.Json.Nodes;

public static class Lifecycle
{
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SetupComplete(JsonObject? details)
    {
        var json = new JsonObject()
        {
            ["antithesis_setup"] = new JsonObject()
            {
                ["status"] = "complete",
                ["details"] = details
            }
        };

        Sink.Write(json);
    }

    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SendEvent(string? name, JsonObject? details)
    {
        var json = new JsonObject()
        {
            [string.IsNullOrWhiteSpace(name) ? "anonymous" : name.Trim()] = details
        };

        Sink.Write(json);
    }
}