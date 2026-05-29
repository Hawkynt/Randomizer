using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Triangular distribution on [<paramref name="min"/>, <paramref name="max"/>] with
///   mode at <paramref name="mode"/>. Popular in risk modeling and PERT analysis where
///   only an optimistic / most-likely / pessimistic estimate is available.
/// </summary>
/// <remarks>
///   Uses the standard inverse-CDF construction with a single comparison to choose
///   between the rising and falling branches.
/// </remarks>
public class Triangular {
  private readonly ArbitraryNumberGenerator _generator;
  private readonly double _min;
  private readonly double _max;
  private readonly double _mode;
  private readonly double _threshold;

  public Triangular(ArbitraryNumberGenerator generator, double min = 0.0, double mode = 0.5, double max = 1.0) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfLessThan(mode, min);
    ArgumentOutOfRangeException.ThrowIfLessThan(max, mode);
    if (min == max)
      throw new ArgumentException("min must be strictly less than max", nameof(max));
    this._generator = generator;
    this._min = min;
    this._max = max;
    this._mode = mode;
    this._threshold = (mode - min) / (max - min);
  }

  public double Next() {
    var u = this._generator.NextDouble();
    return u < this._threshold
      ? this._min + Math.Sqrt(u * (this._max - this._min) * (this._mode - this._min))
      : this._max - Math.Sqrt((1.0 - u) * (this._max - this._min) * (this._max - this._mode));
  }
}
