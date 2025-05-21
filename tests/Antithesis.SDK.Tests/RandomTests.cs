namespace Antithesis.SDK;

using System.IO;
using Xunit;

internal class SystemRandomFixedSeedUInt64Provider : IRandomUInt64Provider
{
    private readonly System.Random _random = new(43);

    ulong IRandomUInt64Provider.Next()
    {
        // System.Random only provides NextInt64 which returns [0, long.MaxValue).
        byte[] ulongBytes = new byte[sizeof(ulong)];
        _random.NextBytes(ulongBytes);

        return BitConverter.ToUInt64(ulongBytes);
    }
}

public class RandomTests
{
    // Explicitly using Type.FullName for return Type clarity here.
    private static Antithesis.SDK.Random NewFixedSeedRandom() => new(new SystemRandomFixedSeedUInt64Provider());

    [Fact]
    public void Shared() => XAssert.Equal(System.Random.Shared, Random.SharedFallbackToSystem);

    // NextDouble should directly exercise our Sample override.
    [Fact]
    public void Sample()
    {
        var random = NewFixedSeedRandom();

        for (int i = 0; i < 10_000_000; i++)
        {
            double value = random.NextDouble();

            XAssert.InRange(value, 0.0, 1.0);
            XAssert.NotEqual(1.0, value);
        }
    }

    [Fact]
    public void Next()
    {
        var random = NewFixedSeedRandom();

        for (int i = 0; i < 10_000_000; i++)
            XAssert.InRange(random.Next(), 0, int.MaxValue - 1);
    }

    [Fact]
    public void NextRange()
    {
        var random = NewFixedSeedRandom();

        XAssert.Throws<ArgumentOutOfRangeException>(() => random.Next(-1, -2));
        XAssert.Throws<ArgumentOutOfRangeException>(() => random.Next(0, -1));
        XAssert.Throws<ArgumentOutOfRangeException>(() => random.Next(1, 0));
        XAssert.Throws<ArgumentOutOfRangeException>(() => random.Next(2, 1));

        for (int i = 0; i < 10_000_000; i++)
        {
            int bound1 = NextBound();
            int bound2 = NextBound();

            if (bound1 == bound2)
            {
                XAssert.Equal(bound1, random.Next(bound1, bound2));
                continue;
            }

            if (bound1 > bound2)
                (bound2, bound1) = (bound1, bound2);

            XAssert.InRange(random.Next(bound1, bound2), bound1, bound2 - 1);
        }

        int NextBound()
        {
            int bound = random.Next();
            bool flipSign = random.Next() % 2 == 1;

            return flipSign ? -bound : bound;
        }
    }

    [Fact]
    public void NextBytesEdges()
    {
        var random = NewFixedSeedRandom();

        XAssert.Throws<ArgumentNullException>(() => random.NextBytes(null!));

        byte[] bytes = [];
        random.NextBytes(bytes);
        XAssert.Empty(bytes);
    }

    [Fact]
    public void NextBytesStatistical()
    {
        var random = NewFixedSeedRandom();

        // We'd like to test with byte arrays in length from 1 to 17 bytes.
        // For each length, we are going to randomly populate the array 1MM runs.
        // For each run, we are going to make a histogram for each position in the array.
        // For each position, we should "see" all values in a random amount within some threshold.
        //
        // Empirically determined thresholds:
        //  100k runs = 0.341
        //   1MM runs = 0.112
        //  10MM runs = 0.036 (20+ seconds runtime)
        // 100MM runs = 0.012
        //
        // TODO : Determine if the above values are statistically "expected".
        const int runs = 1_000_000;
        const double minMaxDiffPercentageThreshold = 0.112;

        for (int length = 1; length <= (2 * sizeof(ulong)) + 1; length++)
        {
            int[][] histograms = new int[length][];

            for (int position = 0; position < length; position++)
                histograms[position] = new int[256];

            for (int run = 0; run < runs; run++)
            {
                byte[] bytes = new byte[length];
                random.NextBytes(bytes);

                for (int position = 0; position < length; position++)
                    histograms[position][bytes[position]]++;
            }

            for (int position = 0; position < length; position++)
            {
                int min = histograms[position].Min();
                int max = histograms[position].Max();

                double minMaxDiffPercentage = (max - min) / (double)max;

                XAssert.InRange(minMaxDiffPercentage, 0.0, minMaxDiffPercentageThreshold);
            }
        }
    }
}