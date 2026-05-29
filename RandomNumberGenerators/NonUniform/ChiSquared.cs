using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Chi-squared distribution with k degrees of freedom. Identical to
///   Gamma(shape = k/2, scale = 2), and used in hypothesis testing
///   (variance tests, goodness-of-fit, contingency tables).
/// </summary>
public class ChiSquared {
  private readonly Gamma _gamma;

  public ChiSquared(ArbitraryNumberGenerator generator, double degreesOfFreedom) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(degreesOfFreedom, 0.0);
    this._gamma = new Gamma(generator, shape: degreesOfFreedom / 2.0, scale: 2.0);
  }

  public double Next() => this._gamma.Next();
}
