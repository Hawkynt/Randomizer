using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Geometric distribution: number of failures before the first success in a
///   sequence of independent Bernoulli(p) trials. Mean = (1-p)/p.
/// </summary>
/// <remarks>
///   Uses the inverse-CDF formula: ⌊log(U) / log(1-p)⌋ — exact and constant-time.
/// </remarks>
public class Geometric {
  private readonly ArbitraryNumberGenerator _generator;
  private readonly double _log1mp;

  public Geometric(ArbitraryNumberGenerator generator, double probability = 0.5) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(probability, 0.0);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(probability, 1.0);
    this._generator = generator;
    this._log1mp = probability >= 1.0 ? double.NegativeInfinity : Math.Log(1.0 - probability);
  }

  public int Next() {
    if (double.IsNegativeInfinity(this._log1mp))
      return 0; // probability == 1 → first trial always succeeds

    var u = this._generator.NextDouble();
    if (u <= 0.0)
      u = double.Epsilon;
    return (int)Math.Floor(Math.Log(u) / this._log1mp);
  }
}
