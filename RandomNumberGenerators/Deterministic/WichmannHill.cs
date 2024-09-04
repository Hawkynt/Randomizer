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
    this._x = this._x * _MULTIPLIER_X % _MODULUS_X;
    this._y = this._y * _MULTIPLIER_Y % _MODULUS_Y;
    this._z = this._z * _MULTIPLIER_Z % _MODULUS_Z;

    return (ulong)(this._x + this._y + this._z);
  }
}
