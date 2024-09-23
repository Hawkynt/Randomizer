using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

public class SelfShrinkingGenerator(ulong polynom) : IRandomNumberGenerator {
  
  public SelfShrinkingGenerator() : this(0b110110010010001001010UL) { }

  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() {
    var result = 0UL;
    var resultBits = 0;

    do {
      var (x, y) = (StepLFSR(), StepLFSR());
      if (x == 0)
        continue;

      result |= (ulong)y << resultBits;
      ++resultBits;
    } while (resultBits < 64);

    return result;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    byte StepLFSR() {
      var state = this._state >> 1;
      this._state = ((ulong)CalculateFeedback() << 63) | state;
      return (byte)(state & 1);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      byte CalculateFeedback() {
        var masked = this._state & polynom;
        if (Popcnt.X64.IsSupported)
          return (byte)(Popcnt.X64.PopCount(masked) & 1);
        
        masked ^= masked >> 32;
        masked ^= masked >> 16;
        masked ^= masked >> 8;
        masked ^= masked >> 4;
        masked ^= masked >> 2;
        masked ^= masked >> 1;
        return (byte)(masked & 1);
      }
    }
  }
}
