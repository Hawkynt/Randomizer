using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class LinearFeedbackShiftRegister : IRandomNumberGenerator {
  private const ulong POLYNOM = 0b110110010010001001010;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() {
    var result = 0UL;
    for (var i = 0; i < 64; ++i)
      result |= (ulong)StepLFSR() << i;

    return result;

    byte StepLFSR() {
      this._state = ((ulong)CalculateFeedback() << 63) | (this._state >> 1);
      return (byte)(this._state & 1);

      byte CalculateFeedback() {
        var masked = this._state & POLYNOM;
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
