using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   RomuQuad by Mark Overton (2020) — the four-register variant of the Romu family.
///   256-bit state, the largest capacity in the family; recommended when
///   massively-parallel work needs many independent streams (the large state space
///   makes overlap probabilities vanishingly small).
/// </summary>
public class RomuQuad : IRandomNumberGenerator {
  private const ulong MULTIPLIER = 15241094284759029579UL;
  private ulong _w, _x, _y, _z;

  public void Seed(ulong seed) {
    this._w = SplitMix64.Next(ref seed);
    this._x = SplitMix64.Next(ref seed);
    this._y = SplitMix64.Next(ref seed);
    this._z = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    var wp = this._w;
    var xp = this._x;
    var yp = this._y;
    var zp = this._z;

    this._w = MULTIPLIER * zp;
    this._x = zp + BitOperations.RotateLeft(wp, 52);
    this._y = yp - xp;
    this._z = BitOperations.RotateLeft(yp + wp, 19);

    return xp;
  }
}
