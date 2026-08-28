namespace Antithesis.SDK;

using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
// File handles (including Linux file handles!) live under this namespace. We are not actually using the Win32 API here
using Microsoft.Win32.SafeHandles;

internal interface ISink { void Write(string message); }

internal static class Sink
{
    static Sink() =>
        Write(new SDKInfoWrapper(_singleton.GetType()));

    // Forces this class's static constructor without doing anything else.
    internal static void Touch() { }

    internal static void Write(object message) =>
        _singleton.Write(JsonSerializer.Serialize(message));

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
        // The File will be created on the first Write; however, the Directory must exist to do so.
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

        internal LocalSink()
        {
            _filePath = Environment.GetEnvironmentVariable(EnvironmentVariableNames.LocalSinkOutputFilePath)!;
            _appendHandle = Native.TryOpenAppend(_filePath);

            if (_appendHandle is null)
                Console.Error.WriteLine(
                    $"{ErrorLogLinePrefix} Unable to open \"{_filePath}\" for appending; records written by several processes to this path may overwrite one another.");
        }

        private const string ErrorLogLinePrefix = "[* antithesis-sdk-dotnet *]";

        private readonly string _filePath;
        private readonly object _padlock = new();
        private readonly SafeFileHandle? _appendHandle;

        void ISink.Write(string message)
        {
            if (_appendHandle is not null)
            {
                byte[] line = new byte[Encoding.UTF8.GetByteCount(message) + 1];
                Encoding.UTF8.GetBytes(message, 0, message.Length, line, 0);
                line[^1] = (byte)'\n';

                Native.Write(_appendHandle, line, (nuint)line.Length);

                return;
            }

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

        // Using a custom file handle binding so that the file can be opened in O_APPEND mode.
        // .NET's FileMode.Append implements its own in-process cursor tracking and is not resilient to
        // multiple writers to the same file. 
        private static class Native
        {
            private const int O_WRONLY = 0x0001;
            private const int O_CREAT = 0x0040;
            private const int O_APPEND = 0x0400;
            private const int Mode0644 = 0x1A4;

            [DllImport("libc", EntryPoint = "open", SetLastError = true)]
            private static extern int Open([MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags, int mode);

            [DllImport("libc", EntryPoint = "write", SetLastError = true)]
            internal static extern nint Write(SafeFileHandle fd, byte[] buffer, nuint count);

            // Returns null if this is not Linux, or libc or the file cannot be opened, in which case
            // the caller falls back to the managed, lock-based append.
            internal static SafeFileHandle? TryOpenAppend(string path)
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    // The O_* flag values are Linux-specific
                    return null;

                try
                {
                    int fd = Open(path, O_WRONLY | O_CREAT | O_APPEND, Mode0644);

                    return fd < 0 ? null : new SafeFileHandle((IntPtr)fd, ownsHandle: true);
                }
                catch (DllNotFoundException) { return null; }
                catch (EntryPointNotFoundException) { return null; }
            }
        }
    }

    private sealed class NoopSink : ISink
    {
        void ISink.Write(string message) { /* Noop */ }
    }
}
