using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Poisson-distributed integer samples with given rate parameter <paramref name="lambda"/>.
///   For small <paramref name="lambda"/> uses Knuth's multiplicative algorithm;
///   for large <paramref name="lambda"/> switches to Atkinson's "PA" rejection method,
///   which keeps the per-sample cost bounded as λ grows.
/// </summary>
public class Poisson {

  private const double SMALL_LAMBDA_THRESHOLD = 30.0;

  private readonly ArbitraryNumberGenerator _generator;
  private readonly double _lambda;

  // Cached values used by both branches.
  private readonly double _expNegLambda;
  private readonly double _c;
  private readonly double _beta;
  private readonly double _alpha;
  private readonly double _k;

  public Poisson(ArbitraryNumberGenerator generator, double lambda = 1.0) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(lambda, 0.0);

    this._generator = generator;
    this._lambda = lambda;

    if (lambda < SMALL_LAMBDA_THRESHOLD) {
      this._expNegLambda = Math.Exp(-lambda);
      return;
    }

    // Atkinson's PA method constants
    this._c = 0.767 - 3.36 / lambda;
    this._beta = Math.PI / Math.Sqrt(3.0 * lambda);
    this._alpha = this._beta * lambda;
    this._k = Math.Log(this._c) - lambda - Math.Log(this._beta);
  }

  public int Next() => this._lambda < SMALL_LAMBDA_THRESHOLD ? this._KnuthSmall() : this._AtkinsonLarge();

  private int _KnuthSmall() {
    var k = 0;
    var p = 1.0;
    do {
      ++k;
      p *= this._generator.NextDouble();
    } while (p > this._expNegLambda);

    return k - 1;
  }

  private int _AtkinsonLarge() {
    for (;;) {
      var u = this._generator.NextDouble();
      var x = (this._alpha - Math.Log((1.0 - u) / u)) / this._beta;
      var n = (int)Math.Floor(x + 0.5);
      if (n < 0)
        continue;

      var v = this._generator.NextDouble();
      var y = this._alpha - this._beta * x;
      var temp = 1.0 + Math.Exp(y);
      var lhs = y + Math.Log(v / (temp * temp));
      var rhs = this._k + n * Math.Log(this._lambda) - _LogFactorial(n);
      if (lhs <= rhs)
        return n;
    }
  }

  private static double _LogFactorial(int n) {
    // Stirling's approximation; accurate enough for the rejection comparison.
    if (n < 2)
      return 0.0;
    var nd = (double)n;
    return nd * Math.Log(nd) - nd + 0.5 * Math.Log(2.0 * Math.PI * nd) + 1.0 / (12.0 * nd);
  }
}
