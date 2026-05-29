using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Pareto (Type I) distribution: a heavy-tailed power-law distribution on
///   [<paramref name="scale"/>, ∞). The classic model for the "80/20 rule" and for
///   any quantity (income, file size, city population, web-page hits) whose extremes
///   dominate the average.
/// </summary>
/// <remarks>
///   Uses the inverse-CDF method: x = scale / U^(1/shape).
/// </remarks>
public class Pareto {
  private readonly ArbitraryNumberGenerator _generator;
  private readonly double _shape;
  private readonly double _scale;

  public Pareto(ArbitraryNumberGenerator generator, double shape = 1.0, double scale = 1.0) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(shape, 0.0);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(scale, 0.0);
    this._generator = generator;
    this._shape = shape;
    this._scale = scale;
  }

  public double Next() {
    var u = this._generator.NextDouble();
    if (u <= 0.0)
      u = double.Epsilon;
    return this._scale / Math.Pow(u, 1.0 / this._shape);
  }
}
