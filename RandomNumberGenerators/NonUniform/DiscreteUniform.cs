using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Discrete uniform distribution: returns an integer in [<paramref name="min"/>, <paramref name="max"/>]
///   (inclusive on both ends), each value equally likely.
/// </summary>
/// <remarks>
///   Uses unbiased rejection sampling internally so that no value is over- or under-represented
///   even when <c>max - min + 1</c> is not a divisor of 2^64.
/// </remarks>
public class DiscreteUniform {
  private readonly ArbitraryNumberGenerator _generator;
  private readonly long _min;
  private readonly ulong _range;

  public DiscreteUniform(ArbitraryNumberGenerator generator, long min, long max) {
    ArgumentNullException.ThrowIfNull(generator);
    if (min > max)
      throw new ArgumentException("min must be <= max", nameof(max));
    this._generator = generator;
    this._min = min;
    this._range = (ulong)(max - min) + 1;
  }

  public long Next() => this._min + (long)this._generator.ModuloRejectionSampling(this._range);
}
