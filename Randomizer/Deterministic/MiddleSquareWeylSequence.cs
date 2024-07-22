using System;

namespace Randomizer.Deterministic;

public class MiddleSquareWeylSequence : IRandomNumberGenerator {

  private const ulong _WEYL_CONSTANT = 0xB5AD4ECEDA1CE2A9;
  private UInt128 _state;
  private UInt128 _weyl;

  public void Seed(ulong seed) {
    this._state = (UInt128)seed << 64 | ~seed;
    this._weyl = 0;
  }

  public ulong Next() {
    this._state *= this._state;
    this._state += this._weyl += _WEYL_CONSTANT;
    return (ulong)(this._state >> 32);
  }

}
