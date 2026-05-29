using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   Threefry2x64-20 counter-based RNG from the Random123 library (Salmon et al., 2011).
///   Based on the Threefish block cipher reduced for RNG use.
/// </summary>
public class Threefry : IRandomNumberGenerator {
  private const ulong SKEIN_PARITY = 0x1BD11BDAA9FC1A22;
  private static readonly int[] _ROTATIONS = [16, 42, 12, 31, 16, 32, 24, 21];

  private readonly int _rounds;
  private ulong _key0;
  private ulong _key1;
  private ulong _counter;
  private ulong _buffered;
  private bool _hasBuffered;

  public Threefry(int rounds = 20) => this._rounds = rounds;

  public void Seed(ulong seed) {
    this._key0 = seed;
    this._key1 = SplitMix64.Next(ref seed);
    this._counter = 0;
    this._hasBuffered = false;
  }

  public ulong Next() {
    if (this._hasBuffered) {
      this._hasBuffered = false;
      return this._buffered;
    }

    var x0 = this._counter++;
    var x1 = 0UL;

    var ks0 = this._key0;
    var ks1 = this._key1;
    var ks2 = SKEIN_PARITY ^ ks0 ^ ks1;

    x0 += ks0;
    x1 += ks1;

    for (var round = 0; round < this._rounds; ++round) {
      x0 += x1;
      x1 = BitOperations.RotateLeft(x1, _ROTATIONS[round % _ROTATIONS.Length]) ^ x0;

      if ((round + 1) % 4 != 0)
        continue;

      var inject = (round + 1) / 4;
      x0 += (inject % 3) switch { 0 => ks0, 1 => ks1, _ => ks2 };
      x1 += ((inject % 3) switch { 0 => ks1, 1 => ks2, _ => ks0 }) + (ulong)inject;
    }

    this._buffered = x1;
    this._hasBuffered = true;
    return x0;
  }
}
