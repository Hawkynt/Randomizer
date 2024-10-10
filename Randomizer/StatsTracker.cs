using System;
using Randomizer.Statistics;

namespace Randomizer;

// this class should tracks stats of random number generators:
// tbd: repetition test for n iterations
// tbd: randogram 8x8x8, 4x256x256
internal class StatsTracker:IValueTracker {

  private readonly IValueTracker[] _trackers = [
    new BitIndexHistogram(),
    new BitCountHistogram(),
    new SpacingHistogram(),
    new RepetitionHistogram()
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
