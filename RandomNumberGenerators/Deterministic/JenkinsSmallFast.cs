using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   Jenkins Small Fast 64-bit (JSF64) by Bob Jenkins.
///   A compact, high-quality PRNG with 256-bit state.
/// </summary>
public class JenkinsSmallFast : IRandomNumberGenerator {
  private ulong _a, _b, _c, _d;

  public void Seed(ulong seed) {
    this._a = 0xF1EA5EED;
    this._b = seed;
    this._c = seed;
    this._d = seed;
    for (var i = 0; i < 20; ++i)
      this.Next();
  }

  public ulong Next() {
    var e = this._a - BitOperations.RotateLeft(this._b, 7);
    this._a = this._b ^ BitOperations.RotateLeft(this._c, 13);
    this._b = this._c + BitOperations.RotateLeft(this._d, 37);
    this._c = this._d + e;
    this._d = e + this._a;
    return this._d;
  }
}
