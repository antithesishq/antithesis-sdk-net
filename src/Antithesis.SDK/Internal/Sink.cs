namespace Antithesis.SDK;

using System.IO;

internal interface ISink { void Write(string message); }

internal static class Sink
{
    static Sink() =>
        Write(new SDKInfoWrapper(_singleton.GetType()));

    internal static void Write(object message) =>
        _singleton.Write(Serializer.Serialize(message));

    private static readonly ISink _singleton =
        FFI.FileExists
            ? new FFISink()
            : (LocalSink.FileOrDirectoryExists
                ? new LocalSink()
                : new NoopSink());

    // Must be placed after _singleton because of field initializer ordering.
    // 
    // We are making this a readonly property with a readonly backing field in order to short-circuit
    // all Assert, Catalog, and Lifecyle public static method calls with as minimal performance impact
    // as possible. This may be a premature optimization.
    internal static bool IsNoop { get; } = _singleton.GetType() == typeof(NoopSink);

    internal sealed class LocalSink : ISink
    {
        // The File will be created during File.AppendText; however, the Directory must exist to do so.
        internal static bool FileOrDirectoryExists
        {
            get
            {
                string? filePath = Environment.GetEnvironmentVariable(EnvironmentVariableNames.LocalSinkOutputFilePath);

                if (string.IsNullOrEmpty(filePath))
                    return false;

                if (File.Exists(filePath))
                    return true;

                string? directory = Path.GetDirectoryName(filePath);

                return !string.IsNullOrEmpty(directory) && Directory.Exists(directory);
            }
        }

        internal LocalSink() => _filePath = Environment.GetEnvironmentVariable(EnvironmentVariableNames.LocalSinkOutputFilePath)!;

        private readonly string _filePath;
        private readonly object _padlock = new();

        void ISink.Write(string message) 
        {
            lock(_padlock)
            {
                try
                {
                    using var writer = File.AppendText(_filePath);

                    writer.WriteLine(message);
                }
                catch (IOException) { /* Ignored. */ }
            }
        }
    }

    private sealed class NoopSink : ISink
    {
        void ISink.Write(string message) { /* Noop */ }
    }
}