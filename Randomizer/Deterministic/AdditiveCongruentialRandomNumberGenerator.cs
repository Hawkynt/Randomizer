using System.Runtime.CompilerServices;

namespace Randomizer.Deterministic;

public class AdditiveCongruentialRandomNumberGenerator : IRandomNumberGenerator {
  private const int K = 12;
  private readonly ulong[] _state = new ulong[AdditiveCongruentialRandomNumberGenerator.K + 1];

  public void Seed(ulong seed) {
    for (var m = 0; m <= AdditiveCongruentialRandomNumberGenerator.K; ++m)
      this._state[m] = SplitMix64(ref seed);
    
    return;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong SplitMix64(ref ulong z) {
      z += 0x9E3779B97F4A7C15;
      z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
      z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
      return z ^= (z >> 31);
    }
  }

  public ulong Next() { // implicit mod 2^64
    for (var m = 1; m <= AdditiveCongruentialRandomNumberGenerator.K; ++m)
      this._state[m] = (this._state[m] + this._state[m - 1]);

    return this._state[AdditiveCongruentialRandomNumberGenerator.K];
  }

}
