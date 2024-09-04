using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class MultiplicativeLinearCongruentialGenerator(ulong multiplier, ulong modulo) : IRandomNumberGenerator {
  private struct MLCGWithoutModulo(ulong multiplier) : IRandomNumberGenerator {
    private ulong _state;
    public void Seed(ulong seed) => this._state = seed;
    public ulong Next() => this._state *= multiplier;
  }

  private struct MLCGWithModulo(ulong multiplier, ulong modulo) : IRandomNumberGenerator {
    private UInt128 _state;
    public void Seed(ulong seed) => this._state = seed % modulo;
    public ulong Next() => (ulong)(this._state = this._state * multiplier % modulo);
  }

  private readonly IRandomNumberGenerator _instance = modulo <= 1
    ? new MLCGWithoutModulo(multiplier)
    : new MLCGWithModulo(multiplier, modulo);

  public MultiplicativeLinearCongruentialGenerator() : this(6364136223846793005, 0) { }

  public void Seed(ulong seed) => this._instance.Seed(seed);

  public ulong Next() => this._instance.Next();
}
