using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

public class BlumMicali(ulong p, ulong g) : IRandomNumberGenerator {

  private ulong _state;

  public BlumMicali() : this(6364136223846793005UL, 2147483647) { }

  public void Seed(ulong seed) {
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(seed, 1UL);
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(seed, p - 1);
    this._state = seed;
  }

  public ulong Next() {
    var result = 0UL;
    for (var i = 0; i < 64; ++i)
      result = (result << 1) | (NextBit() ? 1UL : 0);

    return result;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool NextBit()
      => (this._state = (ulong)BigInteger.ModPow(g, this._state, p)) <= (p - 1) / 2
    ;

  }
}