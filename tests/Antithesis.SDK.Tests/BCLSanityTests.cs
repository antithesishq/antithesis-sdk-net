namespace Antithesis.SDK;

using System.Globalization;
using System.IO;

public class BCLSanityTests
{
    [Fact]
    public void DoubleConstsOperators()
    {
        double nan = double.NaN;
        double positiveInfinity = double.PositiveInfinity;
        double negativeInfinity = double.NegativeInfinity;

        XAssert.False(nan == double.NaN);
        XAssert.True(positiveInfinity == double.PositiveInfinity);
        XAssert.True(negativeInfinity == double.NegativeInfinity);

        XAssert.True(double.PositiveInfinity > double.MaxValue);
        XAssert.True(double.NegativeInfinity < double.MinValue);
    }

    [Fact]
    public void DoubleConstsToString()
    {
        XAssert.Equal("NaN", double.NaN.ToString(CultureInfo.InvariantCulture));
        XAssert.Equal("Infinity", double.PositiveInfinity.ToString(CultureInfo.InvariantCulture));
        XAssert.Equal("-Infinity", double.NegativeInfinity.ToString(CultureInfo.InvariantCulture));
    }

    [Fact]
    public void PathGetDirectoryName()
    {
        XAssert.Equal("/foo/bar", Path.GetDirectoryName("/foo/bar/"));
        XAssert.Equal("/foo", Path.GetDirectoryName("/foo/bar"));
    }
}