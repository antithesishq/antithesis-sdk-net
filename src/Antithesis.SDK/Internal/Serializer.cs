namespace Antithesis.SDK;

using System.Text.Json;

internal static class Serializer
{
    internal static string Serialize(object message) =>
        message as string ?? JsonSerializer.Serialize(message);
}