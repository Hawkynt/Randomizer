using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   Xoshiro128** by Blackman and Vigna — the 32-bit-output sibling of
///   <see cref="Xoshiro256SS"/>, with 128 bits of state (four 32-bit words)
///   and the "**" output scrambler. The intended use case is 32-bit / embedded /
///   GPU code; for 64-bit output we concatenate two 32-bit calls.
/// </summary>
public class Xoshiro128StarStar : IRandomNumberGenerator {
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
    var result = BitOperations.RotateLeft(this._s1 * 5u, 7) * 9u;

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
