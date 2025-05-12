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