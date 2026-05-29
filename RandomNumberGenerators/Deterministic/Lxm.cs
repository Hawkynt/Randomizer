using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   L64X128MixRandom from Java 17's LXM generator family.
///   Combines a 64-bit LCG with a Xoroshiro128 subgenerator
///   and applies the Lea64 mixing function.
/// </summary>
public class Lxm : IRandomNumberGenerator {
  private const ulong LCG_MULTIPLIER = 0xD1342543DE82EF95;

  private ulong _lcgState;
  private ulong _lcgAddend;
  private ulong _x0, _x1;

  public void Seed(ulong seed) {
    this._lcgAddend = SplitMix64.Next(ref seed) | 1;
    this._lcgState = SplitMix64.Next(ref seed);
    this._x0 = SplitMix64.Next(ref seed);
    this._x1 = SplitMix64.Next(ref seed);
    if (this._x0 == 0 && this._x1 == 0)
      this._x0 = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    var s = this._lcgState;
    var q0 = this._x0;
    var q1 = this._x1;

    // LCG step
    this._lcgState = s * LCG_MULTIPLIER + this._lcgAddend;

    // Xoroshiro128 step
    q1 ^= q0;
    this._x0 = BitOperations.RotateLeft(q0, 24) ^ q1 ^ (q1 << 16);
    this._x1 = BitOperations.RotateLeft(q1, 37);

    // Lea64 mixing of combined output
    return MixLea64(s + q0);
  }

  private static ulong MixLea64(ulong z) {
    z = (z ^ (z >> 32)) * 0xDABA0B6EB09322E3;
    z = (z ^ (z >> 32)) * 0xDABA0B6EB09322E3;
    return z ^ (z >> 32);
  }
}
