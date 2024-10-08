﻿using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class XorShiftPlus : IRandomNumberGenerator {
  private ulong _x, _y;

  public void Seed(ulong seed) {
    this._x = seed == 0 ? 1 : seed;
    this._y = ~seed == 0 ? 1 : ~seed;
  }

  public ulong Next() {
    var x = this._x;
    var y = this._y;

    x ^= x << 23;
    x ^= x >> 17;
    x ^= y ^ (y >> 26);

    this._x = y;
    this._y = x;
    return x + y;
  }
}
