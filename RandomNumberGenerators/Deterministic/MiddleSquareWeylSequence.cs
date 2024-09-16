using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class MiddleSquareWeylSequence(ulong weylConstant = 0xB5AD4ECEDA1CE2A9) : IRandomNumberGenerator {
  private UInt128 _state;
  private UInt128 _weyl;

  public void Seed(ulong seed) {
    this._state = ((UInt128)seed << 64) | ~seed;
    this._weyl = 0;
  }

  public ulong Next() {
    var state = this._state;
    state *= state;
    var weyl = this._weyl += weylConstant;
    state += weyl;
    this._state = state;
    return (ulong)(state >> 32);
  }
}
