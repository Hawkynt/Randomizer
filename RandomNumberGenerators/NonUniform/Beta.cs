using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Beta-distributed samples on (0, 1) with shape parameters <paramref name="alpha"/> and <paramref name="beta"/>.
///   Uses the standard construction Beta(α, β) = X / (X + Y) where X ~ Gamma(α, 1) and Y ~ Gamma(β, 1).
/// </summary>
public class Beta {

  private readonly Gamma _x;
  private readonly Gamma _y;

  public Beta(ArbitraryNumberGenerator generator, double alpha = 1.0, double beta = 1.0) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(alpha, 0.0);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(beta, 0.0);

    this._x = new Gamma(generator, alpha);
    this._y = new Gamma(generator, beta);
  }

  public double Next() {
    var x = this._x.Next();
    var y = this._y.Next();
    return x / (x + y);
  }
}
