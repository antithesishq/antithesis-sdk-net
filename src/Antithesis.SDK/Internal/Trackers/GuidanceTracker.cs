namespace Antithesis.SDK;

using System.Collections.Concurrent;

internal static class GuidanceTracker
{
    // Because boolean guidance is a collection of key value pairs, we always write it to Antithesis so it can decide what to pursue.
    internal static bool ShouldBooleanWrite() => true;

    internal static bool ShouldNumericWrite(bool maximize, string idIsTheMessage, double left, double right) =>
        _numericTrackersByIdIsTheMessage.GetOrAdd(idIsTheMessage, _ => new NumericTracker(maximize)).ShouldWrite(left, right);

    private static readonly ConcurrentDictionary<string, NumericTracker> _numericTrackersByIdIsTheMessage = new();
    
    private class NumericTracker
    {
        internal NumericTracker(bool maximize)
        {
            _maximize = maximize;
            _mark = _maximize ? double.NegativeInfinity : double.PositiveInfinity;
        }

        private readonly bool _maximize;
        private double _mark;

        // Numeric guidance is important to write as Antithesis "gets closer" to making an assertion potentially pass or fail.
        // The "antithesis_guidance/maximize" JSON property indicates which "direction" is "closer" to the flipping point.
        internal bool ShouldWrite(double left, double right)
        {
            double diff = left - right;

            if (_maximize && _mark > diff)
                return false;

            if (!_maximize && _mark < diff)
                return false;

            // Write NaN values, but don't let them update the mark.
            if (!double.IsNaN(diff))
                _mark = diff;

            return true;
        }
    }
}