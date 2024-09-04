using System.Numerics;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class Xoroshiro128PlusPlus : IRandomNumberGenerator {
  private ulong _x;
  private ulong _y;

  public void Seed(ulong seed) {
    this._x = SplitMix64.Next(ref seed);
    this._y = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    var x = this._x;
    var y = this._y;
    var result = BitOperations.RotateLeft(x + y, 17) + x;

    y ^= x;
    this._x = BitOperations.RotateLeft(x, 49) ^ y ^ (y << 21);
    this._y = BitOperations.RotateLeft(y, 28);

    return result;
  }
}
