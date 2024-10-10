using System;

namespace Randomizer.Statistics;

internal class LongestRunHistogram : IValueTracker {
  private const int NumBuckets = 64;
  private readonly ulong[] _onesBuckets = new ulong[NumBuckets];
  private readonly ulong[] _zerosBuckets = new ulong[NumBuckets];

  private bool _isCurrentRunOnes;
  private int _currentRunLength;

  #region Implementation of IValueTracker

  public void Feed(ulong value) {
    for (var i = 0; i < 64; ++i) {
      var isOne = (value & (1UL << i)) != 0;
      if (isOne == this._isCurrentRunOnes) {
        ++this._currentRunLength;
        continue;
      }

      this._UpdateBucketStats();
      this._isCurrentRunOnes = isOne;
      this._currentRunLength = 1;
    }
  }

  private void _UpdateBucketStats() => ++(this._isCurrentRunOnes ? this._onesBuckets : this._zerosBuckets)[Math.Min(this._currentRunLength, NumBuckets - 1)];

  public void Print() {
    this._UpdateBucketStats();
    HistogramDrawer.DrawHistogram("Longest Run Histogram", 10, this._onesBuckets, this._zerosBuckets);
  }

  #endregion
}