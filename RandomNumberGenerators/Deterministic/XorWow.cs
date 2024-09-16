using System.Runtime.CompilerServices;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class XorWow : IRandomNumberGenerator {
  private const uint _WEYL_CONSTANT = 362437;
  private uint _x, _y, _z, _w, _v, _weyl;

  public void Seed(ulong seed) {
    var low = (uint)seed;
    var high = (uint)(seed >> 32);

    var s0 = low ^ 0xAAD26B49;
    var s1 = high ^ 0xF7DCEFDD;
    var t0 = 1099087573 * s0;
    var t1 = 2591861531 * s1;

    this._weyl = 6615241 + t1 + t0;
    this._x = 123456789 + t0;
    this._y = 362436069 ^ t0;
    this._z = 521288629 + t1;
    this._w = 88675123 ^ t1;
    this._v = 5783321 + t0;
  }

  public ulong Next() {
    var high = Next32();
    var low = Next32();
    return ((ulong)high << 32) | low;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    uint Next32() {
      var x = this._x;
      x ^= x >> 2;
      x ^= x << 1;

      (this._x, this._y, this._z, this._w) = (this._y, this._z, this._w, this._v);

      var v = this._v;
      v ^= v << 4;
      v ^= x;
      this._v = v;

      return v + (this._weyl += _WEYL_CONSTANT);
    }
  }
}
