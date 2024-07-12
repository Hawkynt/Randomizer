using System.Runtime.CompilerServices;

namespace Randomizer.Deterministic;

public class Xoroshiro128PlusPlus : IRandomNumberGenerator {
  private ulong _x;
  private ulong _y;

  public void Seed(ulong seed) {
    this._x = SplitMix64(ref seed);
    this._y = SplitMix64(ref seed);

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
    var x = this._x;
    var y = this._y;
    var result = RotateLeft(x + y, 17) + x;

    y ^= x;
    this._x = RotateLeft(x, 49) ^ y ^ (y << 21);
    this._y = RotateLeft(y, 28);

    return result;

    static ulong RotateLeft(ulong x, int k) => (x << k) | (x >> (64 - k));
  }

}
