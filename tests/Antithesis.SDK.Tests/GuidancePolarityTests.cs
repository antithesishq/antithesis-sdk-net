namespace Antithesis.SDK;

using System;
using Xunit;

public class GuidancePolarityTests
{
    [Theory]
    // Violating left > right means driving left - right down.
    [InlineData(AssertionMethodType.AlwaysGreaterThan, GuidanceType.Numeric, false)]
    [InlineData(AssertionMethodType.AlwaysGreaterThanOrEqualTo, GuidanceType.Numeric, false)]
    // Violating left < right means driving left - right up.
    [InlineData(AssertionMethodType.AlwaysLessThan, GuidanceType.Numeric, true)]
    [InlineData(AssertionMethodType.AlwaysLessThanOrEqualTo, GuidanceType.Numeric, true)]
    // Satisfying left > right means driving left - right up.
    [InlineData(AssertionMethodType.SometimesGreaterThan, GuidanceType.Numeric, true)]
    [InlineData(AssertionMethodType.SometimesGreaterThanOrEqualTo, GuidanceType.Numeric, true)]
    // Satisfying left < right means driving left - right down.
    [InlineData(AssertionMethodType.SometimesLessThan, GuidanceType.Numeric, false)]
    [InlineData(AssertionMethodType.SometimesLessThanOrEqualTo, GuidanceType.Numeric, false)]
    // Violating "at least one is true" means driving toward none of them.
    [InlineData(AssertionMethodType.AlwaysSome, GuidanceType.Boolean, false)]
    // Satisfying "all are true" means driving toward all of them.
    [InlineData(AssertionMethodType.SometimesAll, GuidanceType.Boolean, true)]
    internal void MatchesSpecification(AssertionMethodType methodType, GuidanceType guidanceType, bool maximize)
    {
        XAssert.Equal(guidanceType, methodType.GetGuidanceType());
        XAssert.Equal(maximize, methodType.GetGuidanceMaximize());
    }

    [Theory]
    [InlineData(AssertionMethodType.AlwaysGreaterThan, AssertionMethodType.SometimesGreaterThan)]
    [InlineData(AssertionMethodType.AlwaysGreaterThanOrEqualTo, AssertionMethodType.SometimesGreaterThanOrEqualTo)]
    [InlineData(AssertionMethodType.AlwaysLessThan, AssertionMethodType.SometimesLessThan)]
    [InlineData(AssertionMethodType.AlwaysLessThanOrEqualTo, AssertionMethodType.SometimesLessThanOrEqualTo)]
    [InlineData(AssertionMethodType.AlwaysSome, AssertionMethodType.SometimesAll)]
    internal void AlwaysAndSometimesSeekOppositeExtremes(AssertionMethodType always, AssertionMethodType sometimes) =>
        XAssert.NotEqual(always.GetGuidanceMaximize(), sometimes.GetGuidanceMaximize());

    [Theory]
    [InlineData(AssertionMethodType.Always, false)]
    [InlineData(AssertionMethodType.AlwaysOrUnreachable, false)]
    [InlineData(AssertionMethodType.Sometimes, false)]
    [InlineData(AssertionMethodType.Reachable, false)]
    [InlineData(AssertionMethodType.Unreachable, false)]
    [InlineData(AssertionMethodType.AlwaysGreaterThan, true)]
    [InlineData(AssertionMethodType.AlwaysGreaterThanOrEqualTo, true)]
    [InlineData(AssertionMethodType.AlwaysLessThan, true)]
    [InlineData(AssertionMethodType.AlwaysLessThanOrEqualTo, true)]
    [InlineData(AssertionMethodType.SometimesGreaterThan, true)]
    [InlineData(AssertionMethodType.SometimesGreaterThanOrEqualTo, true)]
    [InlineData(AssertionMethodType.SometimesLessThan, true)]
    [InlineData(AssertionMethodType.SometimesLessThanOrEqualTo, true)]
    [InlineData(AssertionMethodType.AlwaysSome, true)]
    [InlineData(AssertionMethodType.SometimesAll, true)]
    internal void HasGuidanceAgreesWithHavingAGuidanceType(AssertionMethodType methodType, bool expected)
    {
        XAssert.Equal(expected, methodType.HasGuidance());
        bool hasType;
        try
        {
            _ = methodType.GetGuidanceType();
            hasType = true;
        }
        catch (NotSupportedException)
        {
            hasType = false;
        }
        XAssert.Equal(hasType, methodType.HasGuidance());
    }
}
