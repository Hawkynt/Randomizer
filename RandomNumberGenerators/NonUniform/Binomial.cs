using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Binomial distribution: count of successes in n independent Bernoulli trials,
///   each with success probability p. Mean = np, variance = np(1-p).
/// </summary>
/// <remarks>
///   For small n (default branch) uses direct simulation: count n Bernoulli draws.
///   For large n × min(p, 1-p) the simulation cost would grow linearly; production
///   code would switch to BTRS or rejection sampling.
/// </remarks>
public class Binomial {
  private readonly ArbitraryNumberGenerator _generator;
  private readonly int _trials;
  private readonly double _probability;

  public Binomial(ArbitraryNumberGenerator generator, int trials, double probability = 0.5) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfNegative(trials);
    ArgumentOutOfRangeException.ThrowIfNegative(probability);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(probability, 1.0);
    this._generator = generator;
    this._trials = trials;
    this._probability = probability;
  }

  public int Next() {
    var successes = 0;
    for (var i = 0; i < this._trials; ++i)
      if (this._generator.NextDouble() < this._probability)
        ++successes;
    return successes;
  }
}
