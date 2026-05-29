using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   RomuDuo by Mark Overton (2020) — the two-register variant of the Romu family.
///   128-bit state, faster per call than <see cref="RomuTrio"/> at the cost of
///   smaller capacity.
/// </summary>
public class RomuDuo : IRandomNumberGenerator {
  private const ulong MULTIPLIER = 15241094284759029579UL;
  private ulong _x, _y;

  public void Seed(ulong seed) {
    this._x = SplitMix64.Next(ref seed);
    this._y = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    var xp = this._x;
    this._x = MULTIPLIER * this._y;
    this._y = BitOperations.RotateLeft(this._y, 36) + BitOperations.RotateLeft(this._y, 15) - xp;
    return xp;
  }
}
