using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   MRG32k3a by Pierre L'Ecuyer (1999). A Multiple Recursive Generator
///   combining two parallel third-order recurrences modulo two distinct primes.
///   The default generator in MATLAB, R, SAS, and L'Ecuyer's stochastic simulation library.
/// </summary>
public class Mrg32k3a : IRandomNumberGenerator {
  private const long M1 = 4294967087;
  private const long M2 = 4294944443;
  private const long A12 = 1403580;
  private const long A13N = 810728;
  private const long A21 = 527612;
  private const long A23N = 1370589;

  private long _x10, _x11, _x12;
  private long _x20, _x21, _x22;

  public void Seed(ulong seed) {
    this._x10 = (long)(SplitMix64.Next(ref seed) % (ulong)M1);
    this._x11 = (long)(SplitMix64.Next(ref seed) % (ulong)M1);
    this._x12 = (long)(SplitMix64.Next(ref seed) % (ulong)M1);
    this._x20 = (long)(SplitMix64.Next(ref seed) % (ulong)M2);
    this._x21 = (long)(SplitMix64.Next(ref seed) % (ulong)M2);
    this._x22 = (long)(SplitMix64.Next(ref seed) % (ulong)M2);

    // Avoid degenerate all-zero state
    if (this._x10 == 0 && this._x11 == 0 && this._x12 == 0)
      this._x10 = 1;
    if (this._x20 == 0 && this._x21 == 0 && this._x22 == 0)
      this._x20 = 1;
  }

  public ulong Next() => ((ulong)this._NextInt32() << 32) | this._NextInt32();

  private ulong _NextInt32() {
    var p1 = (A12 * this._x11 - A13N * this._x10) % M1;
    if (p1 < 0)
      p1 += M1;
    this._x10 = this._x11;
    this._x11 = this._x12;
    this._x12 = p1;

    var p2 = (A21 * this._x22 - A23N * this._x20) % M2;
    if (p2 < 0)
      p2 += M2;
    this._x20 = this._x21;
    this._x21 = this._x22;
    this._x22 = p2;

    var z = p1 - p2;
    if (z <= 0)
      z += M1;

    return (ulong)z;
  }
}
