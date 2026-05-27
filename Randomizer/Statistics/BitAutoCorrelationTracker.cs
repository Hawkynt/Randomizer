using System;

namespace Randomizer.Statistics;

internal class BitAutoCorrelationTracker : IValueTracker {
  private static readonly int[] _LAGS = [1, 2, 4, 8, 16, 32];
  private readonly ulong[] _agreements = new ulong[_LAGS.Length];
  private readonly ulong[] _totalPairs = new ulong[_LAGS.Length];

  public void Feed(ulong value) {
    for (var i = 0; i < _LAGS.Length; ++i) {
      var lag = _LAGS[i];
      var bitsToCompare = 64 - lag;
      var mask = bitsToCompare >= 64 ? ulong.MaxValue : (1UL << bitsToCompare) - 1;
      var differing = (ulong)((value ^ (value >> lag)) & mask).CountSetBits();
      this._agreements[i] += (ulong)bitsToCompare - differing;
      this._totalPairs[i] += (ulong)bitsToCompare;
    }
  }

  public void Print() {
    Console.WriteLine("Bit Autocorrelation (intra-value, by lag):");
    for (var i = 0; i < _LAGS.Length; ++i) {
      if (this._totalPairs[i] == 0)
        continue;

      var correlation = (double)this._agreements[i] / this._totalPairs[i] * 2 - 1;
      Console.WriteLine($"  Lag {_LAGS[i],2}: {correlation:+0.000000;-0.000000} (ideal: 0.000000)");
    }
  }
}
