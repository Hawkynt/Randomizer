using System;

namespace Randomizer.Deterministic;

public class MultiplyWithCarry : IRandomNumberGenerator {
  private const ulong A = 6364136223846793005UL;  // Multiplier
  private ulong _state;                           // Current state
  private ulong _carry;                           // Carry value

  public void Seed(ulong seed) {
    _state = seed;
    _carry = ~seed;
  }

  public ulong Next() { // implicit mod 2^64
    UInt128 state = this._state;
    state *= A;
    state += this._carry;

    this._state = (ulong)state;
    this._carry = (ulong)(state >> 64);

    return _state;
  }
}