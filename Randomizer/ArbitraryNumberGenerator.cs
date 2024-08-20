using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Randomizer;

public class ArbitraryNumberGenerator(IRandomNumberGenerator rng) : IRandomNumberGenerator {
  
  public void Seed(ulong seed) => rng.Seed(seed);
  public ulong Next() => rng.Next();

  public uint Truncate32() => (uint)rng.Next();
  public ushort Truncate16() => (ushort)rng.Next();
  public byte Truncate8() => (byte)rng.Next();
  public bool Truncate1() => (rng.Next() & 1) == 1;

  public uint Shift32() => (uint)(rng.Next() >> 32);
  public ushort Shift16() => (ushort)(rng.Next() >> 48);
  public byte Shift8() => (byte)(rng.Next() >> 56);
  public bool Shift1() => (rng.Next() >> 63) == 1;

  public uint Mask32(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(_CountBits(mask), 32, nameof(mask));
    return (uint)_MaskBits(rng.Next(), mask);
  }

  public ushort Mask16(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(_CountBits(mask), 16, nameof(mask));
    return (ushort)_MaskBits(rng.Next(), mask);
  }

  public byte Mask8(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(_CountBits(mask), 8, nameof(mask));
    return (byte)_MaskBits(rng.Next(), mask);
  }

  public bool Mask1(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(_CountBits(mask), 1, nameof(mask));
    return _MaskBits(rng.Next(), mask) != 0;
  }

  public uint Sponge32() {
    var result = rng.Next();
    result ^= result >> 32;
    return (uint)result;
  }

  public ushort Sponge16() {
    var result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    return (ushort)result;
  }

  public byte Sponge8() {
    var result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    result ^= result >> 8;
    return (byte)result;
  }

  public bool Sponge1() {
    var result = rng.Next();
    result ^= result >> 32; 
    result ^= result >> 16; 
    result ^= result >> 8;  
    result ^= result >> 4;  
    result ^= result >> 2;  
    result ^= result >> 1;  
    return result != 0;
  }

  public ulong Construct(byte bitsTotal, ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(bitsTotal);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitsTotal, 64);
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    var bitsPerRound = _CountBits(mask);
    ArgumentOutOfRangeException.ThrowIfNotEqual(bitsTotal % bitsPerRound,0, nameof(mask));
    var result = 0UL;
    do {
      var random = rng.Next();
      var roundBits = _MaskBits(random, mask);
      result <<= bitsPerRound;
      result |= roundBits;
      bitsTotal -= bitsPerRound;
    } while (bitsTotal > 0);

