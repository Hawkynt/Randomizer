using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   WyRand from the wyhash library by Wang Yi. An extremely fast counter-based
///   generator using 128-bit multiply mixing. Widely used in hash tables and
///   general-purpose applications.
/// </summary>
public class WyRand : IRandomNumberGenerator {
  private const ulong INCREMENT = 0xA0761D6478BD642F;
  private const ulong SECRET = 0xE7037ED1A0B428DB;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() {
    this._state += INCREMENT;
    return WyMix(this._state, this._state ^ SECRET);
  }

  private static ulong WyMix(ulong a, ulong b) {
    var full = (UInt128)a * b;
    return (ulong)(full >> 64) ^ (ulong)full;
  }
}
