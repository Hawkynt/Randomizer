using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   TinyMT (Tiny Mersenne Twister) by Saito and Matsumoto (2011).
///   A small-state variant of the Mersenne Twister: 127-bit state, period
///   $2^{127}-1$. Designed for memory-constrained environments where the
///   original MT's ~2.5 KiB state is prohibitive.
/// </summary>
/// <remarks>
///   The "characteristic vector" parameters (mat1, mat2, tmat) come from the
///   reference implementation; alternative tuples produce independent streams
///   with the same period.
/// </remarks>
public class TinyMt : IRandomNumberGenerator {
  private const uint MAT1 = 0x8F7011EE;
  private const uint MAT2 = 0xFC78FF1F;
  private const uint TMAT = 0x3793FDFF;
  private const int MIN_LOOP = 8;
  private const int PRE_LOOP = 8;

  private readonly uint[] _s = new uint[4];

  public void Seed(ulong seed) {
    this._s[0] = (uint)seed;
    this._s[1] = MAT1;
    this._s[2] = MAT2;
    this._s[3] = TMAT;
    for (var i = 1; i < MIN_LOOP; ++i)
      this._s[i & 3] ^= (uint)(i + 1812433253u * (this._s[(i - 1) & 3] ^ (this._s[(i - 1) & 3] >> 30)));

    // The all-zero low-3-bits state is degenerate; tickle it to avoid the trivial cycle.
    if ((this._s[0] & 0x7FFFFFFF) == 0 && this._s[1] == 0 && this._s[2] == 0 && this._s[3] == 0) {
      this._s[0] = (byte)'T';
      this._s[1] = (byte)'I';
      this._s[2] = (byte)'N';
      this._s[3] = (byte)'Y';
    }

    for (var i = 0; i < PRE_LOOP; ++i)
      _Step();
  }

  public ulong Next() => ((ulong)_NextOutput() << 32) | _NextOutput();

  private uint _NextOutput() {
    _Step();

    var t0 = this._s[3];
    var t1 = this._s[0] + (this._s[2] >> 8);
    t0 ^= t1;
    if ((t1 & 1) != 0)
      t0 ^= TMAT;
    return t0;
  }

  private void _Step() {
    var y = this._s[3];
    var x = (this._s[0] & 0x7FFFFFFFu) ^ this._s[1] ^ this._s[2];
    x ^= x << 1;
    y ^= (y >> 1) ^ x;
    this._s[0] = this._s[1];
    this._s[1] = this._s[2];
    this._s[2] = x ^ (y << 10);
    this._s[3] = y;
    if ((y & 1) == 0)
      return;

    this._s[1] ^= MAT1;
    this._s[2] ^= MAT2;
  }
}
