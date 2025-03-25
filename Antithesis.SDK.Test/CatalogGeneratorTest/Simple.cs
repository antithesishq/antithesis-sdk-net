namespace Antithesis.SDK
{
    using System.Collections.Generic;
    using System.Text.Json.Nodes;

    public static class Assert
    {
        // Test at least one of each "signature type" / position of the idIsTheMessage Parameter.

        public static void Always(bool condition, string idIsTheMessage, JsonObject? details = default) { }
        public static void AlwaysGreaterThan(int left, int right, string idIsTheMessage, JsonObject? details = default) { }
        public static void AlwaysSome(IReadOnlyDictionary<string, bool> conditions, string idIsTheMessage, JsonObject? details = default) { }
        public static void Reachable(string idIsTheMessage, JsonObject? details = default) { }
        public static void Sometimes(bool condition, string idIsTheMessage, JsonObject? details = default) { }
    }
}

namespace SomeCompany.SomeProject
{
    using Antithesis.SDK;
    using System.Collections.Generic;

    public sealed class SomeClass
    {
        public SomeClass()
        {
            Assert.Always(true, "Constructor Literal");
        }

        public static void SomeMethodBlock()
        {
            Assert.AlwaysGreaterThan(2, 1, Ids.Field1);
            Assert.Reachable("SomeMethodBlock Literal");
        }

        public static void SomeMethodExpression() =>
            Assert.AlwaysSome(new Dictionary<string, bool>() { [string.Empty] = true }, "SomeMethodExpression Literal");

        public int SomeProperty
        {
            get
            {
                Antithesis.SDK.Assert.Sometimes(true, "SomeProperty Literal");
                return 1;
            }
            set => Antithesis.SDK.Assert.Sometimes(true, Ids.Field2);
        }

        public string this[int i] { get { Assert.Reachable(Id); return i.ToString(); } }

        internal const string Id = "Class Member Const Field";
    }

    public static class Ids
    {
        public const string Field1 = "Ids Field1 Const";
        public const string Field2 = "Ids Field2 Const";
    }
}