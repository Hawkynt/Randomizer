using System;
using Randomizer.Statistics;

namespace Randomizer;

internal class StatsTracker:IValueTracker {

  private readonly IValueTracker[] _trackers = [
    new BitIndexHistogram(),
    new BitCountHistogram(),
    new SpacingHistogram(),
    new RepetitionHistogram(),
    new LongestRunHistogram(),
    new SerialCorrelationTracker(),
    new ChiSquaredUniformityTracker(),
    new MonotoneRunsTracker(),
    new GapTestTracker(),
    new SerialPairsTracker(),
    new BitAutoCorrelationTracker(),
    new AvalancheTracker(),
  ];
  
  public void Feed(ulong value) {
    foreach(var tracker in this._trackers)
      tracker.Feed(value);
  }

  public void Print() {
    foreach (var tracker in this._trackers) {
      tracker.Print();
      Console.WriteLine();
    }
  }
}
