using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Bernoulli distribution: returns true with probability <paramref name="probability"/>,
///   false otherwise. The simplest non-uniform distribution and the building block for
///   Binomial, Geometric and most other discrete distributions.
/// </summary>
public class Bernoulli {
  private readonly ArbitraryNumberGenerator _generator;
  private readonly double _probability;

  public Bernoulli(ArbitraryNumberGenerator generator, double probability = 0.5) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfNegative(probability);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(probability, 1.0);
    this._generator = generator;
    this._probability = probability;
  }

  public bool Next() => this._generator.NextBoolean(this._probability);
}
