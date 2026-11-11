namespace Antithesis.SDK;

using System.Text.Json.Nodes;

// Borrow both objects: JsonNode ownership forbids attaching their children to a merged tree.
// DetailsWriter writes the merged entries.
internal sealed class DetailsOverlay(JsonObject? details, JsonObject guidance)
{
    // Guidance operands shadow same-named caller details.
    internal IEnumerable<KeyValuePair<string, JsonNode?>> Entries =>
        details is null ? guidance : details.Where(entry => !guidance.ContainsKey(entry.Key)).Concat(guidance);
}
