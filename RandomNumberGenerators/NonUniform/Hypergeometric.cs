using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Hypergeometric distribution: count of "success" items drawn when sampling
///   <paramref name="draws"/> items without replacement from a population of
///   <paramref name="populationSize"/> items, <paramref name="successCount"/> of
///   which are successes. The discrete distribution behind the urn problem.
/// </summary>
/// <remarks>
///   Implemented by direct simulation. For huge populations a constant-time
///   algorithm (H2PE) exists; this version is exact at the cost of O(draws) per sample.
/// </remarks>
public class Hypergeometric {
  private readonly ArbitraryNumberGenerator _generator;
  private readonly int _populationSize;
  private readonly int _successCount;
  private readonly int _draws;

  public Hypergeometric(ArbitraryNumberGenerator generator, int populationSize, int successCount, int draws) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentOutOfRangeException.ThrowIfNegative(populationSize);
    ArgumentOutOfRangeException.ThrowIfNegative(successCount);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(successCount, populationSize);
    ArgumentOutOfRangeException.ThrowIfNegative(draws);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(draws, populationSize);

    this._generator = generator;
    this._populationSize = populationSize;
    this._successCount = successCount;
    this._draws = draws;
  }

  public int Next() {
    var remaining = this._populationSize;
    var successesLeft = this._successCount;
    var observed = 0;

    for (var i = 0; i < this._draws; ++i) {
      var probSuccess = (double)successesLeft / remaining;
      if (this._generator.NextDouble() < probSuccess) {
        --successesLeft;
        ++observed;
      }
      --remaining;
    }

    return observed;
  }
}
