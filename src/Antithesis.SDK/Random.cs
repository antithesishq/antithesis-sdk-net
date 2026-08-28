namespace Antithesis.SDK;

// This interface exists to better support mocking and testing.
internal interface IRandomUInt64Provider { ulong Next(); }

/// <summary>
/// The Random class sources entropy directly from the Antithesis platform when executing within
/// Antithesis, and from <see cref="System.Random">System.Random</see> otherwise. For a
/// <see cref="System.Random">System.Random</see>-compatible adapter, see <see cref="AntithesisRandom"/>.
/// </summary>
public static class Random
{
    // Force the Sink's static constructor to write out SDK info in case this is the first use of the SDK.
    static Random() => Sink.Touch();

    // The raw provider behind GetRandom: the FFI when executing within Antithesis, else System.Random.Shared.
    // This is similar behavior to AntithesisRandom.SharedFallbackToSystem.
    private static readonly IRandomUInt64Provider _provider =
        FFI.FileExists ? new FFIRandomUInt64Provider() : new SystemRandomUInt64Provider();

    private sealed class SystemRandomUInt64Provider : IRandomUInt64Provider
    {
        ulong IRandomUInt64Provider.Next()
        {
            Span<byte> bytes = stackalloc byte[sizeof(ulong)];
            System.Random.Shared.NextBytes(bytes);
            return BitConverter.ToUInt64(bytes);
        }
    }

    /// <summary>
    /// Returns a raw 64-bit value chosen by Antithesis. You should use this value immediately,
    /// rather than storing it for later: if the simulation branches after the value is received,
    /// every branch sees the same value, which defeats the purpose of branching. For the same
    /// reason, do not use it to seed a pseudo-random number generator.
    /// </summary>
    public static ulong GetRandom() => _provider.Next();

    /// <summary>
    /// Returns a randomly chosen item from a list of options. You should use this value
    /// immediately rather than storing it, for the reasons described on <see cref="GetRandom"/>.
    /// <br/><br/>
    /// This method is not purely for convenience: signaling to the Antithesis platform that you
    /// intend to use a random value in a structured way enables it to provide more interesting
    /// choices over time.
    /// </summary>
    public static T RandomChoice<T>(IReadOnlyList<T> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        if (items.Count == 0)
            throw new ArgumentException("The list of options must not be empty.", nameof(items));

        // A single choice consumes no randomness.
        if (items.Count == 1)
            return items[0];

        // Unbiased rejection sampling over the full 64-bit draw — the same
        // derivation every Antithesis SDK implements.
        ulong count = (ulong)items.Count;
        ulong ceiling = (ulong.MaxValue / count) * count;

        ulong value = GetRandom();
        while (value >= ceiling)
            value = GetRandom();

        return items[(int)(value % count)];
    }
}