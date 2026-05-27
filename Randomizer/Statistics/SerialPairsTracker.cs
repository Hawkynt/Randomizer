using System;

namespace Randomizer.Statistics;

internal class SerialPairsTracker : IValueTracker {
  private const int GRID_SIZE = 16;
  private const double SCALE_FACTOR = GRID_SIZE / (double)ulong.MaxValue;
  private readonly ulong[,] _grid = new ulong[GRID_SIZE, GRID_SIZE];
  private ulong _count;
  private ulong? _lastValue;

  public void Feed(ulong value) {
    if (this._lastValue == null) {
      this._lastValue = value;
      return;
    }

    var x = Math.Min((int)(this._lastValue.Value * SCALE_FACTOR), GRID_SIZE - 1);
    var y = Math.Min((int)(value * SCALE_FACTOR), GRID_SIZE - 1);
    ++this._grid[y, x];
    ++this._count;
    this._lastValue = value;
  }

  public void Print() {
    if (this._count < (ulong)(GRID_SIZE * GRID_SIZE)) {
      Console.WriteLine("Serial Pairs Test: insufficient data");
      return;
    }

    var expected = (double)this._count / (GRID_SIZE * GRID_SIZE);
    var chiSquared = 0.0;
    for (var y = 0; y < GRID_SIZE; ++y)
    for (var x = 0; x < GRID_SIZE; ++x) {
      var diff = this._grid[y, x] - expected;
      chiSquared += diff * diff / expected;
    }

    var df = GRID_SIZE * GRID_SIZE - 1;
    Console.WriteLine($"Serial Pairs Test (chi-squared): {chiSquared:F2} (df={df}, expected~{df}, suspicious if >{df + 2 * Math.Sqrt(2 * df):F0})");
  }
}
