namespace Antithesis.SDK
{
    public static class Assert
    {
        public static void Always(bool condition, string idIsTheMessage, System.Text.Json.Nodes.JsonObject? details = default) { }
    }
}

namespace SomeCompany.SomeProject
{
    using Antithesis.SDK;

    public class SomeClass
    {
        public static void SomeMethod()
        {
            Assert.Always(true, Ids.Ambiguous);
            Assert.Always(true, Ids.NotAccessible1);
            Assert.Always(true, Ids.NotAccessible2);
            Assert.Always(true, Ids.NotField);
            Antithesis.SDK.Assert.Always(true, Ids.NotConst);
        }
    }

    public static class Ids
    {
        // Ambiguous because the class is declared static but this Field is not.
        public string Ambiguous = string.Empty;

        const string NotAccessible1 = string.Empty;
        private const string NotAccessible2 = string.Empty;

        public static string NotField => string.Empty;

        public static string NotConst = string.Empty;
    }
}