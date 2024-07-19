namespace Randomizer.Cryptographic;

public class SelfShrinkingGenerator : IRandomNumberGenerator {
  private const ulong POLYNOM = 0b110110010010001001010;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() {
    var result = 0UL;
    var resultBits = 0;

    while (resultBits < 64) {
      var x = StepLFSR();
      var y = StepLFSR();

      switch (x) {
        case 1 when y == 1:
          result |= (1UL << resultBits);
          ++resultBits;
          break;
        case 1 when y == 0:
          ++resultBits;
          break;
      }
    }

    return result;
    
    byte StepLFSR() {
      this._state = (ulong)CalculateFeedback() << 63 | (this._state >> 1);
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