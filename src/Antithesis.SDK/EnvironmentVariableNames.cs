namespace Antithesis.SDK;

/// <summary>
/// Contains Antithesis related environment variable names.
/// </summary>
public static class EnvironmentVariableNames
{
    /// <summary>
    /// When executing outside of the Antithesis platform, the default behavior of Assert, Catalog,
    /// and Lifecycle is no-op. Instead, you may set this environment variable in order to have
    /// those classes append their JSONL output to a local file. This environment variable must be
    /// set prior to starting a process and can be set to a file path that does or does not already
    /// exist. The file path must be located inside an already-existing directory.
    /// </summary>
    public const string LocalSinkOutputFilePath = "ANTITHESIS_SDK_LOCAL_OUTPUT";

    /// <summary>
    /// Set by Antithesis to inform applications where they should write structured logs
    /// for processing by Antithesis.
    /// </summary>
    public const string OutputDirectory = "ANTITHESIS_OUTPUT_DIR";
}