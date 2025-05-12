namespace SomeCompany.SomeProject
{
    using Antithesis.SDK;

    public class SomeClass
    {
        public void SomeMethod()
        {
            Assert.Always(true, Ids.Ambiguous);
            Assert.Sometimes(true, Ids.NotAccessible1);
            Assert.Reachable(Ids.NotAccessible2);
            Assert.Unreachable(Ids.NotField);
            Antithesis.SDK.Assert.Always(true, Ids.NotConst);

            string id = string.Empty;
            Assert.AlwaysGreaterThan(2, 1, id);

            Assert.AlwaysLessThan(1, 2, _id);
            Assert.SometimesGreaterThan(2, 1, _idReadonly);
            Assert.SometimesLessThan(1, 2, _idStaticReadonly);
            Assert.Always(true, _idConst);

            Assert.Sometimes(true, Id);
            Assert.Reachable(IdReadonly);
            Assert.Unreachable(IdStaticReadonly);

            Assert.Always(true, null);
            Assert.Always(true, default);
        }

        private string _id = string.Empty;
        private readonly string _idReadonly = string.Empty;
        private static readonly string _idStaticReadonly = string.Empty;
        private const string _idConst = string.Empty;

        public string Id = string.Empty;
        public readonly string IdReadonly = string.Empty;
        public static readonly string IdStaticReadonly = string.Empty;
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