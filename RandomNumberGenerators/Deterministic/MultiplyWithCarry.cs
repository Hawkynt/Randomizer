using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class MultiplyWithCarry : IRandomNumberGenerator {

  private ulong _state;
  private ulong _carry;
  private readonly Func<ulong> _generator;
  private readonly ulong _multiplier;
  private readonly ulong _modulo;

  public MultiplyWithCarry(ulong multiplier = 6364136223846793005UL, ulong modulo = 0) {
    this._multiplier = multiplier;
    this._modulo = modulo;
    this._generator = modulo.IsNotSet() ? this._NextImplicitModulo : this._NextWithModulo;
  }

  public void Seed(ulong seed) {
    if (this._modulo.IsNotSet()) {
      this._state = seed;
      this._carry = ~seed;
    } else {
      this._state = seed % this._modulo;
      this._carry = ~seed % this._modulo;
    }
  }

  private ulong _NextImplicitModulo() {
    UInt128 state = this._state;
    state *= this._multiplier;
    state += this._carry;

    this._state = (ulong)state;
    this._carry = (ulong)(state >> 64);

    return this._state;
  }

  private ulong _NextWithModulo() {
    UInt128 state = this._state;
    state *= this._multiplier;
    state += this._carry;

    this._state = (ulong)(state % this._modulo);
    this._carry = (ulong)(state / this._modulo);

    return this._state;
  }

  public ulong Next() => this._generator();

}
