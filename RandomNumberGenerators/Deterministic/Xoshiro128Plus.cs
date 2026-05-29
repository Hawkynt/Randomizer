using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   Xoshiro128+ by Blackman and Vigna — the 32-bit + variant designed for
///   generating IEEE-754 floats. The three low-order bits are LFSR-linear, so
///   do not use this for integer work that relies on the low bits.
/// </summary>
public class Xoshiro128Plus : IRandomNumberGenerator {
  private uint _s0, _s1, _s2, _s3;

  public void Seed(ulong seed) {
    var s = seed;
    this._s0 = (uint)SplitMix64.Next(ref s);
    this._s1 = (uint)SplitMix64.Next(ref s);
    this._s2 = (uint)SplitMix64.Next(ref s);
    this._s3 = (uint)SplitMix64.Next(ref s);
  }

  public ulong Next() => ((ulong)Next32() << 32) | Next32();

  private uint Next32() {
    var result = this._s0 + this._s3;

    var t = this._s1 << 9;
    this._s2 ^= this._s0;
    this._s3 ^= this._s1;
    this._s1 ^= this._s2;
    this._s0 ^= this._s3;
    this._s2 ^= t;
    this._s3 = BitOperations.RotateLeft(this._s3, 11);

    return result;
  }
}
