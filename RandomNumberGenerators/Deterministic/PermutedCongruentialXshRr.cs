using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   PCG-XSH-RR with 128-bit state and 64-bit output (an extension of O'Neill's
///   pcg_xsh_rr_64_32 variant to 128-bit state). Shares the LCG core with
///   <see cref="PermutedCongruentialGenerator"/> (RXS-M-XS) and
///   <see cref="PermutedCongruentialXslRr"/>, applying the alternative
///   "XOR-Shift-High, Random-Rotation" output transform: XOR a high-shifted copy
///   of the state into itself to mix high bits down, then take the upper 64 bits
///   and rotate by a count derived from the top 6 bits of the state.
/// </summary>
public class PermutedCongruentialXshRr : IRandomNumberGenerator {
  private UInt128 _state;

  private readonly UInt128 MULTIPLIER = UInt128.Parse("110282366920938463463374607431768211483", NumberStyles.Integer, CultureInfo.InvariantCulture);
  private readonly UInt128 INCREMENT = 1442695040888963407UL;

  public void Seed(ulong seed) => this._state = ((UInt128)seed << 64) | ~seed;

  public ulong Next() {
    var state = this._state * MULTIPLIER + INCREMENT;
    this._state = state;
    return Permute(state);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong Permute(UInt128 state) {
      // XSH: xor a high-shifted copy of the state into itself, then take upper 64 bits.
      var xored = (ulong)((state >> 64) ^ (state >> 35));
      var rotation = (int)(state >> 122);
      return BitOperations.RotateRight(xored, rotation);
    }
  }
}
