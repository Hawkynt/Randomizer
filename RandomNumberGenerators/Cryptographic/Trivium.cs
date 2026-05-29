using System;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   Trivium stream cipher (eSTREAM portfolio, 2008). A hardware-oriented
///   lightweight stream cipher with 288-bit state composed of three NLFSRs.
///   Used as a CSPRNG by generating 64 keystream bits per Next() call.
/// </summary>
public class Trivium : IRandomNumberGenerator {
  private const int STATE_SIZE = 288;
  private readonly bool[] _state = new bool[STATE_SIZE];

  public void Seed(ulong seed) {
    Array.Clear(this._state);

    // Derive 80-bit key and 80-bit IV from the 64-bit seed via SplitMix64
    var key = SplitMix64.Next(ref seed);
    var keyHi = SplitMix64.Next(ref seed);
    var iv = SplitMix64.Next(ref seed);
    var ivHi = SplitMix64.Next(ref seed);

    // Load key into positions 1..80 (0-indexed: 0..79)
    for (var i = 0; i < 64; ++i)
      this._state[i] = ((key >> i) & 1) != 0;
    for (var i = 0; i < 16; ++i)
      this._state[64 + i] = ((keyHi >> i) & 1) != 0;

    // Load IV into positions 94..173 (0-indexed: 93..172)
    for (var i = 0; i < 64; ++i)
      this._state[93 + i] = ((iv >> i) & 1) != 0;
    for (var i = 0; i < 16; ++i)
      this._state[93 + 64 + i] = ((ivHi >> i) & 1) != 0;

    // Set the final three bits (positions 286, 287, 288 → 0-indexed 285, 286, 287)
    this._state[285] = true;
    this._state[286] = true;
    this._state[287] = true;

    // Warm-up: 4 * 288 = 1152 cycles discarded
    for (var i = 0; i < 4 * STATE_SIZE; ++i)
      this._StepBit();
  }

  public ulong Next() {
    var result = 0UL;
    for (var i = 0; i < 64; ++i)
      if (this._StepBit())
        result |= 1UL << i;
    return result;
  }

  private bool _StepBit() {
    var s = this._state;

    // 1-indexed positions in the spec, converted to 0-indexed:
    // t1 = s66 ^ s93; t2 = s162 ^ s177; t3 = s243 ^ s288
    var t1 = s[65] ^ s[92];
    var t2 = s[161] ^ s[176];
    var t3 = s[242] ^ s[287];

    var output = t1 ^ t2 ^ t3;

    // t1 ^= s91 & s92 ^ s171
    // t2 ^= s175 & s176 ^ s264
    // t3 ^= s286 & s287 ^ s69
    t1 ^= (s[90] & s[91]) ^ s[170];
    t2 ^= (s[174] & s[175]) ^ s[263];
    t3 ^= (s[285] & s[286]) ^ s[68];

    // Shift the three registers (positions 1..93, 94..177, 178..288) right by one,
    // prepending t3, t1, t2 respectively
    for (var i = 92; i > 0; --i)
      s[i] = s[i - 1];
    s[0] = t3;

    for (var i = 176; i > 93; --i)
      s[i] = s[i - 1];
    s[93] = t1;

    for (var i = 287; i > 177; --i)
      s[i] = s[i - 1];
    s[177] = t2;

    return output;
  }
}
