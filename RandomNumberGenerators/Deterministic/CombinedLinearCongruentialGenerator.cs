using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class CombinedLinearCongruentialGenerator(CombinationMode mode, LinearCongruentialGenerator first, LinearCongruentialGenerator second, params LinearCongruentialGenerator[] others) : IRandomNumberGenerator {

  private readonly Func<ulong, ulong, ulong> _combiner = Utils.GetOperation(mode);
  private readonly LinearCongruentialGenerator _first = first ?? throw new ArgumentNullException(nameof(first));
  private readonly LinearCongruentialGenerator _second = second ?? throw new ArgumentNullException(nameof(second));
  private readonly LinearCongruentialGenerator[] _others = others ?? [];

  public CombinedLinearCongruentialGenerator() : this(
    CombinationMode.Additive,
    new(6364136223846793005, 1442695040888963407, 0),
    new(3935559000370003845, 2691343689449507681, 0)
  ) { }

  public CombinedLinearCongruentialGenerator(CombinationMode mode) : this(
    mode,
    new(6364136223846793005, 1442695040888963407, 0),
    new(3935559000370003845, 2691343689449507681, 0)
  ) { }

  public void Seed(ulong seed) {
    this._first.Seed(SplitMix64.Next(ref seed));
    this._second.Seed(SplitMix64.Next(ref seed));
    foreach (var other in this._others)
      other.Seed(SplitMix64.Next(ref seed));
  }

  public ulong Next() {
    // implicit mod 2^64
    var result = this._first.Next();
    var combiner = this._combiner;
    result = combiner(result, this._second.Next());
    foreach (var other in this._others)
      result = combiner(result, other.Next());

    return result;
  }

}
