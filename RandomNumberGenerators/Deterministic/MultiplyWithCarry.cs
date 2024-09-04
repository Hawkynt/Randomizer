using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class MultiplyWithCarry : IRandomNumberGenerator {
  private const ulong A = 6364136223846793005UL; // Multiplier
  private ulong _state; // Current state
  private ulong _carry; // Carry value

  public void Seed(ulong seed) {
    this._state = seed;
    this._carry = ~seed;
  }

  public ulong Next() {
    // implicit mod 2^64
    UInt128 state = this._state;
    state *= A;
    state += this._carry;

    this._state = (ulong)state;
    this._carry = (ulong)(state >> 64);

    return this._state;
  }
}
