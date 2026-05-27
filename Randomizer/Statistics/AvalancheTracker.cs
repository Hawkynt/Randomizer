using System;

namespace Randomizer.Statistics;

internal class AvalancheTracker : IValueTracker {
  private readonly ulong[] _distances = new ulong[65];
  private ulong? _lastValue;

  public void Feed(ulong value) {
    if (this._lastValue == null) {
      this._lastValue = value;
      return;
    }

    ++this._distances[(value ^ this._lastValue.Value).CountSetBits()];
    this._lastValue = value;
  }

  public void Print() => HistogramDrawer.DrawHistogram("Successive Difference (Hamming Distance)", 10, this._distances);
}
