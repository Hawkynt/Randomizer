using System.Runtime.CompilerServices;

namespace Randomizer.Deterministic;

public class MiddleSquare : IRandomNumberGenerator {

  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() {
    var high = Next32();
    var low = Next32();
    return (ulong)high << 32 | low;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    uint Next32() {
      this._state *= this._state;
      return (uint)(this._state >> 16);
    }
  }

}
