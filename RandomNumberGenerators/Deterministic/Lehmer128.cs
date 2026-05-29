using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   128-bit Lehmer (MCG) generator. Multiplies a 128-bit state by a 64-bit constant
///   and returns the upper 64 bits. Recommended by Steele and Vigna for its speed
///   and statistical quality, distinct from the classical 64-bit MLCG.
/// </summary>
public class Lehmer128 : IRandomNumberGenerator {
  private const ulong MULTIPLIER = 0xDA942042E4DD58B5;
  private UInt128 _state;

  public void Seed(ulong seed) => this._state = ((UInt128)SplitMix64.Next(ref seed) << 64) | SplitMix64.Next(ref seed) | 1;

  public ulong Next() {
    this._state *= MULTIPLIER;
    return (ulong)(this._state >> 64);
  }
}
