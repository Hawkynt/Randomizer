using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   ISAAC-64 (Indirection, Shift, Accumulate, Add, and Count) by Bob Jenkins (1996).
///   A cryptographically secure PRNG using array indirection for non-linearity.
/// </summary>
public class Isaac : IRandomNumberGenerator {
  private const int SIZE = 256;
  private const int MASK = SIZE - 1;

  private readonly ulong[] _mem = new ulong[SIZE];
  private readonly ulong[] _results = new ulong[SIZE];
  private ulong _aa, _bb, _cc;
  private int _index;

  public void Seed(ulong seed) {
    this._aa = 0;
    this._bb = 0;
    this._cc = 0;
    this._index = SIZE;

    for (var i = 0; i < SIZE; ++i)
      this._results[i] = SplitMix64.Next(ref seed);

    this._Initialize();
  }

  public ulong Next() {
    if (this._index >= SIZE) {
      this._Generate();
      this._index = 0;
    }

    return this._results[this._index++];
  }

  private void _Initialize() {
    const ulong golden = 0x9E3779B97F4A7C15;
    ulong a = golden, b = golden, c = golden, d = golden, e = golden, f = golden, g = golden, h = golden;

    for (var i = 0; i < 4; ++i)
      _Mix(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h);

    for (var i = 0; i < SIZE; i += 8) {
      a += this._results[i];
      b += this._results[i + 1];
      c += this._results[i + 2];
      d += this._results[i + 3];
      e += this._results[i + 4];
      f += this._results[i + 5];
      g += this._results[i + 6];
      h += this._results[i + 7];
      _Mix(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h);
      this._mem[i] = a;
      this._mem[i + 1] = b;
      this._mem[i + 2] = c;
      this._mem[i + 3] = d;
      this._mem[i + 4] = e;
      this._mem[i + 5] = f;
      this._mem[i + 6] = g;
      this._mem[i + 7] = h;
    }

    for (var i = 0; i < SIZE; i += 8) {
      a += this._mem[i];
      b += this._mem[i + 1];
      c += this._mem[i + 2];
      d += this._mem[i + 3];
      e += this._mem[i + 4];
      f += this._mem[i + 5];
      g += this._mem[i + 6];
      h += this._mem[i + 7];
      _Mix(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h);
      this._mem[i] = a;
      this._mem[i + 1] = b;
      this._mem[i + 2] = c;
      this._mem[i + 3] = d;
      this._mem[i + 4] = e;
      this._mem[i + 5] = f;
      this._mem[i + 6] = g;
      this._mem[i + 7] = h;
    }
  }

  private void _Generate() {
    ++this._cc;
    this._bb += this._cc;

    for (var i = 0; i < SIZE; ++i) {
      var x = this._mem[i];
      this._aa = (i % 4) switch {
        0 => ~(this._aa ^ (this._aa << 21)),
        1 => this._aa ^ (this._aa >> 5),
        2 => this._aa ^ (this._aa << 12),
        _ => this._aa ^ (this._aa >> 33),
      } + this._mem[(i + SIZE / 2) & MASK];

      var y = this._mem[(x >> 3) & MASK] + this._aa + this._bb;
      this._mem[i] = y;
      this._bb = this._mem[(y >> 11) & MASK] + x;
      this._results[i] = this._bb;
    }
  }

  private static void _Mix(ref ulong a, ref ulong b, ref ulong c, ref ulong d, ref ulong e, ref ulong f, ref ulong g, ref ulong h) {
    a -= e; f ^= h >> 9;  h += a;
    b -= f; g ^= a << 9;  a += b;
    c -= g; h ^= b >> 23; b += c;
    d -= h; a ^= c << 15; c += d;
    e -= a; b ^= d >> 14; d += e;
    f -= b; c ^= e << 20; e += f;
    g -= c; d ^= f >> 17; f += g;
    h -= d; e ^= g << 14; g += h;
  }
}
