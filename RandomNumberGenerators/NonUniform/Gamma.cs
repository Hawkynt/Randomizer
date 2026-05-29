using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Gamma-distributed samples with given shape and scale parameters.
///   Uses Marsaglia-Tsang's squeeze method (2000), which is fast and works
///   uniformly for shape ≥ 1. For shape &lt; 1 we sample with shape + 1 and
///   apply Stuart's correction: G(α) = G(α+1) · U^(1/α).
/// </summary>
public class Gamma {

  private readonly ArbitraryNumberGenerator _generator;
  private readonly double _shape;
  private readonly double _scale;
  private readonly double _effectiveShape;
  private readonly double _d;
  private readonly double _c;
  private readonly bool _correctSmallShape;

  public Gamma(ArbitraryNumberGenerator generator, double shape = 1.0, double scale = 1.0) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(shape, 0.0);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(scale, 0.0);

    this._generator = generator;
    this._shape = shape;
    this._scale = scale;
    this._correctSmallShape = shape < 1.0;
    this._effectiveShape = this._correctSmallShape ? shape + 1.0 : shape;

    this._d = this._effectiveShape - 1.0 / 3.0;
    this._c = 1.0 / Math.Sqrt(9.0 * this._d);
  }

  public double Next() {
    double sample;
    for (;;) {
      double x, v;
      do {
        x = _StandardNormal(this._generator);
        v = 1.0 + this._c * x;
      } while (v <= 0.0);

      v = v * v * v;
      var u = this._generator.NextDouble();
      var x2 = x * x;
      if (u < 1.0 - 0.0331 * x2 * x2) {
        sample = this._d * v;
        break;
      }

      if (Math.Log(u) < 0.5 * x2 + this._d * (1.0 - v + Math.Log(v))) {
        sample = this._d * v;
        break;
      }
    }

    if (this._correctSmallShape) {
      var u = this._generator.NextDouble();
      sample *= Math.Pow(u, 1.0 / this._shape);
    }

    return sample * this._scale;
  }

  // Standard normal via Marsaglia polar method, inlined to avoid a class dependency
  // on MarsagliaPolar (which returns pairs).
  private static double _StandardNormal(ArbitraryNumberGenerator generator) {
    for (;;) {
      var x = 2.0 * generator.NextDouble() - 1.0;
      var y = 2.0 * generator.NextDouble() - 1.0;
      var s = x * x + y * y;
      if (s is <= 0.0 or >= 1.0)
        continue;

      return x * Math.Sqrt(-2.0 * Math.Log(s) / s);
    }
  }
}
