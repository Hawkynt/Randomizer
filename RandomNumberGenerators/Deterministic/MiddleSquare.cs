using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class MiddleSquare : IRandomNumberGenerator {

  private UInt128 _state;

  public void Seed(ulong seed) => this._state = (UInt128)seed << 64 | ~seed;

  public ulong Next() => (ulong)((this._state *= this._state) >> 32);
}
