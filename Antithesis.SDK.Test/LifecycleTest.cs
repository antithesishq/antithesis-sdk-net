namespace Antithesis.SDK;

public class LifecycleTest
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