using System.Runtime.CompilerServices;

namespace Randomizer.Deterministic;

public class ComplementaryMultiplyWithCarry : IRandomNumberGenerator {
  private static readonly UInt128 A = 6364136223846793005UL;
  private const int R = 4096;
  private readonly ulong[] _state = new ulong[ComplementaryMultiplyWithCarry.R];
  private ulong _carry;
  private int _index = ComplementaryMultiplyWithCarry.R - 1;

  public void Seed(ulong seed) {
    for (var i = 0; i < ComplementaryMultiplyWithCarry.R; ++i)
      this._state[i] = SplitMix64(ref seed);

    this._carry = SplitMix64(ref seed);
    return;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong SplitMix64(ref ulong z) {
      z += 0x9E3779B97F4A7C15;
      z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
      z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
      return z ^= (z >> 31);
    }

  }

  public ulong Next() { // implicit mod 2^64
    this._index = (this._index + 1) % ComplementaryMultiplyWithCarry.R;
    var t = ComplementaryMultiplyWithCarry.A * this._state[this._index] + this._carry;

    this._carry = (ulong)(t >> 64);
    this._state[this._index] = (ulong)t;

    return ulong.MaxValue - this._state[this._index];
  }
}