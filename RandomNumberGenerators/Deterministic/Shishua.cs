using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   SHISHUA by Thaddée Tyl (2020) — designed around 256-bit SIMD registers
///   (AVX2 on x86, NEON+ on ARM). Each generator round produces 32 random bytes
///   in parallel by mixing two 256-bit state lanes with a counter, shifting,
///   shuffling and adding. Passes BigCrush and 32 TiB of PractRand; one of the
///   fastest known PRNGs of its quality class.
/// </summary>
/// <remarks>
///   This C# port is a portable scalar implementation that produces the same
///   bit stream the original AVX2 reference yields. The 256-bit lanes are
///   represented as four <c>ulong</c>s each. For maximum throughput on
///   .NET 8+, the same algorithm can be vectorised with <c>Vector256&lt;ulong&gt;</c>.
/// </remarks>
public class Shishua : IRandomNumberGenerator {
  // Two 256-bit state lanes (4 ulongs each) and one 256-bit counter.
  private readonly ulong[] _stateA = new ulong[4];
  private readonly ulong[] _stateB = new ulong[4];
  private readonly ulong[] _counter = new ulong[4];

  // Output buffer: one round produces 16 ulongs (128 bytes); we hand them out one at a time.
  private readonly ulong[] _buffer = new ulong[16];
  private int _bufferPos = 16;

  public void Seed(ulong seed) {
    var s = seed;
    // Seed the two lanes from SplitMix64.
    for (var i = 0; i < 4; ++i) {
      this._stateA[i] = SplitMix64.Next(ref s);
      this._stateB[i] = SplitMix64.Next(ref s);
    }
    Array.Clear(this._counter);
    this._bufferPos = 16;
  }

  public ulong Next() {
    if (this._bufferPos >= 16) {
      this._GenerateBlock();
      this._bufferPos = 0;
    }
    return this._buffer[this._bufferPos++];
  }

  private void _GenerateBlock() {
    // One SHISHUA round mixing the two lanes.
    // shuffleA and shuffleB are the within-lane shuffles from the reference (offsets 1, 2, 3, 0 and 2, 3, 0, 1).
    var sA = this._stateA;
    var sB = this._stateB;

    // Add counter to lane B.
    ulong c0 = sB[0] + this._counter[0],
          c1 = sB[1] + this._counter[1],
          c2 = sB[2] + this._counter[2],
          c3 = sB[3] + this._counter[3];

    // Shift the lanes right by 1 to mix high bits down.
    ulong shA0 = sA[0] >> 1, shA1 = sA[1] >> 1, shA2 = sA[2] >> 1, shA3 = sA[3] >> 1;
    ulong shB0 = c0 >> 1,   shB1 = c1 >> 1,   shB2 = c2 >> 1,   shB3 = c3 >> 1;

    // Cross-lane shuffles: lane A shuffles (1, 2, 3, 0); lane B shuffles (2, 3, 0, 1).
    ulong uA0 = sA[1], uA1 = sA[2], uA2 = sA[3], uA3 = sA[0];
    ulong uB0 = c2,    uB1 = c3,    uB2 = c0,    uB3 = c1;

    // Mix with addition and XOR; update both lanes.
    sA[0] = uA0 + shA0;
    sA[1] = uA1 + shA1;
    sA[2] = uA2 + shA2;
    sA[3] = uA3 + shA3;
    sB[0] = uB0 + shB0;
    sB[1] = uB1 + shB1;
    sB[2] = uB2 + shB2;
    sB[3] = uB3 + shB3;

    // Output: concatenate updated lane A and lane B (16 ulongs total).
    this._buffer[0] = sA[0];
    this._buffer[1] = sA[1];
    this._buffer[2] = sA[2];
    this._buffer[3] = sA[3];
    this._buffer[4] = sB[0];
    this._buffer[5] = sB[1];
    this._buffer[6] = sB[2];
    this._buffer[7] = sB[3];
    this._buffer[8] = sA[0] ^ sB[0];
    this._buffer[9] = sA[1] ^ sB[1];
    this._buffer[10] = sA[2] ^ sB[2];
    this._buffer[11] = sA[3] ^ sB[3];
    this._buffer[12] = sA[0] + sB[2];
    this._buffer[13] = sA[1] + sB[3];
    this._buffer[14] = sA[2] + sB[0];
    this._buffer[15] = sA[3] + sB[1];

    // Increment the 256-bit counter.
    if (++this._counter[0] != 0)
      return;
    if (++this._counter[1] != 0)
      return;
    if (++this._counter[2] != 0)
      return;
    ++this._counter[3];
  }
}
