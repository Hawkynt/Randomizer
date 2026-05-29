using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Cauchy (Lorentz) distribution: a heavy-tailed continuous distribution with
///   undefined mean and variance, parameterised by a location (median) and a
///   half-width-at-half-maximum scale parameter.
/// </summary>
/// <remarks>
///   Uses the inverse-CDF method: x = location + scale · tan(π(U − ½)).
/// </remarks>
public class Cauchy {
  private readonly ArbitraryNumberGenerator _generator;
  private readonly double _location;
  private readonly double _scale;

  public Cauchy(ArbitraryNumberGenerator generator, double location = 0.0, double scale = 1.0) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(scale, 0.0);
    this._generator = generator;
    this._location = location;
    this._scale = scale;
  }

  public double Next() {
    var u = this._generator.NextDouble();
    return this._location + this._scale * Math.Tan(Math.PI * (u - 0.5));
  }
}
