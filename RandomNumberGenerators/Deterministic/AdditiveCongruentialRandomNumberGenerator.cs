using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class AdditiveCongruentialRandomNumberGenerator : IRandomNumberGenerator {
  private readonly ulong[] _state;
  private readonly Func<ulong> _generator;
  private readonly ulong _modulo;

  public AdditiveCongruentialRandomNumberGenerator(ulong modulo = 0, int k = 12) {
    this._modulo = modulo;
    this._state = new ulong[k + 1];
    this._generator = modulo.IsNotSet() ? this._NextImplicitModulo : this._NextWithModulo;
  }

  public void Seed(ulong seed) {
    if (this._modulo.IsNotSet())
      for (var m = 0; m < this._state.Length; ++m)
        this._state[m] = SplitMix64.Next(ref seed);
    else
      for (var m = 0; m < this._state.Length; ++m)
        this._state[m] = SplitMix64.Next(ref seed) % this._modulo;
  }

  public ulong Next() => this._generator();
  
  private ulong _NextImplicitModulo() {
    for (var m = 1; m < this._state.Length; ++m)
      this._state[m] += this._state[m - 1];

    return this._state[^1];
  }

  private ulong _NextWithModulo() {
    for (var m = 1; m < this._state.Length; ++m) {
      UInt128 state = this._state[m];
      state += this._state[m - 1];
      state %= this._modulo;
      this._state[m] = (ulong)state;
    }

    return this._state[^1];
  }

}
