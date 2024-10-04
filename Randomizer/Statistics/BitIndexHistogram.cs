using System;
using System.Collections.Generic;

namespace Randomizer.Statistics;

internal class BitIndexHistogram:IValueTracker {
  private readonly ulong[] _ones=new ulong[64];
  private readonly ulong[] _zeros = new ulong[64];

  #region Implementation of IValueTracker

  public void Feed(ulong value) {
    for (var i = 0; i < 64; i++)
      ++((value & (1UL << i)) == 0 ? this._zeros : this._ones)[i];
  }

  public void Print() {
    const int height = 10;
    const int width = 64;
    var output = new char[height * 2 + 1, width];

    // Initialize the array with spaces
    for (var y = 0; y < height * 2 + 1; ++y)
    for (var x = 0; x < width; ++x)
      output[y, x] = ' ';

    // Write the bit index row
    for (var i = 0; i < width; ++i)
      output[height, i] = i % 10 == 0 ? i == 0 ? '0' : (char)('A' - 1 + i / 10) : (char)('0' + i % 10);

    // Find max to normalize
    var max = Math.Max(this._zeros.Max(), this._ones.Max());
    
    // Calculate and write the histogram bars
    for (var i = 0; i < width; ++i) {
      var onesHeight = this._ones[i] * height/ max;
      var zerosHeight = this._zeros[i] * height / max;

      for (ulong j = 0; j < onesHeight; ++j)
        output[height - 1 - j, i] = '#';

      for (ulong j = 0; j < zerosHeight; ++j)
        output[height + 1 + j, i] = '#';
    }

    // Convert the array to a string and print it
    Console.WriteLine("Bit Index Histogram:");
    for (var i = 0; i < height * 2 + 1; ++i) {
      for (var j = 0; j < width; ++j)
        Console.Write(output[i, j]);

      Console.WriteLine();
    }
  }

  #endregion
}
