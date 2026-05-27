using System;

namespace Randomizer.Statistics;

internal class GapTestTracker : IValueTracker {
  private const int MAX_GAP_LENGTH = 64;
  private static readonly ulong TARGET_THRESHOLD = (ulong)(ulong.MaxValue * 0.125);

  private readonly ulong[] _gaps = new ulong[MAX_GAP_LENGTH];
  private int _currentGap;
  private bool _seenFirstHit;

  public void Feed(ulong value) {
    if (value < TARGET_THRESHOLD) {
      if (this._seenFirstHit)
        ++this._gaps[Math.Min(this._currentGap, MAX_GAP_LENGTH - 1)];

      this._currentGap = 0;
      this._seenFirstHit = true;
    } else if (this._seenFirstHit) {
      ++this._currentGap;
    }
  }

  public void Print() => HistogramDrawer.DrawHistogram("Gap Test (gaps between values in bottom 12.5%)", 10, this._gaps);
}