    return result;
  }

  /// <summary>
  /// A structure used to extract <see cref="uint"/>, <see cref="ushort"/>, and <see cref="byte"/> values directly from a <see cref="ulong"/> 
  /// without performing manual bit shifts and masks. This approach is typically faster, although the compiler (as of 2024) generates
  /// unnecessary stack copies.
  /// </summary>
  /// <param name="value">The <see cref="ulong"/> value from which to extract parts.</param>
  [StructLayout(LayoutKind.Explicit)]
  private readonly struct SliceUnion(ulong value) {

    [FieldOffset(0)]
    public readonly ulong Value = value;

    [FieldOffset(0)]
    public readonly uint R32_0;
    [FieldOffset(4)]
    public readonly uint R32_1;

    [FieldOffset(0)]
    public readonly ushort R16_0;
    [FieldOffset(2)]
    public readonly ushort R16_1;
    [FieldOffset(4)]
    public readonly ushort R16_2;
    [FieldOffset(6)]
    public readonly ushort R16_3;

    [FieldOffset(0)]
    public readonly byte R8_0;
    [FieldOffset(1)]
    public readonly byte R8_1;
    [FieldOffset(2)]
    public readonly byte R8_2;
    [FieldOffset(3)]
    public readonly byte R8_3;
    [FieldOffset(4)]
    public readonly byte R8_4;
    [FieldOffset(5)]
    public readonly byte R8_5;
    [FieldOffset(6)]
    public readonly byte R8_6;
    [FieldOffset(7)]
    public readonly byte R8_7;
    [FieldOffset(8)]
    public readonly byte R8_8;

  }

  public (uint,uint) Slice32x2() {
    var random = new SliceUnion(rng.Next());
    return (
      random.R32_0,
      random.R32_1
    );
  }

  public (ushort, ushort, ushort, ushort) Slice16x4() {
    var random = new SliceUnion(rng.Next());
    return (
      random.R16_0,
      random.R16_1,
      random.R16_2,
      random.R16_3
    );
  }

  public (byte, byte, byte, byte, byte, byte, byte, byte) Slice8x8() {
    var random = new SliceUnion(rng.Next());
    return (
      random.R8_0,
      random.R8_1,
      random.R8_2,
      random.R8_3,
      random.R8_4,
      random.R8_5,
      random.R8_6,
      random.R8_7
    );
  }

  public ulong Modulo(ulong mod) => rng.Next() % mod;
  
  public ulong RejectionSampling(ulong mod) {
    ulong result;
    do 
      result = rng.Next(); 
    while (result >= mod);

    return result;
  }

  public ulong ModuloRejectionSampling(ulong mod) {
    var maxValidRange = ulong.MaxValue - ulong.MaxValue % mod;
    ulong result;
    do 
      result = rng.Next();
    while (result >= maxValidRange);

    return result % mod;
  }

  public double Scale(double scale) => rng.Next() * scale / ulong.MaxValue;

  public float NextSingle() {
    var mantissa = (uint)(rng.Next() >> (64 - 23));
    var floatBits = (127 << 23) | mantissa;
    return BitConverter.Int32BitsToSingle((int)floatBits) - 1.0f;
  }

  public double NextDouble() {
    var mantissa = rng.Next() >> (64 - 52);
    var doubleBits = (1023UL << 52) | mantissa;
    return BitConverter.Int64BitsToDouble((long)doubleBits) - 1.0d;
  }

  /// <summary>
  /// Extracts bits from the specified <paramref name="value"/> according to the bit positions set in the <paramref name="mask"/>.
  /// </summary>
  /// <param name="value">The <see cref="ulong"/> value from which bits are extracted.</param>
  /// <param name="mask">The <see cref="ulong"/> mask that specifies which bits to extract.</param>
  /// <returns>
  /// A <see cref="ulong"/> value containing the bits extracted from <paramref name="value"/> and positioned contiguously
  /// based on the <paramref name="mask"/>.
  /// </returns>
  /// <remarks>
  /// If the CPU supports the BMI2 instruction set, the PEXT (Parallel Bits Extract) instruction is used for optimal performance.
  /// If not, a manual bit-by-bit extraction is performed.
  /// </remarks>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static ulong _MaskBits(ulong value, ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);

    if (Bmi2.X64.IsSupported)
      return Bmi2.X64.ParallelBitExtract(value, mask);

    var result = 0UL;
    var random = value & mask;
    var position = 0;
    do {
      if ((mask & 1) != 0)
        result |= (random & 1) << position++;

      mask >>= 1;
      random >>= 1;
    } while (mask > 0);

    return result;
  }

  /// <summary>
  /// Counts the number of bits set to 1 in the given <see cref="ulong"/> mask.
  /// </summary>
  /// <param name="mask">The <see cref="ulong"/> value in which to count the set bits (1s).</param>
  /// <returns>
  /// The number of bits set to 1 in the <paramref name="mask"/> value.
  /// </returns>
  /// <remarks>
  /// This method uses the <c>POPCNT</c> instruction for efficient bit counting if the CPU supports it.
  /// If the CPU supports the 64-bit <c>POPCNT</c> instruction (<see cref="System.Runtime.Intrinsics.X86.Popcnt.X64"/>),
  /// it is used to count the bits in the entire <see cref="ulong"/> at once.
  /// If only the 32-bit <c>POPCNT</c> instruction (<see cref="System.Runtime.Intrinsics.X86.Popcnt"/>) is supported,
  /// the method counts the bits in the lower and upper 32 bits of the <see cref="ulong"/> separately and sums the results.
  /// If neither instruction is supported, the method falls back to manually counting the bits.
  /// </remarks>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static byte _CountBits(ulong mask) {
    if (Popcnt.X64.IsSupported)
      return (byte)Popcnt.X64.PopCount(mask);

    if (Popcnt.IsSupported)
      return (byte)(Popcnt.PopCount((uint)mask) + Popcnt.PopCount((uint)(mask >> 32)));

    var result = 0UL;
    while(mask > 0) {
      result += mask & 1;
      mask >>= 1;
    }

    return (byte)result;
  }

}