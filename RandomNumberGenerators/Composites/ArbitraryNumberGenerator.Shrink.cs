using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Hawkynt.RandomNumberGenerators.Composites;

partial class ArbitraryNumberGenerator {

  public uint Truncate32() => (uint)rng.Next();
  public ushort Truncate16() => (ushort)rng.Next();
  public byte Truncate8() => (byte)rng.Next();
  public bool Truncate1() => (rng.Next() & 1) == 1;

  public uint Shift32() => (uint)(rng.Next() >> 32);
  public ushort Shift16() => (ushort)(rng.Next() >> 48);
  public byte Shift8() => (byte)(rng.Next() >> 56);
  public bool Shift1() => (rng.Next() >> 63) == 1;

  public uint Mask32(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 32UL, nameof(mask));
    return (uint)_ParallelBitExtract(rng.Next(), mask);
  }

  public ushort Mask16(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 16UL, nameof(mask));
    return (ushort)_ParallelBitExtract(rng.Next(), mask);
  }

  public byte Mask8(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 8UL, nameof(mask));
    return (byte)_ParallelBitExtract(rng.Next(), mask);
  }

  public bool Mask1(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 1UL, nameof(mask));
    return _ParallelBitExtract(rng.Next(), mask) != 0;
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
    var bitsPerRound = (byte)BitOperations.PopCount(mask);
    ArgumentOutOfRangeException.ThrowIfNotEqual(bitsTotal % bitsPerRound,0, nameof(mask));
    var result = 0UL;
    do {
      var random = rng.Next();
      var roundBits = _ParallelBitExtract(random, mask);
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
  private static ulong _ParallelBitExtract(ulong value, ulong mask) {
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
  
}