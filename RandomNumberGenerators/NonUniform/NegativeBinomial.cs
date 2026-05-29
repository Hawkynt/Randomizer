using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Negative Binomial distribution: number of failures observed before the
///   <paramref name="successesRequired"/>-th success in independent Bernoulli(p) trials.
///   Generalises the geometric distribution (which is the special case successes = 1).
/// </summary>
/// <remarks>
///   Implemented by direct simulation. For large <paramref name="successesRequired"/>
///   a Gamma-Poisson mixture is asymptotically faster; we keep the direct version for
///   clarity and correctness.
/// </remarks>
public class NegativeBinomial {
  private readonly ArbitraryNumberGenerator _generator;
  private readonly int _successesRequired;
  private readonly double _probability;

  public NegativeBinomial(ArbitraryNumberGenerator generator, int successesRequired, double probability = 0.5) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(successesRequired);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(probability, 0.0);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(probability, 1.0);

    this._generator = generator;
    this._successesRequired = successesRequired;
    this._probability = probability;
  }

  public int Next() {
    var successes = 0;
    var failures = 0;
    while (successes < this._successesRequired) {
      if (this._generator.NextDouble() < this._probability)
        ++successes;
      else
        ++failures;
    }
    return failures;
  }
}
