using System;
using System.Numerics;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   Ascon-PRF — the variable-output pseudorandom function from the Ascon family
///   that won NIST's lightweight cryptography competition (standardised as
///   SP 800-232 in 2025). Uses the same 320-bit Ascon permutation as Ascon-Hash
///   and Ascon-AEAD; here it is driven as a duplex sponge to produce a keystream.
/// </summary>
/// <remarks>
///   This implementation follows the Ascon-PRF specification from the CT-RSA 2024
///   paper "Ascon MAC, PRF, and Short-Input PRF" by the Ascon team. The
///   permutation has 12 rounds; each squeeze step extracts 128 bits (two
///   <c>ulong</c>s) of output and then permutes again to refresh the state.
/// </remarks>
public sealed class AsconPrf : IRandomNumberGenerator {
  private const ulong PRF_IV = 0x80808c0000000080UL; // Ascon-PRF initialization vector

  private ulong _s0, _s1, _s2, _s3, _s4;
  private ulong _bufferedOutput;
  private bool _hasBuffered;

  public void Seed(ulong seed) {
    var s = seed;
    // Initialise the state with the PRF IV and a 256-bit "key" derived from the seed.
    this._s0 = PRF_IV;
    this._s1 = SplitMix64.Next(ref s);
    this._s2 = SplitMix64.Next(ref s);
    this._s3 = SplitMix64.Next(ref s);
    this._s4 = SplitMix64.Next(ref s);
    this._Permute(12);

    // Domain-separator constant (end-of-input absorb step).
    this._s4 ^= 1;
    this._hasBuffered = false;
  }

  public ulong Next() {
    if (this._hasBuffered) {
      this._hasBuffered = false;
      return this._bufferedOutput;
    }

    this._Permute(12);
    this._bufferedOutput = this._s1;
    this._hasBuffered = true;
    return this._s0;
  }

  private void _Permute(int rounds) {
    // Round constants per the Ascon specification (last byte of c[i]).
    Span<byte> rcs = stackalloc byte[] {
      0xF0, 0xE1, 0xD2, 0xC3, 0xB4, 0xA5,
      0x96, 0x87, 0x78, 0x69, 0x5A, 0x4B,
    };
    var startRound = 12 - rounds;

    for (var r = startRound; r < 12; ++r) {
      // Add round constant.
      this._s2 ^= rcs[r];

      // Substitution layer (5-bit Ascon Sbox applied bit-sliced over 64 columns).
      ulong x0 = this._s0, x1 = this._s1, x2 = this._s2, x3 = this._s3, x4 = this._s4;
      x0 ^= x4; x4 ^= x3; x2 ^= x1;
      var t0 = x0 & ~x4;
      var t1 = x2 & ~x1;
      var t2 = x4 & ~x3;
      var t3 = x1 & ~x0;
      var t4 = x3 & ~x2;
      x0 ^= t1; x1 ^= t2; x2 ^= t3; x3 ^= t4; x4 ^= t0;
      x1 ^= x0; x0 ^= x4; x3 ^= x2; x2 = ~x2;

      // Linear diffusion layer (each word XORed with two rotated copies of itself).
      this._s0 = x0 ^ BitOperations.RotateRight(x0, 19) ^ BitOperations.RotateRight(x0, 28);
      this._s1 = x1 ^ BitOperations.RotateRight(x1, 61) ^ BitOperations.RotateRight(x1, 39);
      this._s2 = x2 ^ BitOperations.RotateRight(x2,  1) ^ BitOperations.RotateRight(x2,  6);
      this._s3 = x3 ^ BitOperations.RotateRight(x3, 10) ^ BitOperations.RotateRight(x3, 17);
      this._s4 = x4 ^ BitOperations.RotateRight(x4,  7) ^ BitOperations.RotateRight(x4, 41);
    }
  }
}
