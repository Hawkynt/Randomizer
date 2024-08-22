using System;
using System.IO.Enumeration;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Hawkynt.RandomNumberGenerators.Deterministic;

namespace Hawkynt.RandomNumberGenerators.Composites;

partial class ArbitraryNumberGenerator {
  
  public UInt128 Concat128() => (UInt128)rng.Next() << 64 | rng.Next();
  
  public Vector256<ulong> Concat256() => Vector256.Create(rng.Next(), rng.Next(), rng.Next(), rng.Next());
  
  public Vector512<ulong> Concat512() => Vector512.Create(
    rng.Next(), rng.Next(), rng.Next(), rng.Next(),
    rng.Next(), rng.Next(), rng.Next(), rng.Next()
  );

  public UInt128 SplitMix128() {
    var random = rng.Next();
    return (UInt128)random << 64 | SplitMix64.Next(ref random);
  }

  public Vector256<ulong> SplitMix256() {
    var random = rng.Next();
    return Vector256.Create(random, SplitMix64.Next(ref random), SplitMix64.Next(ref random), SplitMix64.Next(ref random));
  }

  public Vector512<ulong> SplitMix512() {
    var random = rng.Next();
    return Vector512.Create(
      random, SplitMix64.Next(ref random), SplitMix64.Next(ref random), SplitMix64.Next(ref random),
      SplitMix64.Next(ref random), SplitMix64.Next(ref random), SplitMix64.Next(ref random), SplitMix64.Next(ref random)
    );
  }

  public UInt128 SpreadBits128(UInt128 mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    var bitCount = _PopCount(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, 64);
    var random = rng.Next();
    var result = UInt128.Zero;
    for (var i = 0; i < bitCount; ++i) {
      UInt128 bit = random & 1;
      random >>= 1;

      var upperZero= BitOperations.TrailingZeroCount((ulong)(mask >> 64));
      var nextPosition = BitOperations.TrailingZeroCount((ulong)mask);
      if (nextPosition == 64)
        nextPosition += upperZero;

      result |= bit << nextPosition;
      mask &= ~(UInt128.One << nextPosition);
    }
    
    return result;
  }

  public Vector256<ulong> SpreadBits256(Vector256<ulong> mask) {
    var bitCount = _PopCount(mask);
    ArgumentOutOfRangeException.ThrowIfZero(bitCount, nameof(mask));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, 64);
    var random = rng.Next();
    var result = Vector256<ulong>.Zero;
    for (var i = 0; i < bitCount; ++i) {
      var bit = random & 1;
      random >>= 1;

      var value0 = mask.GetElement(0);
      var value1 = mask.GetElement(1);
      var value2 = mask.GetElement(2);
      var value3 = mask.GetElement(3);

      var zeroes0 = BitOperations.TrailingZeroCount(value0);
      var zeroes1 = BitOperations.TrailingZeroCount(value1);
      var zeroes2 = BitOperations.TrailingZeroCount(value2);
      var zeroes3 = BitOperations.TrailingZeroCount(value3);

      var nextPosition = zeroes0;
      if (zeroes0 == 64) {
        nextPosition += zeroes1;
        if (zeroes1 == 64) {
          nextPosition += zeroes2;
          if (zeroes2 == 64)
            nextPosition += zeroes3;
        }
      }

      var elementIndex = nextPosition >> 6;
      var intraElementIndex = nextPosition & 63;
      var element = result.GetElement(elementIndex);
      element |= bit << intraElementIndex;
      result = result.WithElement(elementIndex, element);

      mask = mask.WithElement(elementIndex, mask.GetElement(elementIndex) & ~(1UL << intraElementIndex));
    }

    return result;
  }

  public Vector512<ulong> SpreadBits512(Vector512<ulong> mask) {
    var bitCount = _PopCount(mask);
    ArgumentOutOfRangeException.ThrowIfZero(bitCount, nameof(mask));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, 64);
    var random = rng.Next();
    var result = Vector512<ulong>.Zero;
    for (var i = 0; i < bitCount; ++i) {
      var bit = random & 1;
      random >>= 1;

      var nextPosition = 0;
      for (var j = 0; j < 8; ++j) {
        var currentZeroes = BitOperations.TrailingZeroCount(mask.GetElement(j));
        nextPosition += currentZeroes;
        if (currentZeroes != 64)
          break;
      }

      var elementIndex = nextPosition >> 6;
      var intraElementIndex = nextPosition & 63;
      var element = result.GetElement(elementIndex);
      element |= bit << intraElementIndex;
      result = result.WithElement(elementIndex, element);

      mask = mask.WithElement(elementIndex, mask.GetElement(elementIndex) & ~(1UL << intraElementIndex));
    }

    return result;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static byte _PopCount(UInt128 mask) => (byte)(ulong.PopCount((ulong)mask) + ulong.PopCount((ulong)(mask >> 64)));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static ushort _PopCount(Vector256<ulong> mask) {
    var pc0 = ulong.PopCount(mask.GetElement(0));
    var pc1 = ulong.PopCount(mask.GetElement(1));
    var pc2 = ulong.PopCount(mask.GetElement(2));
    var pc3 = ulong.PopCount(mask.GetElement(3));

    var pc01 = pc0 + pc1;
    var pc23 = pc2 + pc3;

    return (ushort)(pc01 + pc23);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static ushort _PopCount(Vector512<ulong> mask) {
    
    var pc0123 = _PopCount(mask.GetLower());
    var pc4567 = _PopCount(mask.GetUpper());

    return (ushort)(pc0123 + pc4567);
  }

}