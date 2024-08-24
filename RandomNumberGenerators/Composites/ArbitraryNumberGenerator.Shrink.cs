using System;
using System.Numerics;
using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global

namespace Hawkynt.RandomNumberGenerators.Composites;

partial class ArbitraryNumberGenerator {

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public uint Truncate32() => (uint)rng.Next();
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ushort Truncate16() => (ushort)rng.Next();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte Truncate8() => (byte)rng.Next();
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Truncate1() => (rng.Next() & 1) == 1;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ulong Truncate(byte bitCount) {
    ArgumentOutOfRangeException.ThrowIfZero(bitCount);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, (byte)63);

    return rng.Next() & ((1UL << bitCount) - 1);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public uint Shift32() => (uint)(rng.Next() >> 32);
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ushort Shift16() => (ushort)(rng.Next() >> 48);
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte Shift8() => (byte)(rng.Next() >> 56);
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Shift1() => (rng.Next() >> 63) == 1;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ulong Shift(byte bitCount) {
    ArgumentOutOfRangeException.ThrowIfZero(bitCount);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, (byte)63);

    return rng.Next() >> (64 - bitCount);
  }
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public uint Mask32(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 32UL, nameof(mask));

    return (uint)_ParallelBitExtract(rng.Next(), mask);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ushort Mask16(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 16UL, nameof(mask));

    return (ushort)_ParallelBitExtract(rng.Next(), mask);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte Mask8(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 8UL, nameof(mask));

    return (byte)_ParallelBitExtract(rng.Next(), mask);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Mask1(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 1UL, nameof(mask));

    return _ParallelBitExtract(rng.Next(), mask) != 0;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ulong Mask(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 63UL, nameof(mask));

    return _ParallelBitExtract(rng.Next(), mask);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public uint Sponge32() {
    var result = rng.Next();
    result ^= result >> 32;
    return (uint)result;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ushort Sponge16() {
    var result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    return (ushort)result;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte Sponge8() {
    var result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    result ^= result >> 8;
    return (byte)result;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte Sponge4() {
    var result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    result ^= result >> 8;
    result ^= result >> 4;
    return (byte)(result & 0xF);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte Sponge2() {
    var result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    result ^= result >> 8;
    result ^= result >> 4;
    result ^= result >> 2;
    return (byte)(result & 0x3);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (uint,uint) Slice32x2() {
    var random = new SliceUnion(rng.Next());
    return (
      random.R32_0,
      random.R32_1
    );
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (ushort, ushort, ushort, ushort) Slice16x4() {
    var random = new SliceUnion(rng.Next());
    return (
      random.R16_0,
      random.R16_1,
      random.R16_2,
      random.R16_3
    );
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ulong Modulo(ulong mod) => rng.Next() % mod;
  
  public ulong RejectionSampling(ulong mod) {
    ulong result;
    do 
      result = rng.Next(); 
    while (result >= mod);

    return result;
  }

  public ulong ModuloRejectionSampling(ulong mod) {
    // Check if mod is a power of 2
    if ((mod & (mod - 1)) == 0)
      // If mod is a power of 2, we can directly return the result of modulo operation
      return rng.Next() & (mod - 1);

    var maxValidRange = ulong.MaxValue - ulong.MaxValue % mod;
    ulong result;
    do 
      result = rng.Next();
    while (result >= maxValidRange);

    return result % mod;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public double Scale(double scale) => rng.Next() * scale / ulong.MaxValue;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public float NextSingle() {
    var mantissa = (uint)(rng.Next() >> (64 - 23));
    var floatBits = (127 << 23) | mantissa;
    return BitConverter.Int32BitsToSingle((int)floatBits) - 1.0f;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public double NextDouble() {
    var mantissa = rng.Next() >> (64 - 52);
    var doubleBits = (1023UL << 52) | mantissa;
    return BitConverter.Int64BitsToDouble((long)doubleBits) - 1.0d;
  }

}