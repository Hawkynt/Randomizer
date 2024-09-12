using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class MultiplicativeLinearCongruentialGenerator : IRandomNumberGenerator {
  private ulong _state;
  private readonly Func<ulong> _generator;
  private readonly ulong _multiplier;
  private readonly ulong _modulo;

  public MultiplicativeLinearCongruentialGenerator(ulong multiplier = 6364136223846793005, ulong modulo = 0) {
    this._multiplier = multiplier;
    this._modulo = modulo;
    this._generator = modulo.IsNotSet() ? this._NextImplicitModulo : this._NextWithModulo;
  }

  public void Seed(ulong seed) => this._state = this._modulo.IsNotSet() ? seed : seed % this._modulo;

  public ulong Next() => this._generator();

  private ulong _NextImplicitModulo() => this._state *= this._multiplier;

  private ulong _NextWithModulo() {
    UInt128 state = this._state;
    state *= this._multiplier;
    state %= this._modulo;
    return this._state = (ulong)state;
  }

}
