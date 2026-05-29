using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Weibull distribution: a continuous distribution on [0, ∞) widely used in
///   reliability engineering and survival analysis. Shape <paramref name="shape"/> &lt; 1
///   models "infant mortality" (failure rate decreasing with time); shape = 1 reduces
///   to the exponential distribution; shape &gt; 1 models wear-out (failure rate
///   increasing with time).
/// </summary>
/// <remarks>
///   Uses the inverse-CDF method: x = scale · (−log(1 − U))^(1/shape).
/// </remarks>
public class Weibull {
  private readonly ArbitraryNumberGenerator _generator;
  private readonly double _shape;
  private readonly double _scale;

  public Weibull(ArbitraryNumberGenerator generator, double shape = 1.0, double scale = 1.0) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(shape, 0.0);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(scale, 0.0);
    this._generator = generator;
    this._shape = shape;
    this._scale = scale;
  }

  public double Next() {
    var u = this._generator.NextDouble();
    if (u >= 1.0)
      u = 1.0 - double.Epsilon;
    return this._scale * Math.Pow(-Math.Log(1.0 - u), 1.0 / this._shape);
  }
}
