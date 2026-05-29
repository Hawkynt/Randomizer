using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   Xoroshiro128+ by Blackman and Vigna. Shares the state-evolution function with
///   <see cref="Xoroshiro128PlusPlus"/> but uses the simpler "+" output scrambler
///   (s0 + s1) instead of the "++" rotation. The lowest three bits are LFSR-linear
///   so this variant targets generating floating-point doubles, not integer work
///   that depends on the low bits.
/// </summary>
public class Xoroshiro128Plus : IRandomNumberGenerator {
  private ulong _s0, _s1;

  public void Seed(ulong seed) {
    this._s0 = SplitMix64.Next(ref seed);
    this._s1 = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    var s0 = this._s0;
    var s1 = this._s1;
    var result = s0 + s1;

    s1 ^= s0;
    this._s0 = BitOperations.RotateLeft(s0, 24) ^ s1 ^ (s1 << 16);
    this._s1 = BitOperations.RotateLeft(s1, 37);

    return result;
  }
}
