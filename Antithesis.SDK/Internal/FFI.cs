namespace Antithesis.SDK;

using System.IO;
using System.Runtime.InteropServices;

internal class FFIRandomUInt64Provider : FFI, IRandomUInt64Provider
{
    ulong IRandomUInt64Provider.Next() => fuzz_get_random();
}

internal class FFISink : FFI, ISink
{
    void ISink.Write(string message)
    {
        fuzz_json_data(message, (ulong)message.Length);
        fuzz_flush();
    }
}

internal class FFI
{
    protected FFI() => ThrowIfNotFileExists();
    
    private const string FilePath = "/usr/lib/libvoidstar.so";
    internal static bool FileExists { get; } = File.Exists(FilePath);

    internal static void ThrowIfNotFileExists()
    {
        if (!FileExists)
            throw new FileNotFoundException(FilePath);
    }

    [DllImport(FilePath)]
    protected static extern ulong fuzz_get_random();

    [DllImport(FilePath)]
    protected static extern void fuzz_json_data([MarshalAs(UnmanagedType.LPUTF8Str)] string message, ulong length);

    [DllImport(FilePath)]
    protected static extern void fuzz_flush();
}