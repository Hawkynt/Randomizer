using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   Xoshiro512** by Blackman and Vigna — the 512-bit-state sibling of
///   <see cref="Xoshiro256SS"/>. Eight 64-bit state words give a $2^{512}-1$ period;
///   the "**" output scrambler provides excellent statistical quality.
/// </summary>
public class Xoshiro512StarStar : IRandomNumberGenerator {
  private readonly ulong[] _s = new ulong[8];

  public void Seed(ulong seed) {
    for (var i = 0; i < 8; ++i)
      this._s[i] = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    var result = BitOperations.RotateLeft(this._s[1] * 5, 7) * 9;
    _Advance();
    return result;
  }

  private void _Advance() {
    var t = this._s[1] << 11;
    this._s[2] ^= this._s[0];
    this._s[5] ^= this._s[1];
    this._s[1] ^= this._s[2];
    this._s[7] ^= this._s[3];
    this._s[3] ^= this._s[4];
    this._s[4] ^= this._s[5];
    this._s[0] ^= this._s[6];
    this._s[6] ^= this._s[7];
    this._s[6] ^= t;
    this._s[7] = BitOperations.RotateLeft(this._s[7], 21);
  }
}
