namespace SomeCompany.SomeConsole;

using Antithesis.SDK;

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
}