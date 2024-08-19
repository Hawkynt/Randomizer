using System.Runtime.CompilerServices;

namespace Randomizer.Deterministic;

public class Xoshiro256SS : IRandomNumberGenerator {
  private ulong _w, _x, _y, _z;

  public void Seed(ulong seed) {
    this._w = SplitMix64.Next(ref seed);
    this._x = SplitMix64.Next(ref seed);
    this._y = SplitMix64.Next(ref seed);
    this._z = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    var result = RotateLeft(this._x * 5, 7) * 9;

    var x = this._x << 17;

    this._y ^= this._w;
    this._z ^= this._x;
    this._x ^= this._y;
    this._w ^= this._z;

    this._y ^= x;
    this._z = RotateLeft(this._z, 45);

    return result;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong RotateLeft(ulong x, int k) => (x << k) | (x >> (64 - k));
  }

}
