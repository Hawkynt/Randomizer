using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class MiddleSquareWeylSequence : IRandomNumberGenerator {
  private UInt128 _state;
  private UInt128 _weyl;
  private readonly Func<ulong> _generator;
  private readonly ulong _modulo;
  private readonly ulong _weylConstant;

  public MiddleSquareWeylSequence(ulong modulo, ulong weylConstant = 0xB5AD4ECEDA1CE2A9) {
    this._modulo = modulo;
    this._weylConstant = weylConstant;
    this._generator = modulo.IsNotSet() ? this._NextWithoutModulo : this._NextWithModulo;
  }

  public MiddleSquareWeylSequence(ulong weylConstant = 0xB5AD4ECEDA1CE2A9) {
    this._weylConstant = weylConstant;
    this._generator = this._NextWithoutModulo;
  }

  public void Seed(ulong seed) {
    this._state = ((UInt128)seed << 64) | ~seed;
    this._weyl = 0;
  }

  public ulong Next() => this._generator();

  private ulong _NextWithoutModulo() {
    var state = this._state;
    state *= state;
    var weyl = this._weyl += this._weylConstant;
    state += weyl;
    this._state = state;
    return (ulong)(state >> 32);
  }

  private ulong _NextWithModulo() {
    var state = this._state;
    state *= state;
    var weyl = this._weyl += this._weylConstant;
    state += weyl;
    this._state = state;
    return (ulong)(state / this._modulo % this._modulo);
  }

}
