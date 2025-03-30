namespace Antithesis.SDK;

using System.Diagnostics;
using System.Text.Json.Nodes;

/// <summary>
/// The Lifecycle class contains methods used to inform Antithesis that particular test phases or milestones have been reached.
/// </summary>
public static class Lifecycle
{
    private const string OutputDirectoryEnvironmentVariableName = "ANTITHESIS_OUTPUT_DIR";

    /// <summary>
    /// Determines if this code is executing within Antithesis according to the existence of an
    /// Antithesis environment variable as documented on our
    /// <a href="https://antithesis.com/docs/environment/the_antithesis_environment/#detecting-whether-you-are-running-within-antithesis">website</a>.
    /// </summary>
    public static bool IsAntithesis { get; } = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(OutputDirectoryEnvironmentVariableName));

    /// <summary>
    /// Indicates to Antithesis that setup has completed. Call this method when your system and workload are fully initialized.
    /// After this method is called, Antithesis will take a snapshot of your system and begin
    /// <a href="https://antithesis.com/docs/applications/reliability/fault_injection/" target="_blank">injecting faults</a>.
    /// Calling this method multiple times or from multiple processes will have no effect. Antithesis will treat the first time
    /// any process called this method as the moment that the setup was completed.
    /// </summary>
    /// <inheritdoc cref="SendEvent" path="/param"/>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SetupComplete(JsonObject? details = null)
    {
        // Calling SendEvent here instead of Sink.Write'ing the full "antithesis_setup" JsonObject makes the code slightly more complicated;
        // however, it is important for demonstrating that "antithesis_setup" is simply a known / special event.
        if (!Sink.IsNoop)
            SendEvent(SetupCompleteJsonPropertyName, SetupCompleteJsonDetails(details));
    }

    internal const string SetupCompleteJsonPropertyName = "antithesis_setup";

    internal static JsonObject SetupCompleteJsonDetails(JsonObject? details = null) =>
        new()
        {
            ["status"] = "complete",
            ["details"] = details
        };

    /// <summary>
    /// Indicates to Antithesis that a certain event has been reached. It sends a structured log message to Antithesis
    /// that you may later use to aid debugging.
    /// </summary>
    /// <param name="name">The name of the event being logged.</param>
    /// <param name="details">Optional additional details to provide greater context for this event. Evaluated at runtime.</param>
    [Conditional(ConditionalCompilation.SymbolName)]
    public static void SendEvent(string? name, JsonObject? details = null)
    {
        if (!Sink.IsNoop)
            Sink.Write(SendEventJson(name, details));
    }

    internal static JsonObject SendEventJson(string? name, JsonObject? details = null) =>
        new() { [string.IsNullOrWhiteSpace(name) ? "anonymous" : name.Trim()] = details };
}