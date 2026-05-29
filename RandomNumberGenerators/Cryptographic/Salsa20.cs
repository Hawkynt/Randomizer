using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   Salsa20 stream cipher by Daniel J. Bernstein, the direct predecessor to
///   <see cref="ChaCha20"/>. Operates on a 512-bit state (16 × 32-bit words)
///   composed of four constants, an 8-word key, a 2-word block counter and a
///   2-word nonce. Each output block runs the state through 20 rounds (alternating
///   four column rounds and four row rounds), adds the original state back and
///   serialises the result. The default round count of 20 is the standard
///   "Salsa20/20" recommendation; 12 and 8 round variants ("Salsa20/12",
///   "Salsa20/8") are also defined for higher speed at lower security margin.
/// </summary>
public class Salsa20 : IRandomNumberGenerator {
  private const int COUNTER_LO = 8;
  private const int COUNTER_HI = 9;
  private const int NONCE_0 = 6;
  private const int NONCE_1 = 7;

  private readonly uint[] _state = new uint[16];
  private readonly int _rounds;

  public Salsa20(int rounds = 20) {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rounds);
    this._rounds = rounds;
  }

  public void Seed(ulong seed) {
    // Standard Salsa20 "expand 32-byte k" constants at positions 0, 5, 10, 15.
    this._state[0] = 0x61707865;
    this._state[5] = 0x3320646E;
    this._state[10] = 0x79622D32;
    this._state[15] = 0x6B206574;

    // Key (8 × 32-bit words at positions 1-4 and 11-14) and nonce (positions 6, 7)
    // derived from the seed via SplitMix64.
    int[] keyIndices = [1, 2, 3, 4, 11, 12, 13, 14];
    foreach (var i in keyIndices) {
      var v = SplitMix64.Next(ref seed);
      this._state[i] = (uint)((v >> 32) ^ v);
    }

    this._state[NONCE_0] = (uint)SplitMix64.Next(ref seed);
    this._state[NONCE_1] = (uint)SplitMix64.Next(ref seed);

    this._state[COUNTER_LO] = 0;
    this._state[COUNTER_HI] = 0;
  }

  public ulong Next() {
    var working = (uint[])this._state.Clone();

    for (var r = 0; r < this._rounds; r += 2) {
      // Column rounds
      QuarterRound(ref working[0], ref working[4], ref working[8], ref working[12]);
      QuarterRound(ref working[5], ref working[9], ref working[13], ref working[1]);
      QuarterRound(ref working[10], ref working[14], ref working[2], ref working[6]);
      QuarterRound(ref working[15], ref working[3], ref working[7], ref working[11]);
      // Row rounds
      QuarterRound(ref working[0], ref working[1], ref working[2], ref working[3]);
      QuarterRound(ref working[5], ref working[6], ref working[7], ref working[4]);
      QuarterRound(ref working[10], ref working[11], ref working[8], ref working[9]);
      QuarterRound(ref working[15], ref working[12], ref working[13], ref working[14]);
    }

    for (var i = 0; i < 16; ++i)
      working[i] += this._state[i];

    // Increment the 64-bit block counter.
    if (++this._state[COUNTER_LO] == 0)
      ++this._state[COUNTER_HI];

    return ((ulong)working[0] << 32) | working[1];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void QuarterRound(ref uint a, ref uint b, ref uint c, ref uint d) {
      b ^= BitOperations.RotateLeft(a + d, 7);
      c ^= BitOperations.RotateLeft(b + a, 9);
      d ^= BitOperations.RotateLeft(c + b, 13);
      a ^= BitOperations.RotateLeft(d + c, 18);
    }
  }
}
