namespace Antithesis.SDK;

/// <summary>
/// Contains Antithesis related environment variable names.
/// </summary>
public static class EnvironmentVariableNames
{
    internal const string LocalSinkOutputFilePath = "ANTITHESIS_SDK_LOCAL_OUTPUT";

    /// <summary>
    /// Set by Antithesis to inform applications where they should write structured logs
    /// for processing by Antithesis.
    /// </summary>
    public const string OutputDirectory = "ANTITHESIS_OUTPUT_DIR";
}