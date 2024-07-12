using System.Runtime.CompilerServices;

namespace Randomizer.Deterministic;

public class Xoshiro256SS : IRandomNumberGenerator {
  private ulong _w, _x, _y, _z;

  public void Seed(ulong seed) {
    this._w = SplitMix64(ref seed);
    this._x = SplitMix64(ref seed);
    this._y = SplitMix64(ref seed);
    this._z = SplitMix64(ref seed);

    return;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong SplitMix64(ref ulong z) {
      z += 0x9E3779B97F4A7C15;
      z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
      z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
      return z ^= (z >> 31);
    }
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
