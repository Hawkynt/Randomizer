using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class LinearCongruentialGenerator(ulong multiplier, ulong increment, ulong modulo) : IRandomNumberGenerator {
  private struct LCGWithoutModulo(ulong multiplier, ulong increment) : IRandomNumberGenerator {
    private ulong _state;
    public void Seed(ulong seed) => this._state = seed;
    public ulong Next() => this._state = this._state * multiplier + increment;
  }

  private struct LCGWithModulo(ulong multiplier, ulong increment, ulong modulo) : IRandomNumberGenerator {
    private UInt128 _state;
    public void Seed(ulong seed) => this._state = seed % modulo;
    public ulong Next() => (ulong)(this._state = (this._state * multiplier + increment) % modulo);
  }

  private readonly IRandomNumberGenerator _instance = modulo <= 1
    ? new LCGWithoutModulo(multiplier, increment)
    : new LCGWithModulo(multiplier, increment, modulo);

  public LinearCongruentialGenerator() : this(6364136223846793005, 1442695040888963407, 0) { }

  public void Seed(ulong seed) => this._instance.Seed(seed);

  public ulong Next() => this._instance.Next();
}
