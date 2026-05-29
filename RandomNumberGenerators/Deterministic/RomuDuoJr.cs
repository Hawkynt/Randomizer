using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   RomuDuoJr by Mark Overton (2020) — the simplest, fastest Romu variant.
///   128-bit state and only three operations per output. Recommended when speed
///   is the top priority and short streams (no parallel overlap concerns) are sufficient.
/// </summary>
public class RomuDuoJr : IRandomNumberGenerator {
  private const ulong MULTIPLIER = 15241094284759029579UL;
  private ulong _x, _y;

  public void Seed(ulong seed) {
    this._x = SplitMix64.Next(ref seed);
    this._y = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    var xp = this._x;
    this._x = MULTIPLIER * this._y;
    this._y = BitOperations.RotateLeft(this._y - xp, 27);
    return xp;
  }
}
