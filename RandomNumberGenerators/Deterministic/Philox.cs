using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   Philox2x64-10 counter-based RNG from the Random123 library (Salmon et al., 2011).
///   Widely used in GPU computing (NumPy, TensorFlow, JAX).
/// </summary>
public class Philox : IRandomNumberGenerator {
  private const ulong MULTIPLIER = 0xD2B74407B1CE6E93;
  private const ulong ROUND_KEY_BUMP = 0x9E3779B97F4A7C15;

  private readonly int _rounds;
  private ulong _counter;
  private ulong _key;
  private ulong _buffered;
  private bool _hasBuffered;

  public Philox(int rounds = 10) => this._rounds = rounds;

  public void Seed(ulong seed) {
    this._key = seed;
    this._counter = 0;
    this._hasBuffered = false;
  }

  public ulong Next() {
    if (this._hasBuffered) {
      this._hasBuffered = false;
      return this._buffered;
    }

    var lo = this._counter++;
    var hi = 0UL;
    var roundKey = this._key;

    for (var i = 0; i < this._rounds; ++i) {
      var product = Math.BigMul(lo, MULTIPLIER);
      var newLo = (ulong)(product >> 64) ^ roundKey ^ hi;
      hi = (ulong)product;
      lo = newLo;
      roundKey += ROUND_KEY_BUMP;
    }

    this._buffered = hi;
    this._hasBuffered = true;
    return lo;
  }
}
