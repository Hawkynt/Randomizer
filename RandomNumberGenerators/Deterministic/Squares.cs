using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   Squares RNG by Bernard Widynski (2022). A counter-based generator
///   using iterated squaring with a Weyl sequence key.
/// </summary>
public class Squares : IRandomNumberGenerator {
  private ulong _counter;
  private ulong _key;

  public void Seed(ulong seed) {
    this._key = SplitMix64.Next(ref seed) | 1;
    this._counter = 0;
  }

  public ulong Next() {
    var counter = this._counter++;
    var key = this._key;

    var y = counter * key;
    var x = y;
    var z = y + key;

    // Round 1
    x = x * x + y;
    x = (x >> 32) | (x << 32);

    // Round 2
    x = x * x + z;
    x = (x >> 32) | (x << 32);

    // Round 3
    x = x * x + y;
    x = (x >> 32) | (x << 32);

    // Round 4
    var t = x = x * x + z;
    x = (x >> 32) | (x << 32);

    // Round 5 (64-bit output)
    return t ^ ((x * x + y) >> 32);
  }
}
