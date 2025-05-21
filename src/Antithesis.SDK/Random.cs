namespace Antithesis.SDK;

using System.Diagnostics;

// This interface exists to better support mocking and testing.
internal interface IRandomUInt64Provider { ulong Next(); }

// Methods adapted or inspired by:
// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Random.ImplBase.cs
// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Random.Xoshiro256StarStarImpl.cs

/// <summary>
/// The Random class is a subclass of <see cref="System.Random">System.Random</see> that encapsules Antithesis's deterministic and
/// reproducible random number generator.
/// </summary>
/// <remarks>
/// Regarding the methods which have to be overriden with Sample when subclassing System.Random:
/// <a href="https://learn.microsoft.com/en-us/dotnet/api/system.random.sample?view=net-9.0#notes-to-inheritors">Microsoft Learn</a>.
/// We also chose to override NextBytes(Span&lt;Byte&gt;) because we used it for NextBytes(Byte[]) which had to be overridden.
/// </remarks>
public class Random : System.Random
{
    /// <inheritdoc />
    [Obsolete("Please use Antithesis.SDK.Random.SharedFallbackToSystem which explicitly defines its behavior when outside Antithesis.", true)]
    new public static System.Random Shared => throw new NotSupportedException();

    /// <summary>
    /// Returns a Singleton of this class when executing within Antithesis; else, falls back to System.Random.Shared.
    /// </summary>
    public static System.Random SharedFallbackToSystem { get; } =
        FFI.FileExists ? new Random(new FFIRandomUInt64Provider()) : System.Random.Shared;

    internal Random(IRandomUInt64Provider randomUInt64) =>
        _randomUInt64 = randomUInt64 ?? throw new ArgumentNullException(nameof(randomUInt64));

    private readonly IRandomUInt64Provider _randomUInt64;

    /// <inheritdoc />
    protected override double Sample()
    {
        // Regarding converting the ulong to double between [0.0, 1.0) :
        // https://github.com/dotnet/runtime/blob/cc803458ab4dabf020b462873be5bf56f1640b1e/src/libraries/System.Private.CoreLib/src/System/Random.Xoshiro256StarStarImpl.cs#L182
        //
        // As described in http://prng.di.unimi.it/:
        // "A standard double (64-bit) floating-point number in IEEE floating point format has 52 bits of significand,
        //  plus an implicit bit at the left of the significand. Thus, the representation can actually store numbers with
        //  53 significant binary digits. Because of this fact, in C99 a 64-bit unsigned integer x should be converted to
        //  a 64-bit double using the expression
        //  (x >> 11) * 0x1.0p-53"
        return (_randomUInt64.Next() >> 11) * (1.0 / (1ul << 53));
    }

    /// <inheritdoc />
    public override int Next()
    {
        while (true)
        {
            // Regarding the loop :
            // https://github.com/dotnet/runtime/blob/cc803458ab4dabf020b462873be5bf56f1640b1e/src/libraries/System.Private.CoreLib/src/System/Random.Xoshiro256StarStarImpl.cs#L76
            //
            // Get top 31 bits to get a value in the range [0, int.MaxValue], but try again
            // if the value is actually int.MaxValue, as the method is defined to return a value
            // in the range [0, int.MaxValue).
            ulong result = _randomUInt64.Next() >> 33;

            if (result != int.MaxValue)
                return (int)result;
        }
    }

    /// <inheritdoc />
    public override int Next(int minValue, int maxValue)
    {
        if (minValue > maxValue)
            throw new ArgumentOutOfRangeException(nameof(minValue));

        if (minValue == maxValue)
            return minValue;

        return (int)NextUInt32((uint)(maxValue - minValue)) + minValue;
    }

    private uint NextUInt32(uint maxValue)
    {
        // Regarding this algorithm:
        // https://github.com/dotnet/runtime/blob/cc803458ab4dabf020b462873be5bf56f1640b1e/src/libraries/System.Private.CoreLib/src/System/Random.ImplBase.cs#L36
        // 
        // NextUInt32/64 algorithms based on https://arxiv.org/pdf/1805.10941.pdf and https://github.com/lemire/fastrange.

        ulong randomProduct = (ulong)maxValue * (_randomUInt64.Next() >> 32);
        uint lowPart = (uint)randomProduct;

        if (lowPart < maxValue)
        {
            uint remainder = (0u - maxValue) % maxValue;

            while (lowPart < remainder)
            {
                randomProduct = (ulong)maxValue * (_randomUInt64.Next() >> 32);
                lowPart = (uint)randomProduct;
            }
        }

        return (uint)(randomProduct >> 32);
    }

    /// <inheritdoc />
    public override void NextBytes(byte[] buffer)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));

        NextBytes((Span<byte>)buffer);
    }

    /// <inheritdoc />
    public override void NextBytes(Span<byte> buffer)
    {
        while (buffer.Length >= sizeof(ulong))
        {
            ulong random = _randomUInt64.Next();
            BitConverter.GetBytes(random).CopyTo(buffer);

            buffer = buffer.Slice(sizeof(ulong));
        }

        if (!buffer.IsEmpty)
        {
            ulong random = _randomUInt64.Next();
            byte[] bytes = BitConverter.GetBytes(random);

            Debug.Assert(buffer.Length < bytes.Length);

            for (int i = 0; i < buffer.Length && i < bytes.Length; i++)
                buffer[i] = bytes[i];
        }
    }
}