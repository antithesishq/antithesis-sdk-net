using Antithesis.SDK;

var builder = WebApplication.CreateBuilder(args);
// LogLevels would normally be set via appSettings.[Environment].json files; however, in order
// to make it explicit that we should not excessively log to Antithesis and to reduce the number
// of files in this simple example project, we are hardcoding the default LogLevel to Warning.
builder.Logging.SetMinimumLevel(LogLevel.Warning);

var app = builder.Build();

var random = Antithesis.SDK.Random.SharedThrowIfNativeLibraryNotExists;

app.MapGet("/", async () =>
{
    Assert.Always(true, "Hardcoded to Pass");
    Assert.SometimesGreaterThan(random.NextDouble(), 0.9999, "Should Pass Randomly 1 of 10k");

    await Task.Delay(100);
    
    return "Hello, Antithesis!";
});

Lifecycle.SetupComplete();

Assert.Reachable("Immediately After Setup Complete");

app.Run();

Assert.Unreachable("app.Run Blocks Indefinitely");