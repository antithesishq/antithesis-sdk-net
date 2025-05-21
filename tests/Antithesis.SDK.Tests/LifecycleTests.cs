namespace Antithesis.SDK;

using System.Text.Json;

public class LifecycleTests
{
    [Fact]
    public void SetupComplete()
    {
        XAssert.Equal(
            "{\"antithesis_setup\":{\"status\":\"complete\",\"details\":null}}",
            JsonSerializer.Serialize(
                Lifecycle.SendEventJson(
                    Lifecycle.SetupCompleteJsonPropertyName,
                    Lifecycle.SetupCompleteJsonDetails())));
    }
}