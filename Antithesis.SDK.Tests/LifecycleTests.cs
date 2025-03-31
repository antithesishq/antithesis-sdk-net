namespace Antithesis.SDK;

public class LifecycleTests
{
    [Fact]
    public void SetupComplete()
    {
        XAssert.Equal(
            "{\"antithesis_setup\":{\"status\":\"complete\",\"details\":null}}",
            Serializer.Serialize(
                Lifecycle.SendEventJson(
                    Lifecycle.SetupCompleteJsonPropertyName,
                    Lifecycle.SetupCompleteJsonDetails())));
    }
}