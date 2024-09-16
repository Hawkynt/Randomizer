using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class WichmannHill : IRandomNumberGenerator {
  private const ulong _MODULUS_X = 18446744073709551557;
  private const ulong _MODULUS_Y = 18446744073709551533;
  private const ulong _MODULUS_Z = 18446744073709551521;
  private const ulong _MULTIPLIER_X = 6364136223846793005;
  private const ulong _MULTIPLIER_Y = 1442695040888963407;
  private const ulong _MULTIPLIER_Z = 1229782938247303441;

  private UInt128 _x, _y, _z;

  public void Seed(ulong seed) {
    var (q, r) = Math.DivRem(seed, _MODULUS_X);
    this._x = r == 0 ? ~r : r;
    (q, r) = Math.DivRem(q, _MODULUS_Y);
    this._y = r == 0 ? ~r : r;
    this._z = q == 0 ? ~q : q;
  }

  public ulong Next() {
    var x = this._x;
    var y = this._y;
    var z = this._z;
    x*=_MULTIPLIER_X;
    y*=_MULTIPLIER_Y;
    z*=_MULTIPLIER_Z;
    x%=_MODULUS_X;
    y%=_MODULUS_Y;
    z%=_MODULUS_Z;
    
    this._x = x;
    this._y = y;
    this._z = z;

    return (ulong)(x + y + z);
  }
}
