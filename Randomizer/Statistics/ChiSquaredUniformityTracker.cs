using System;
using System.Linq;

namespace Randomizer.Statistics;

internal class ChiSquaredUniformityTracker : IValueTracker {
  private const int NUM_BINS = 256;
  private const double SCALE_FACTOR = NUM_BINS / (double)ulong.MaxValue;
  private readonly ulong[] _bins = new ulong[NUM_BINS];
  private ulong _count;

  public void Feed(ulong value) {
    ++this._bins[Math.Min((int)(value * SCALE_FACTOR), NUM_BINS - 1)];
    ++this._count;
  }

  public void Print() {
    if (this._count < (ulong)NUM_BINS) {
      Console.WriteLine("Chi-Squared Uniformity: insufficient data");
      return;
    }

    var expected = (double)this._count / NUM_BINS;
    var chiSquared = this._bins.Sum(observed => {
      var diff = observed - expected;
      return diff * diff / expected;
    });

    var df = NUM_BINS - 1;
    Console.WriteLine($"Chi-Squared Uniformity: {chiSquared:F2} (df={df}, expected~{df}, suspicious if >{df + 2 * Math.Sqrt(2 * df):F0})");
  }
}
