using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class KeepItSimpleStupid(CombinationMode mode, IRandomNumberGenerator first, params IRandomNumberGenerator[] others) : IRandomNumberGenerator {

  public KeepItSimpleStupid() : this(
    CombinationMode.Xor,
    new LinearCongruentialGenerator(),
    new XorShift(),
    new MultiplyWithCarry()
  ) { }

  public KeepItSimpleStupid(CombinationMode mode) : this(
    mode,
    new LinearCongruentialGenerator(),
    new XorShift(),
    new MultiplyWithCarry()
  ) { }

  private readonly Func<ulong, ulong, ulong> _operation = Utils.GetOperation(mode);
  private readonly IRandomNumberGenerator _first = first ?? throw new ArgumentNullException(nameof(first));
  private readonly IRandomNumberGenerator[] _others = others ?? [];

  public void Seed(ulong seed) {
    this._first.Seed(seed);
    foreach (var rng in this._others)
      rng.Seed(seed);
  }

  public ulong Next() {
    var result = this._first.Next();
    foreach (var rng in this._others)
      result = this._operation(result, rng.Next());

    return result;
  }

}
