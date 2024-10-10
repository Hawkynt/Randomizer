using System;

namespace Randomizer.Statistics;

internal class BitCountHistogram : IValueTracker {
  private readonly ulong[] _ones = new ulong[65];

  #region Implementation of IValueTracker

  public void Feed(ulong value) => ++this._ones[value.CountSetBits()];

  public void Print() => HistogramDrawer.DrawHistogram("Bit Count Histogram", 10, this._ones);

  #endregion
}
