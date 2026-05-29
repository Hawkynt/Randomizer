using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   RomuTrio by Mark Overton (2020). A very fast, high-quality PRNG
///   with 192-bit state using rotation-based mixing.
/// </summary>
public class RomuTrio : IRandomNumberGenerator {
  private const ulong MULTIPLIER = 15241094284759029579;
  private ulong _x, _y, _z;

  public void Seed(ulong seed) {
    this._x = SplitMix64.Next(ref seed);
    this._y = SplitMix64.Next(ref seed);
    this._z = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    var xp = this._x;
    var yp = this._y;
    var zp = this._z;

    this._x = MULTIPLIER * zp;
    this._y = BitOperations.RotateLeft(yp - xp, 12);
    this._z = BitOperations.RotateLeft(zp - yp, 44);

    return xp;
  }
}
