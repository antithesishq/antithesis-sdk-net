namespace Antithesis.SDK;

using System.Collections.Concurrent;
using System.Threading;

internal static class AssertionTracker
{
    internal static bool ShouldWrite(string idIsTheMessage, bool condition) =>
        _passFailCountsByIdIsTheMessage.GetOrAdd(idIsTheMessage, _ => new PassFailCountTracker()).ShouldWrite(condition);

    private static readonly ConcurrentDictionary<string, PassFailCountTracker> _passFailCountsByIdIsTheMessage = new();

    private class PassFailCountTracker
    {
        private int _passCount;
        private int _failCount;

        // Only the first pass or fail is valuable to Antithesis. Any others cost performance and log space for no added value.
        internal bool ShouldWrite(bool condition) =>
            (condition
                ? Interlocked.Increment(ref _passCount)
                : Interlocked.Increment(ref _failCount))
            == 1;
    }
}