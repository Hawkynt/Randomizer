using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   Small Fast Counting 64-bit (sfc64) by Chris Doty-Humphrey.
///   A counter-augmented chaotic generator. Used in NumPy alongside Philox.
/// </summary>
public class Sfc64 : IRandomNumberGenerator {
  private const int BARREL_SHIFT = 24;
  private const int RSHIFT = 11;
  private const int LSHIFT = 3;

  private ulong _a, _b, _c, _counter;

  public void Seed(ulong seed) {
    this._a = seed;
    this._b = seed;
    this._c = seed;
    this._counter = 1;
    for (var i = 0; i < 12; ++i)
      this.Next();
  }

  public ulong Next() {
    var output = this._a + this._b + this._counter++;
    this._a = this._b ^ (this._b >> RSHIFT);
    this._b = this._c + (this._c << LSHIFT);
    this._c = BitOperations.RotateLeft(this._c, BARREL_SHIFT) + output;
    return output;
  }
}
