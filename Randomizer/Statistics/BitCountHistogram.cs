using System;
using System.Collections.Generic;

namespace Randomizer.Statistics;

internal class BitCountHistogram : IValueTracker {
  private readonly ulong[] _ones = new ulong[65];

  #region Implementation of IValueTracker

  public void Feed(ulong value) => ++this._ones[value.CountSetBits()];

  public void Print() {
    const int height = 10;
    const int width = 65;
    var output = new char[height + 1, width];

    // Initialize the array with spaces
    for (var y = 0; y < height + 1; ++y)
    for (var x = 0; x < width; ++x)
      output[y, x] = ' ';

    // Write the bit count row
    for (var i = 0; i < width; ++i)
      output[height, i] = i % 10 == 0 ? i == 0 ? '0' : (char)('A' - 1 + i / 10) : (char)('0' + i % 10);

    // Find max to normalize
    var max = this._ones.Max();

    // Calculate and write the histogram bars
    for (var i = 0; i < width; ++i) {
      var onesHeight = this._ones[i] * height / max;

      for (ulong j = 0; j < onesHeight; ++j)
        output[height - 1 - (int)j, i] = '#';
    }

    // Convert the array to a string and print it
    Console.WriteLine("Bit Count Histogram:");
    for (var i = 0; i < height + 1; ++i) {
      for (var j = 0; j < width; ++j)
        Console.Write(output[i, j]);

      Console.WriteLine();
    }
  }

  #endregion
}
