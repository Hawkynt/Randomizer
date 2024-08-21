using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class KeepItSimpleStupid:IRandomNumberGenerator {

  private readonly LinearCongruentialGenerator _lcg = new();
  private readonly XorShift _xs = new();
  private readonly MultiplyWithCarry _mwc = new();

  public void Seed(ulong seed) {
    this._lcg.Seed(seed);
    this._xs.Seed(seed);
    this._mwc.Seed(seed);
  }

  public ulong Next() => this._lcg.Next() ^ this._xs.Next() ^ this._mwc.Next();

}