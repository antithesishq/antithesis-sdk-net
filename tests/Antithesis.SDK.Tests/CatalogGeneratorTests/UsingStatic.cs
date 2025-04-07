namespace SomeCompany.SomeProject;

using static Antithesis.SDK.Assert;
using System.Collections.Generic;

public sealed class SomeClass
{
    public SomeClass()
    {
        Always(true, "Constructor Literal");
    }

    public static void SomeMethodBlock()
    {
        AlwaysGreaterThan(2, 1, Ids.Field1);
        Reachable("SomeMethodBlock Literal");
    }

    public static void SomeMethodExpression() =>
        AlwaysSome(new Dictionary<string, bool>() { [string.Empty] = true }, "SomeMethodExpression Literal");

    public int SomeProperty
    {
        get
        {
            Sometimes(true, "SomeProperty Literal");
            return 1;
        }
        set => Sometimes(true, Ids.Field2);
    }

    public string this[int i] { get { Reachable(Id); return i.ToString(); } }

    internal const string Id = "Class Member Const Field";
}

public static class Ids
{
    public const string Field1 = "Ids Field1 Const";
    public const string Field2 = "Ids Field2 Const";
}