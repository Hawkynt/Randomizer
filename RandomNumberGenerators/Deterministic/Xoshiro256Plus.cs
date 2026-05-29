using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   Xoshiro256+ by Blackman and Vigna. Shares its state-evolution function with
///   <see cref="Xoshiro256SS"/>, but uses the simpler "+" output scrambler
///   (s[0] + s[3]) instead of the "**" multiplication. This is the default
///   PRNG of .NET 6+ <see cref="System.Random"/>; the low three bits are linear
///   so it is intended for generating floating-point values, not for integer
///   work where the lowest bits matter.
/// </summary>
public class Xoshiro256Plus : IRandomNumberGenerator {
  private ulong _w, _x, _y, _z;

  public void Seed(ulong seed) {
    this._w = SplitMix64.Next(ref seed);
    this._x = SplitMix64.Next(ref seed);
    this._y = SplitMix64.Next(ref seed);
    this._z = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    var result = this._w + this._z;

    var x = this._x << 17;

    this._y ^= this._w;
    this._z ^= this._x;
    this._x ^= this._y;
    this._w ^= this._z;

    this._y ^= x;
    this._z = BitOperations.RotateLeft(this._z, 45);

    return result;
  }
}
