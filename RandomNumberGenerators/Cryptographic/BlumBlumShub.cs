using System;
using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

public class BlumBlumShub : IRandomNumberGenerator {
  
  public BlumBlumShub() : this(18446744073709551559, 30064771079) { }

  public BlumBlumShub(ulong p, ulong q) {
    if (p % 4 != 3 || q % 4 != 3)
      throw new ArgumentException("Both p and q must be congruent to 3 modulo 4.");

    this._modulus = (UInt128)p * q;
  }

  private UInt128 _state;
  private readonly UInt128 _modulus;
  
  public void Seed(ulong seed) {
    this._state = seed % this._modulus;

    // Ensure seed is relatively prime to modulus
    while (BigInteger.GreatestCommonDivisor(this._state, this._modulus) != 1)
      this._state = (this._state + 1) % this._modulus;
  }

  public ulong Next() {
    ulong result = 0;
    
    // extract 8 bits at a time
    for (var i = 0; i < 64; i += 8) {
      this._state = this._state * this._state % this._modulus;
      result |= (ulong)(this._state & 0xff) << i;
    }

    return result;
  }
}
