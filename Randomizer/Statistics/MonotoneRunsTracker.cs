using System;

namespace Randomizer.Statistics;

internal class MonotoneRunsTracker : IValueTracker {
  private const int MAX_RUN_LENGTH = 64;
  private readonly ulong[] _ascendingRuns = new ulong[MAX_RUN_LENGTH];
  private readonly ulong[] _descendingRuns = new ulong[MAX_RUN_LENGTH];
  private ulong? _lastValue;
  private int _currentRunLength;
  private bool _isAscending;
  private bool _hasDirection;

  public void Feed(ulong value) {
    if (this._lastValue == null) {
      this._lastValue = value;
      return;
    }

    var ascending = value >= this._lastValue.Value;

    if (!this._hasDirection) {
      this._isAscending = ascending;
      this._hasDirection = true;
      this._currentRunLength = 1;
    } else if (ascending == this._isAscending) {
      ++this._currentRunLength;
    } else {
      this._RecordRun();
      this._isAscending = ascending;
      this._currentRunLength = 1;
    }

    this._lastValue = value;
  }

  private void _RecordRun() => ++(this._isAscending ? this._ascendingRuns : this._descendingRuns)[Math.Min(this._currentRunLength, MAX_RUN_LENGTH) - 1];

  public void Print() {
    if (this._hasDirection)
      this._RecordRun();

    HistogramDrawer.DrawHistogram("Monotone Runs (ascending/descending)", 10, this._ascendingRuns, this._descendingRuns);
  }
}
