using System;
using System.Collections.Generic;

namespace Randomizer.Statistics;

internal class SpacingHistogram : IValueTracker {
  private const int NumBins = 40; // Number of bins for spacing histogram
  private readonly ulong[] _bins = new ulong[NumBins];
  private ulong? _lastValue;

  #region Implementation of IValueTracker

  public void Feed(ulong value) {
    if (this._lastValue == null) {
      this._lastValue = value;
      return;
    }

    double delta = value - this._lastValue.Value;
    var spacingBin = (int)(delta.Abs() / ulong.MaxValue * NumBins);
    ++this._bins[spacingBin];
  }

  public void Print() {
    const int height = 10;
    var output = new char[height + 1, NumBins];

    // Initialize the array with spaces
    for (var y = 0; y < height + 1; ++y)
    for (var x = 0; x < NumBins; ++x)
      output[y, x] = ' ';

    // Write the bin index row
    for (var i = 0; i < NumBins; ++i)
      output[height, i] = i % 10 == 0 ? i == 0 ? '0' : (char)('A' - 1 + i / 10) : (char)('0' + i % 10);

    // Find max to normalize
    var max = this._bins.Max();

    // Calculate and write the histogram bars
    for (var i = 0; i < NumBins; ++i) {
      var binHeight = this._bins[i] * height / max;

      for (ulong j = 0; j < binHeight; ++j)
        output[height - 1 - (int)j, i] = '#';
    }

    // Convert the array to a string and print it
    Console.WriteLine("Spacing Histogram:");
    for (var i = 0; i < height + 1; ++i) {
      for (var j = 0; j < NumBins; ++j)
        Console.Write(output[i, j]);

      Console.WriteLine();
    }
  }

  #endregion
}
