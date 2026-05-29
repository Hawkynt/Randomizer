using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.QuasiRandom;

/// <summary>
///   Sobol' low-discrepancy sequence (Sobol', 1967). Like <see cref="Halton"/>,
///   this is a deterministic space-filling sequence rather than a random one, but
///   it is generated incrementally using Gray-code traversal and a table of
///   precomputed "direction numbers". The 1-dimensional Sobol' sequence used here
///   shares its base-2 mathematics with the van der Corput / base-2 Halton sequence
///   but produces points in a different order, making it convenient for
///   incremental Monte Carlo refinement.
/// </summary>
public class Sobol : IRandomNumberGenerator {
  private readonly ulong[] _direction = new ulong[64];
  private ulong _state;
  private ulong _counter;

  public Sobol() {
    // For dimension 1 the Sobol' direction numbers are the powers of 2,
    // each placed at the highest available bit position. Real multi-dimensional
    // Sobol' requires Joe-Kuo tables of direction numbers — out of scope here.
    for (var i = 0; i < 64; ++i)
      this._direction[i] = 1UL << (63 - i);
  }

  public void Seed(ulong seed) {
    this._state = 0;
    this._counter = 0;
    // Advance to position 'seed' so that two Sobol' instances seeded identically
    // produce the same sequence.
    for (var i = 0UL; i < seed; ++i)
      this.Next();
  }

  public ulong Next() {
    // Gray-code trick: the i-th and (i+1)-th Gray codes differ in exactly one bit,
    // whose position is the trailing-zero count of (i+1). XOR-ing in the direction
    // number for that position incrementally builds the sequence.
    var c = BitOperations.TrailingZeroCount(++this._counter);
    this._state ^= this._direction[c];
    return this._state;
  }
}
