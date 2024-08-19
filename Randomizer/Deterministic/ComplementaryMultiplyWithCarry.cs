using System;

namespace Randomizer.Deterministic;

public class ComplementaryMultiplyWithCarry : IRandomNumberGenerator {
  private static readonly UInt128 A = 6364136223846793005UL;
  private const int R = 4096;
  private readonly ulong[] _state = new ulong[R];
  private ulong _carry;
  private int _index = R - 1;

  public void Seed(ulong seed) {
    for (var i = 0; i < R; ++i)
      this._state[i] = SplitMix64.Next(ref seed);

    this._carry = SplitMix64.Next(ref seed);
  }

  public ulong Next() { // implicit mod 2^64
    this._index = (this._index + 1) % R;
    var t = A * this._state[this._index] + this._carry;

    this._carry = (ulong)(t >> 64);
    this._state[this._index] = (ulong)t;

    return ulong.MaxValue - this._state[this._index];
  }
}