﻿using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class XorShiftStar : IRandomNumberGenerator {
  private const ulong _MULTIPLICATOR = 0x2545F4914F6CDD1D;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed == 0 ? 1 : seed;

  public ulong Next() {
    var s = this._state;
    s ^= s >> 12;
    s ^= s << 25;
    s ^= s >> 27;

    this._state = s;
    return s * _MULTIPLICATOR;
  }
}
