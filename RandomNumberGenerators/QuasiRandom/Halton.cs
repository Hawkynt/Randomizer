using System;
using System.Buffers.Binary;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.QuasiRandom;

/// <summary>
///   Halton low-discrepancy sequence (Halton, 1960). A deterministic, space-filling
///   sequence designed for Monte Carlo integration rather than statistical randomness.
///   The base-2 variant (the default) is identical to the van der Corput sequence and
///   reduces to a simple bit reversal of the counter.
/// </summary>
/// <remarks>
///   <para>
///     A Halton sequence is <i>not</i> a pseudo-random generator: its output is highly
///     correlated and predictable. It will fail statistical-randomness tests by design.
///     The value of low-discrepancy sequences is that successive points fill the unit
///     interval more evenly than uniform-random samples would, which gives Monte Carlo
///     integrators faster convergence (O(log(n)/n) vs O(1/√n)).
///   </para>
/// </remarks>
public class Halton : IRandomNumberGenerator {
  private readonly int _base;
  private ulong _counter;

  public Halton(int @base = 2) {
    ArgumentOutOfRangeException.ThrowIfLessThan(@base, 2);
    this._base = @base;
  }

  public void Seed(ulong seed) => this._counter = seed;

  public ulong Next() {
    var n = ++this._counter;
    return this._base == 2 ? _ReverseBits(n) : _GeneralBase(n);
  }

  private static ulong _ReverseBits(ulong x) {
    x = ((x & 0x5555555555555555UL) << 1) | ((x >> 1) & 0x5555555555555555UL);
    x = ((x & 0x3333333333333333UL) << 2) | ((x >> 2) & 0x3333333333333333UL);
    x = ((x & 0x0F0F0F0F0F0F0F0FUL) << 4) | ((x >> 4) & 0x0F0F0F0F0F0F0F0FUL);
    x = BinaryPrimitives.ReverseEndianness(x);
    return x;
  }

  private ulong _GeneralBase(ulong n) {
    var f = 1.0;
    var r = 0.0;
    var b = (ulong)this._base;
    var i = n;
    while (i > 0) {
      f /= b;
      r += f * (i % b);
      i /= b;
    }
    // Scale [0, 1) to [0, 2^64). Using 2^64 ensures the maximum representable input
    // (very close to 1.0) maps just below ulong.MaxValue.
    return (ulong)(r * 18446744073709551616.0);
  }
}
