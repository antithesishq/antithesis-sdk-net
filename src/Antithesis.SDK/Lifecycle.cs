namespace Antithesis.SDK;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

/// <summary>
/// The Lifecycle class contains methods used to inform Antithesis that particular test phases or milestones have been reached.
/// </summary>
/// <remarks>If you include opaque CLR objects in your assertion details, the SDK may be unable to detect unsupported output from their serializers and emit the corresponding diagnostics.</remarks>
public static class Lifecycle
{
    /// <summary>
    /// Determines if this code is executing within Antithesis according to the existence of an
    /// Antithesis environment variable as documented on our
    /// <a href="https://antithesis.com/docs/environment/the_antithesis_environment/#detecting-whether-you-are-running-within-antithesis">website</a>.
    /// </summary>
    public static bool IsAntithesis { get; } =
        !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(EnvironmentVariableNames.OutputDirectory));

    /// <summary>
    /// Indicates to Antithesis that setup has completed. Call this method when your system and workload are fully initialized.
    /// After this method is called, Antithesis will take a snapshot of your system and begin
    /// <a href="https://antithesis.com/docs/environment/fault_injection/">injecting faults</a>.
    /// Calling this method multiple times or from multiple processes will have no effect. Antithesis will treat the first time
    /// any process called this method as the moment that the setup was completed.
    /// </summary>
    /// <inheritdoc cref="SendEvent" path="/param"/>
    public static void SetupComplete(JsonObject? details = null)
    {
        if (!Sink.IsNoop)
            Sink.WriteDetails(SetupCompleteJson(details));
    }

    internal const string SetupCompleteJsonPropertyName = "antithesis_setup";

    internal static SetupInfoWrapper SetupCompleteJson(JsonObject? details = null) => new(new SetupInfo(details));

    /// <summary>
    /// Indicates to Antithesis that a certain event has been reached. It sends a structured log message to Antithesis
    /// that you may later use to aid debugging.
    /// </summary>
    /// <param name="name">The name of the event being logged.</param>
    /// <param name="details">Optional additional details, evaluated at runtime. Null means no details: setup omits the details key,
    /// and a user event has an empty object payload. An explicitly empty object is preserved. Serialization failures
    /// produce an antithesis_error record and the event is still emitted without the failed details.</param>
    public static void SendEvent(string? name, JsonObject? details = null)
    {
        if (!Sink.IsNoop)
            Sink.WriteDetails(SendEventJson(name, details));
    }

    internal static EventInfo SendEventJson(string? name, object? details = null) => new(name, details);

    internal sealed class SetupInfoWrapper(SetupInfo setup) : IDetailsRecord
    {
        [JsonPropertyName(SetupCompleteJsonPropertyName)]
        public SetupInfo Setup => setup;

        [JsonIgnore]
        public string Context => "setup_complete";
        [JsonIgnore]
        public InspectedDetails? Details => setup.Details;
        public bool TryOmitDetails()
        {
            setup.Details = null;
            return true;
        }
    }

    internal sealed class SetupInfo(JsonObject? details)
    {
        [JsonPropertyName("status")]
        public string Status => "complete";
        [JsonPropertyName("details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InspectedDetails? Details { get; internal set; } = InspectedDetails.From(details);
    }

    // A dictionary wraps a borrowed node without assigning its Parent.
    internal sealed class EventInfo : Dictionary<string, InspectedDetails>, IDetailsRecord
    {
        private readonly string _name;
        internal EventInfo(string? name, object? details)
        {
            _name = name ?? string.Empty;
            this[_name] = new InspectedDetails(details ?? new JsonObject());
        }

        public string Context => _name;
        public InspectedDetails? Details => this[_name];
        public bool TryOmitDetails()
        {
            this[_name] = new InspectedDetails(new JsonObject());
            return true;
        }
    }
}