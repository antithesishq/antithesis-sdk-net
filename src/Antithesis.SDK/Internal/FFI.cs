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
        // The length must be the UTF-8 byte count, not string.Length (UTF-16
        // code units).
        byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(message);
        fuzz_json_data(utf8, (ulong)utf8.Length);
        fuzz_flush();
    }
}

internal class FFI
{
    protected FFI()
    {
        if (!FileExists)
            throw new FileNotFoundException(FilePath);
    }

    private const string FilePath = "/usr/lib/libvoidstar.so";
    internal static bool FileExists { get; } = File.Exists(FilePath);

    [DllImport(FilePath)]
    protected static extern ulong fuzz_get_random();

    [DllImport(FilePath)]
    protected static extern void fuzz_json_data(byte[] message, ulong length);

    [DllImport(FilePath)]
    protected static extern void fuzz_flush();
}