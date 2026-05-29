using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Student's t-distribution with <paramref name="degreesOfFreedom"/> df.
///   Symmetric around zero, heavier-tailed than the normal for small df, and approaches
///   the standard normal as df → ∞. The workhorse distribution for small-sample
///   hypothesis testing.
/// </summary>
/// <remarks>
///   Uses the canonical construction T = Z / √(V / df) where Z ~ N(0, 1) and V ~ χ²(df).
/// </remarks>
public class StudentT {
  private readonly ArbitraryNumberGenerator _generator;
  private readonly ChiSquared _chi;
  private readonly double _df;

  public StudentT(ArbitraryNumberGenerator generator, double degreesOfFreedom) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(degreesOfFreedom, 0.0);
    this._generator = generator;
    this._df = degreesOfFreedom;
    this._chi = new ChiSquared(generator, degreesOfFreedom);
  }

  public double Next() {
    var z = _StandardNormal(this._generator);
    var v = this._chi.Next();
    return z / Math.Sqrt(v / this._df);
  }

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
