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
    var states = this._state;
    for (var m = 1; m < states.Length; ++m)
      states[m] += states[m - 1];

    return states[^1];
  }

  private ulong _NextWithModulo() {
    var states = this._state;
    for (var m = 1; m < states.Length; ++m) {
      UInt128 state = states[m];
      state += states[m - 1];
      state %= this._modulo;
      states[m] = (ulong)state;
    }

    return states[^1];
  }

}
