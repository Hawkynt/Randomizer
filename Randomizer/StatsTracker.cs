using System;
using System.IO;
using System.Linq;
using Randomizer.Statistics;

namespace Randomizer;

internal class StatsTracker:IValueTracker {

  private const string RANDOGRAM_DIR = "randograms";

  private readonly IValueTracker[] _trackers;

  public StatsTracker(string? name = null) {
    var safeName = name == null ? null : string.Concat(name.Select(c => char.IsLetterOrDigit(c) ? c : '_'));

    this._trackers = [
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
      .._RandogramsFor(safeName),
    ];
  }

  private static IValueTracker[] _RandogramsFor(string? safeName) {
    if (safeName == null)
      return [];

    return Enum.GetValues<Randogram.BitSelectionMethod>()
      .Select(method => (IValueTracker)new Randogram(16, method, new FileInfo(Path.Combine(RANDOGRAM_DIR, $"{safeName}_{method}.png"))))
      .ToArray();
  }
  
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
