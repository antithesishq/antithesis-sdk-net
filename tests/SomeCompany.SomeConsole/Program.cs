using Antithesis.SDK;
using SomeCompany.SomeConsole;

Lifecycle.SetupComplete();

Assert.Reachable("Program.Main.Reachable");

AssertionTrackerTests.OnlyWriteFirstPassAndFirstFail();

GuidanceTrackerTests.BooleanWriteAlways();
GuidanceTrackerTests.NumericWriteAsMaxOrMinimizing();