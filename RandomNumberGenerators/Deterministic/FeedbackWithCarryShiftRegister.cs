using System.Runtime.CompilerServices;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class FeedbackWithCarryShiftRegister(ulong polynom = 0b1000_1101__0101_1101__1100_1011__1101_1011__0110_0111__1100_1010__1101_1011__0110_0111UL) : IRandomNumberGenerator {

  private ulong _state;
  private byte _carryBit;

  public void Seed(ulong seed) {
    this._carryBit = (byte)(seed & 1);
    this._state = seed;
  }

  public ulong Next() {
    var qword = 0UL;
    for (var i = 0; i < 64; ++i)
      qword |= (ulong)GetNextBit() << i;

    return qword;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    byte GetNextBit() {
      var feedbackBit = ComputeFeedbackBit();
      var feedbackCarrySum = feedbackBit + this._carryBit;
      this._carryBit = (byte)(feedbackCarrySum >> 1);

      // get one bit out of state
      var result = (byte)(this._state & 1);
      this._state >>= 1;

      // and rotate the feedbackCarrySum in
      this._state |= (ulong)(feedbackCarrySum & 1) << 63;

      return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    byte ComputeFeedbackBit() {
      var result = this._state & polynom;
      result ^= result >> 32;
      result ^= result >> 16;
      result ^= result >> 8;
      result ^= result >> 4;
      result ^= result >> 2;
      result ^= result >> 1;
      return (byte)(result & 1);
    }
  }
}
