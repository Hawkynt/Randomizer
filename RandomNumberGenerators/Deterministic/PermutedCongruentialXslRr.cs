using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   PCG-XSL-RR variant of the Permuted Congruential Generator family
///   (O'Neill, 2014). Shares the 128-bit LCG core with the existing
///   <see cref="PermutedCongruentialGenerator"/> (which uses the RXS-M-XS
///   output permutation), but applies the simpler XSL-RR output function:
///   XOR the high half of the state into the low half, then rotate by a count
///   derived from the top bits of the state. XSL-RR is the canonical PCG output
///   transform for 128-bit → 64-bit generators.
/// </summary>
public class PermutedCongruentialXslRr : IRandomNumberGenerator {
  private UInt128 _state;

  private readonly UInt128 MULTIPLIER = UInt128.Parse("110282366920938463463374607431768211483", NumberStyles.Integer, CultureInfo.InvariantCulture);
  private readonly UInt128 INCREMENT = 1442695040888963407UL;

  public void Seed(ulong seed) => this._state = ((UInt128)seed << 64) | ~seed;

  public ulong Next() {
    var state = this._state;
    state = state * MULTIPLIER + INCREMENT;
    this._state = state;
    return Permute(state);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong Permute(UInt128 state) {
      // XSL-RR output: fold the high 64 bits into the low 64 via XOR,
      // then rotate the result by the top 6 bits of the state.
      var rotation = (int)(state >> 122);
      var xored = (ulong)(state >> 64) ^ (ulong)state;
      return BitOperations.RotateRight(xored, rotation);
    }
  }
}
