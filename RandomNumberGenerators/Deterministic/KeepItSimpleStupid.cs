using System;
using System.ComponentModel;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class KeepItSimpleStupid(KeepItSimpleStupid.Mode mode, IRandomNumberGenerator first, params IRandomNumberGenerator[] others) : IRandomNumberGenerator {

  public enum Mode {
    Additive,
    Subtractive,
    Multiplicative,
    Xor,
  }

  public KeepItSimpleStupid() : this(
    Mode.Xor,
    new LinearCongruentialGenerator(), 
    new XorShift(), 
    new MultiplyWithCarry()
  ) { }

  private readonly Func<ulong, ulong, ulong> _operation= mode switch {
    Mode.Additive => _Additive,
    Mode.Subtractive => _Subtractive,
    Mode.Multiplicative => _Multiplicative,
    Mode.Xor => _Xor,
    _ => throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(Mode))
  };

  public void Seed(ulong seed) {
    first.Seed(seed);
    foreach(var rng in others) 
      rng.Seed(seed);
  }

  public ulong Next() {
    var result= first.Next();
    foreach (var rng in others)
      result=this._operation(result, rng.Next());

    return result;
  }
  
  private static ulong _Additive(ulong a, ulong b) => a + b;
  private static ulong _Subtractive(ulong a, ulong b) => unchecked(a - b);
  private static ulong _Multiplicative(ulong a, ulong b) => a * b;
  private static ulong _Xor(ulong a, ulong b) => a ^ b;
}
