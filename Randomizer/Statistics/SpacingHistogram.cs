using System;

namespace Randomizer.Statistics;

internal class SpacingHistogram : IValueTracker {
  private const int NumBins = 64;
  private const double ScaleFactor = NumBins / (double)ulong.MaxValue;

  private readonly ulong[] _bins = new ulong[NumBins];
  private ulong? _lastValue;

  #region Implementation of IValueTracker

  public void Feed(ulong value) {
    if (this._lastValue == null) {
      this._lastValue = value;
      return;
    }

    double delta = value - this._lastValue.Value;
    var spacingBin = (int)(delta.Abs() * ScaleFactor);
    ++this._bins[spacingBin];
  }

  public void Print() => HistogramDrawer.DrawHistogram("Spacing Histogram", 10, this._bins);

  #endregion
}
