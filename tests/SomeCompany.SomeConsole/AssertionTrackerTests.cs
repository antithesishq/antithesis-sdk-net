namespace SomeCompany.SomeConsole;

using Antithesis.SDK;

public static class AssertionTrackerTests
{
    public static void OnlyWriteFirstPassAndFirstFail()
    {
        foreach (bool condition in new bool[] { true, false, true, false })
        {
            Assert.Always(condition, "AssertionTrackerTests.OnlyWriteFirstPassAndFirstFail.Always");
            Assert.Sometimes(condition, "AssertionTrackerTests.OnlyWriteFirstPassAndFirstFail.Sometimes");

            Assert.Reachable("AssertionTrackerTests.OnlyWriteFirstPassAndFirstFail.Reachable");
            Assert.Unreachable("AssertionTrackerTests.OnlyWriteFirstPassAndFirstFail.Unreachable");
        }
    }
}