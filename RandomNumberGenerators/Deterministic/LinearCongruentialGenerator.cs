using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class LinearCongruentialGenerator : IRandomNumberGenerator {

  private ulong _state;
  private readonly Func<ulong> _generator;
  private readonly ulong _multiplier;
  private readonly ulong _increment;
  private readonly ulong _modulo;

  public LinearCongruentialGenerator(ulong multiplier = 6364136223846793005, ulong increment = 1442695040888963407, ulong modulo = 0) {
    this._multiplier = multiplier;
    this._increment = increment;
    this._modulo = modulo;
    this._generator = modulo.IsNotSet() ? this._NextImplicitModulo : this._NextWithModulo;
  }

  public void Seed(ulong seed) => this._state = this._modulo.IsNotSet() ? seed : seed % this._modulo;

  public ulong Next() => this._generator();

  private ulong _NextImplicitModulo() => this._state * this._multiplier + this._increment;

  private ulong _NextWithModulo() {
    UInt128 state = this._state;
    state *= this._multiplier;
    state += this._increment;
    state %= this._modulo;
    return this._state = (ulong)state;

  }

}
