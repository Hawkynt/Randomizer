using System;
using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

public class BlumBlumShub : IRandomNumberGenerator {
  
  public BlumBlumShub() : this(18446744073709551559, 30064771079, 8) { }

  public BlumBlumShub(ulong p, ulong q, int bitsPerIteration) {
    if (p % 4 != 3 || q % 4 != 3)
      throw new ArgumentException("Both p and q must be congruent to 3 modulo 4.");

    ArgumentOutOfRangeException.ThrowIfLessThan(bitsPerIteration,1);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitsPerIteration, 64);
    ArgumentOutOfRangeException.ThrowIfNotEqual(64 % bitsPerIteration, 0, nameof(bitsPerIteration));

    this._modulus = (UInt128)p * q;
    this._bitsPerIteration = bitsPerIteration;
    this._mask = (1UL << bitsPerIteration) - 1;
  }

  private readonly int _bitsPerIteration;
  private readonly ulong _mask;

  private UInt128 _state;
  private readonly UInt128 _modulus;
  
  public void Seed(ulong seed) {
    var modulus = this._modulus;
    var state = seed % modulus;

    // Ensure seed is relatively prime to modulus
    while (BigInteger.GreatestCommonDivisor(state, modulus) != 1)
      state = (state + 1) % modulus;

    this._state = state;
  }

  public ulong Next() {
    ulong result = 0;
    
    // extract n bits at a time
    var state = this._state;
    var modulus = this._modulus;
    var mask = this._mask;
    
    for (int i = 0, bitsPerIteration = this._bitsPerIteration; i < 64; i += bitsPerIteration) {
      state = state * state % modulus;
      result |= ((ulong)state & mask) << i;
    }

    this._state = state;

    return result;
  }
}
