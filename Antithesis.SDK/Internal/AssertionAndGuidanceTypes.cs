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

internal enum AssertionVerboseType
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

internal static class AssertionVerboseTypeExtensions
{
    internal static AssertionAssertType GetAssertType(this AssertionVerboseType verboseType) => verboseType switch
    {
        AssertionVerboseType.Always or
        AssertionVerboseType.AlwaysOrUnreachable or
        AssertionVerboseType.AlwaysGreaterThan or
        AssertionVerboseType.AlwaysGreaterThanOrEqualTo or
        AssertionVerboseType.AlwaysLessThan or
        AssertionVerboseType.AlwaysLessThanOrEqualTo or
        AssertionVerboseType.AlwaysSome =>
            AssertionAssertType.Always,

        AssertionVerboseType.Sometimes or
        AssertionVerboseType.SometimesGreaterThan or
        AssertionVerboseType.SometimesGreaterThanOrEqualTo or
        AssertionVerboseType.SometimesLessThan or
        AssertionVerboseType.SometimesLessThanOrEqualTo or
        AssertionVerboseType.SometimesAll =>
            AssertionAssertType.Sometimes,

        AssertionVerboseType.Unreachable or
        AssertionVerboseType.Reachable =>
            AssertionAssertType.Reachability,

        _ => throw new NotImplementedException(verboseType.ToString())
    };

    internal static AssertionDisplayType GetDisplayType(this AssertionVerboseType verboseType) => verboseType switch
    {
        AssertionVerboseType.Always or
        AssertionVerboseType.AlwaysGreaterThan or
        AssertionVerboseType.AlwaysGreaterThanOrEqualTo or
        AssertionVerboseType.AlwaysLessThan or
        AssertionVerboseType.AlwaysLessThanOrEqualTo or
        AssertionVerboseType.AlwaysSome =>
            AssertionDisplayType.Always,

        AssertionVerboseType.AlwaysOrUnreachable =>
            AssertionDisplayType.AlwaysOrUnreachable,

        AssertionVerboseType.Sometimes or
        AssertionVerboseType.SometimesGreaterThan or
        AssertionVerboseType.SometimesGreaterThanOrEqualTo or
        AssertionVerboseType.SometimesLessThan or
        AssertionVerboseType.SometimesLessThanOrEqualTo or
        AssertionVerboseType.SometimesAll =>
            AssertionDisplayType.Sometimes,

        AssertionVerboseType.Unreachable =>
            AssertionDisplayType.Unreachable,

        AssertionVerboseType.Reachable =>
            AssertionDisplayType.Reachable,

        _ => throw new NotImplementedException(verboseType.ToString())
    };

    internal static GuidanceType GetGuidanceType(this AssertionVerboseType verboseType) => verboseType switch
    {
        AssertionVerboseType.AlwaysGreaterThan or
        AssertionVerboseType.AlwaysGreaterThanOrEqualTo or
        AssertionVerboseType.AlwaysLessThan or
        AssertionVerboseType.AlwaysLessThanOrEqualTo or
        AssertionVerboseType.SometimesGreaterThan or
        AssertionVerboseType.SometimesGreaterThanOrEqualTo or
        AssertionVerboseType.SometimesLessThan or
        AssertionVerboseType.SometimesLessThanOrEqualTo =>
            GuidanceType.Numeric,
        
        AssertionVerboseType.AlwaysSome or
        AssertionVerboseType.SometimesAll =>
            GuidanceType.Boolean,

        _ => throw new NotSupportedException(verboseType.ToString())
    };

    internal static bool GetGuidanceMaximize(this AssertionVerboseType verboseType) => verboseType switch
    {
        AssertionVerboseType.AlwaysGreaterThan or
        AssertionVerboseType.AlwaysGreaterThanOrEqualTo or
        AssertionVerboseType.SometimesGreaterThan or
        AssertionVerboseType.SometimesGreaterThanOrEqualTo or
        AssertionVerboseType.AlwaysSome  =>
            false,

        AssertionVerboseType.AlwaysLessThan or
        AssertionVerboseType.AlwaysLessThanOrEqualTo or
        AssertionVerboseType.SometimesLessThan or
        AssertionVerboseType.SometimesLessThanOrEqualTo or
        AssertionVerboseType.SometimesAll =>
            true,

        _ => throw new NotSupportedException(verboseType.ToString())
    };
}