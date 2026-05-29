using System;
using System.Collections.Generic;
using System.Linq;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

/// <summary>
///   Categorical distribution: returns one of N items, each with its own probability.
///   The discrete generalisation of a Bernoulli — used wherever you need a weighted
///   random choice from a finite set (loot tables, mixture models, weighted A/B).
/// </summary>
/// <remarks>
///   Uses cumulative-distribution binary search: O(log n) per sample after O(n) setup.
/// </remarks>
public class Categorical<T> {
  private readonly ArbitraryNumberGenerator _generator;
  private readonly T[] _items;
  private readonly double[] _cumulative;

  public Categorical(ArbitraryNumberGenerator generator, IEnumerable<(T item, double weight)> weighted) {
    ArgumentNullException.ThrowIfNull(generator);
    ArgumentNullException.ThrowIfNull(weighted);

    var arr = weighted.ToArray();
    if (arr.Length == 0)
      throw new ArgumentException("At least one item required", nameof(weighted));
    var totalWeight = arr.Sum(x => x.weight);
    if (totalWeight <= 0.0)
      throw new ArgumentException("Total weight must be positive", nameof(weighted));

    this._generator = generator;
    this._items = arr.Select(x => x.item).ToArray();
    this._cumulative = new double[arr.Length];
    var running = 0.0;
    for (var i = 0; i < arr.Length; ++i) {
      ArgumentOutOfRangeException.ThrowIfNegative(arr[i].weight, "weight");
      running += arr[i].weight / totalWeight;
      this._cumulative[i] = running;
    }
    // Guard against floating-point drift: pin the final CDF entry to exactly 1.0.
    this._cumulative[^1] = 1.0;
  }

  public T Next() {
    var u = this._generator.NextDouble();
    var idx = Array.BinarySearch(this._cumulative, u);
    if (idx < 0)
      idx = ~idx;
    if (idx >= this._items.Length)
      idx = this._items.Length - 1;
    return this._items[idx];
  }
}
