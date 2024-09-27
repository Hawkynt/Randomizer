using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class MiddleSquare : IRandomNumberGenerator {
  
  private UInt128 _state;
  private readonly Func<ulong> _generator;
  private readonly ulong _modulo;

  public MiddleSquare() => this._generator = this._NextWithoutModulo;

  public MiddleSquare(ulong modulo = 0) {
    this._modulo = modulo;
    this._generator = modulo.IsNotSet() ? this._NextWithoutModulo : this._NextWithDivisor;
  }

  public void Seed(ulong seed) => this._state = ((UInt128)seed << 64) | ~seed;
  public ulong Next() => this._generator();
  private ulong _NextWithoutModulo() => (ulong)((this._state *= this._state) >> 32);
  private ulong _NextWithDivisor() => (ulong)((this._state *= this._state) / this._modulo % this._modulo);

}
