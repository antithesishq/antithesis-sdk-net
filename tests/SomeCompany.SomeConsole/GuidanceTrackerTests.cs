namespace SomeCompany.SomeConsole;

using Antithesis.SDK;
using System.Collections.Generic;

public static class GuidanceTrackerTests
{
    public static void BooleanWriteAlways() { /* Covered by AssertionTrackerTests. */ }
    
    public static void NumericWriteAsMaxOrMinimizing()
    {
        double[] operands = [ 1.0, 2.0, 0.0, -1.0, -2.0, -1.0 ];

        foreach (double operand in operands)
            Assert.AlwaysGreaterThan(operand, 0, "GuidanceTrackerTests.NumericWriteAsMaxOrMinimizing.AlwaysGreaterThan");

        foreach (double operand in operands)
            Assert.SometimesLessThan(0, operand, "GuidanceTrackerTests.NumericWriteAsMaxOrMinimizing.SometimesLessThan");
    }

    public static void EveryMethodWritesItsOwnDirection()
    {
        Assert.AlwaysGreaterThan(1, 0, "GuidanceTrackerTests.EveryMethodWritesItsOwnDirection.AlwaysGreaterThan");
        Assert.AlwaysGreaterThanOrEqualTo(1, 0, "GuidanceTrackerTests.EveryMethodWritesItsOwnDirection.AlwaysGreaterThanOrEqualTo");
        Assert.AlwaysLessThan(0, 1, "GuidanceTrackerTests.EveryMethodWritesItsOwnDirection.AlwaysLessThan");
        Assert.AlwaysLessThanOrEqualTo(0, 1, "GuidanceTrackerTests.EveryMethodWritesItsOwnDirection.AlwaysLessThanOrEqualTo");
        Assert.SometimesGreaterThan(1, 0, "GuidanceTrackerTests.EveryMethodWritesItsOwnDirection.SometimesGreaterThan");
        Assert.SometimesGreaterThanOrEqualTo(1, 0, "GuidanceTrackerTests.EveryMethodWritesItsOwnDirection.SometimesGreaterThanOrEqualTo");
        Assert.SometimesLessThan(0, 1, "GuidanceTrackerTests.EveryMethodWritesItsOwnDirection.SometimesLessThan");
        Assert.SometimesLessThanOrEqualTo(0, 1, "GuidanceTrackerTests.EveryMethodWritesItsOwnDirection.SometimesLessThanOrEqualTo");
        Assert.AlwaysSome(new Dictionary<string, bool> { ["yes"] = true }, "GuidanceTrackerTests.EveryMethodWritesItsOwnDirection.AlwaysSome");
        Assert.SometimesAll(new Dictionary<string, bool> { ["yes"] = true }, "GuidanceTrackerTests.EveryMethodWritesItsOwnDirection.SometimesAll");
    }

    public static void EmptyConditionsStillWriteAnEmptyPayload()
    {
        var none = new Dictionary<string, bool>();
        Assert.AlwaysSome(none, "GuidanceTrackerTests.EmptyConditionsStillWriteAnEmptyPayload.AlwaysSome");
        Assert.SometimesAll(none, "GuidanceTrackerTests.EmptyConditionsStillWriteAnEmptyPayload.SometimesAll");
    }
}
