using System;
using System.Runtime.CompilerServices;

namespace Randomizer.Deterministic ;

public class SubtractWithBorrow : IRandomNumberGenerator {
  private const ulong M = ulong.MaxValue;
  private const int S = 63;
  private const int L = 4093;
  private const int R = 4096;
  private readonly ulong[] _state = new ulong[R];
  private ulong _carry;
  private int _index;

  public void Seed(ulong seed) {
    for (var i = 0; i < R; ++i)
      this._state[i] = SplitMix64(ref seed);

    this._carry = SplitMix64(ref seed);
    this._index = R - 1;
    return;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong SplitMix64(ref ulong z) {
      z += 0x9E3779B97F4A7C15;
      z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
      z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
      return z ^= z >> 31;
    }
  }

  public ulong Next() {
    this._index = (this._index + 1) % R;
    var j = (this._index + R - S) % R;
    var k = (this._index + R - L) % R;

    var t = (Int128)this._state[j] - this._state[k] - this._carry;
    this._carry = t < 0 ? 1UL : 0UL;
    if (t < 0)
      t += M;

    this._state[this._index] = (ulong)t;

    return this._state[this._index];
  }
}