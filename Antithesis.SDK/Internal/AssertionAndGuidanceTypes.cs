namespace Antithesis.SDK;

internal enum AssertionAssertType
{
    Always,
    Sometimes,
    Reachability
}

internal enum AssertionDisplayType
{
    Always,
    AlwaysOrUnreachable,
    Sometimes,
    Unreachable,
    Reachable
}

internal enum AssertionMethodType
{
    Always,
    AlwaysOrUnreachable,
    AlwaysGreaterThan,
    AlwaysGreaterThanOrEqualTo,
    AlwaysLessThan,
    AlwaysLessThanOrEqualTo,
    AlwaysSome,
    Sometimes,
    SometimesGreaterThan,
    SometimesGreaterThanOrEqualTo,
    SometimesLessThan,
    SometimesLessThanOrEqualTo,
    SometimesAll,
    Unreachable,
    Reachable
}

internal enum GuidanceType
{
    Numeric,
    Boolean,
    Json
}

internal static class AssertionDisplayTypeExtensions
{
    internal static bool GetMustHit(this AssertionDisplayType displayType) => displayType switch
    {
        AssertionDisplayType.Always or
        AssertionDisplayType.Sometimes or
        AssertionDisplayType.Reachable =>
            true,
        
        AssertionDisplayType.AlwaysOrUnreachable or
        AssertionDisplayType.Unreachable =>
            false,

        _ => throw new NotImplementedException(displayType.ToString())
    };
}

internal static class AssertionMethodTypeExtensions
{
    internal static AssertionAssertType GetAssertType(this AssertionMethodType methodType) => methodType switch
    {
        AssertionMethodType.Always or
        AssertionMethodType.AlwaysOrUnreachable or
        AssertionMethodType.AlwaysGreaterThan or
        AssertionMethodType.AlwaysGreaterThanOrEqualTo or
        AssertionMethodType.AlwaysLessThan or
        AssertionMethodType.AlwaysLessThanOrEqualTo or
        AssertionMethodType.AlwaysSome =>
            AssertionAssertType.Always,

        AssertionMethodType.Sometimes or
        AssertionMethodType.SometimesGreaterThan or
        AssertionMethodType.SometimesGreaterThanOrEqualTo or
        AssertionMethodType.SometimesLessThan or
        AssertionMethodType.SometimesLessThanOrEqualTo or
        AssertionMethodType.SometimesAll =>
            AssertionAssertType.Sometimes,

        AssertionMethodType.Unreachable or
        AssertionMethodType.Reachable =>
            AssertionAssertType.Reachability,

        _ => throw new NotImplementedException(methodType.ToString())
    };

    internal static AssertionDisplayType GetDisplayType(this AssertionMethodType methodType) => methodType switch
    {
        AssertionMethodType.Always or
        AssertionMethodType.AlwaysGreaterThan or
        AssertionMethodType.AlwaysGreaterThanOrEqualTo or
        AssertionMethodType.AlwaysLessThan or
        AssertionMethodType.AlwaysLessThanOrEqualTo or
        AssertionMethodType.AlwaysSome =>
            AssertionDisplayType.Always,

        AssertionMethodType.AlwaysOrUnreachable =>
            AssertionDisplayType.AlwaysOrUnreachable,

        AssertionMethodType.Sometimes or
        AssertionMethodType.SometimesGreaterThan or
        AssertionMethodType.SometimesGreaterThanOrEqualTo or
        AssertionMethodType.SometimesLessThan or
        AssertionMethodType.SometimesLessThanOrEqualTo or
        AssertionMethodType.SometimesAll =>
            AssertionDisplayType.Sometimes,

        AssertionMethodType.Unreachable =>
            AssertionDisplayType.Unreachable,

        AssertionMethodType.Reachable =>
            AssertionDisplayType.Reachable,

        _ => throw new NotImplementedException(methodType.ToString())
    };

    internal static GuidanceType GetGuidanceType(this AssertionMethodType methodType) => methodType switch
    {
        AssertionMethodType.AlwaysGreaterThan or
        AssertionMethodType.AlwaysGreaterThanOrEqualTo or
        AssertionMethodType.AlwaysLessThan or
        AssertionMethodType.AlwaysLessThanOrEqualTo or
        AssertionMethodType.SometimesGreaterThan or
        AssertionMethodType.SometimesGreaterThanOrEqualTo or
        AssertionMethodType.SometimesLessThan or
        AssertionMethodType.SometimesLessThanOrEqualTo =>
            GuidanceType.Numeric,
        
        AssertionMethodType.AlwaysSome or
        AssertionMethodType.SometimesAll =>
            GuidanceType.Boolean,

        _ => throw new NotSupportedException(methodType.ToString())
    };

    internal static bool GetGuidanceMaximize(this AssertionMethodType methodType) => methodType switch
    {
        AssertionMethodType.AlwaysGreaterThan or
        AssertionMethodType.AlwaysGreaterThanOrEqualTo or
        AssertionMethodType.SometimesGreaterThan or
        AssertionMethodType.SometimesGreaterThanOrEqualTo or
        AssertionMethodType.AlwaysSome  =>
            false,

        AssertionMethodType.AlwaysLessThan or
        AssertionMethodType.AlwaysLessThanOrEqualTo or
        AssertionMethodType.SometimesLessThan or
        AssertionMethodType.SometimesLessThanOrEqualTo or
        AssertionMethodType.SometimesAll =>
            true,

        _ => throw new NotSupportedException(methodType.ToString())
    };
}