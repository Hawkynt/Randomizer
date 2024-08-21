using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class AdditiveCongruentialRandomNumberGenerator : IRandomNumberGenerator {
  private const int K = 12;
  private readonly ulong[] _state = new ulong[K + 1];

  public void Seed(ulong seed) {
    for (var m = 0; m <= K; ++m)
      this._state[m] = SplitMix64.Next(ref seed);
  }

  public ulong Next() { // implicit mod 2^64
    for (var m = 1; m <= K; ++m)
      this._state[m] += this._state[m - 1];

    return this._state[K];
  }

}
