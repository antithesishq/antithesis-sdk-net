namespace Antithesis.SDK;

using System.IO;

internal interface ISink { void Write(string message); }

internal static class Sink
{
    static Sink() => Write(VersionInfoWrapper.Singleton);

    internal static void Write(object message) =>
        _singleton.Write(Serializer.Serialize(message));

    private static readonly ISink _singleton =
        FFI.FileExists
            ? new FFISink()
            : (LocalSink.FileExists
                ? new LocalSink()
                : new NoopSink());

    private sealed class LocalSink : ISink
    {
        private const string FilePathEnvironmentVariableName = "ANTITHESIS_SDK_LOCAL_OUTPUT";
        private static readonly string? FilePath = Environment.GetEnvironmentVariable(FilePathEnvironmentVariableName);
        
        internal static bool FileExists = !string.IsNullOrEmpty(FilePath) && File.Exists(FilePath);

        void ISink.Write(string message) 
        {
            // This is not thread-safe; however, LocalSink should never be used under normal circumstances (the FFISink will be used instead);
            // it is simply provided for parity with the Java SDK.
            try
            {
                using var writer = File.AppendText(FilePath!);

                writer.WriteLine(message);
            }
            catch (IOException) { /* Ignored. */ }
        }
    }

    private sealed class NoopSink : ISink
    {
        void ISink.Write(string message) { /* Noop */ }
    }
}