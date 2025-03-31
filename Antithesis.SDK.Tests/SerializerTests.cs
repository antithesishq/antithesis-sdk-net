namespace Antithesis.SDK;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public class SerializerTests
{
    [Fact]
    public void String()
    {
        string message = "Antithesis Operations LLC";
        XAssert.Equal(message, Serializer.Serialize(message));
    }

    [Fact]
    public void NonString()
    {
        XAssert.Equal(
            "{\"class\":\"class\",\"function\":\"function\",\"file\":\"file\",\"begin_line\":0,\"begin_column\":0}",
            Serializer.Serialize(LocationInfo.Unknown));
    }

    [Fact]
    public void NestedJsonObject()
    {
        XAssert.Equal(
            "{\"property1\":1,\"property2\":2,\"nested\":{\"property3\":3,\"property4\":4}}",
            Serializer.Serialize(new InfoWithNestedJsonObject()));
    }

    private class InfoWithNestedJsonObject
    {
        [JsonPropertyName("property1")]
        public int Property1 => 1;

        [JsonPropertyName("property2")]
        public int Property2 => 2;

        [JsonPropertyName("nested")]
        public JsonObject Nested => new()
        {
            ["property3"] = 3,
            ["property4"] = 4
        };
    }
}