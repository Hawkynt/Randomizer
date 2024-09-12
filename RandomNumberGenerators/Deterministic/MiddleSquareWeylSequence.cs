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
    this._state *= this._state;
    this._state += this._weyl += weylConstant;
    return (ulong)(this._state >> 32);
  }
}
