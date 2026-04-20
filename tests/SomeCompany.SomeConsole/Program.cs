using Antithesis.SDK;
using SomeCompany.SomeConsole;

Lifecycle.SetupComplete();
Assert.Reachable("Program.Main.Reachable");

Lifecycle.SendEvent("AssertionTrackerTests.OnlyWriteFirstPassAndFirstFail");
AssertionTrackerTests.OnlyWriteFirstPassAndFirstFail();

Lifecycle.SendEvent("GuidanceTrackerTests.BooleanWriteAlways");
GuidanceTrackerTests.BooleanWriteAlways();

Lifecycle.SendEvent("GuidanceTrackerTests.NumericWriteAsMaxOrMinimizing");
GuidanceTrackerTests.NumericWriteAsMaxOrMinimizing();