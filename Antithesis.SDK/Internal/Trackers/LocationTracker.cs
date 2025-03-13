namespace Antithesis.SDK;

using System.Collections.Concurrent;

internal static class LocationTracker
{
    internal static LocationInfo GetOrAdd(string idIsTheMessage, LocationInfo locationInfo) =>
        _locationsByIdIsTheMessage.GetOrAdd(idIsTheMessage, locationInfo);

    private static readonly ConcurrentDictionary<string, LocationInfo> _locationsByIdIsTheMessage = new();
}