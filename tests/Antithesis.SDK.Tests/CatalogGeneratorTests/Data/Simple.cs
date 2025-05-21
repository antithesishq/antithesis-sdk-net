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

        ~SomeClass()
        {
            Assert.Always(true, "Destructor Literal");
        }

        public static explicit operator byte(SomeClass _)
        {
            Assert.Always(true, "Conversion Operator Literal 1");

            return 0x00;
        }

        public static explicit operator System.String(SomeClass someClass)
        {
            Assert.Always(true, "Conversion Operator Literal 2");

            return someClass.ToString();
        }

        public static bool operator ==(SomeClass left, SomeClass right)
        {
            Assert.Always(true, "Operator Overload Literal");

            return left?.Equals(right) ?? false;
        }
    }

    public static class Ids
    {
        public const string Field1 = "Ids Field1 Const";
        public const string Field2 = "Ids Field2 Const";
    }
}