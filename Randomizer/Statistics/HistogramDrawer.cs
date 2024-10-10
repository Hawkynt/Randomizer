using System;
using System.Linq;

namespace Randomizer.Statistics;

internal static class HistogramDrawer {

  public static void DrawHistogram(string title, int height, ulong[] data, ulong[]? additionalData = null, char character = '#') {
    var width = data.Length;
    var outputHeight = additionalData == null ? height : height * 2;
    var output = new char[outputHeight + 1, width];

    // Initialize the array with spaces
    for (var y = 0; y < outputHeight + 1; ++y)
      for (var x = 0; x < width; ++x)
        output[y, x] = ' ';

    // Write the bin index row
    for (var i = 0; i < width; ++i)
      output[height, i] = i % 10 == 0 ? i == 0 ? '0' : (char)('A' - 1 + i / 10) : (char)('0' + i % 10);

    // Find max to normalize
    var max = data.Max();
    if (additionalData != null)
      max = Math.Max(max, additionalData.Max());

    // Calculate and write the histogram bars for data
    for (var i = 0; i < width; ++i) {
      var binHeight = data[i] * (ulong)height / max;
      for (ulong j = 0; j < binHeight; ++j)
        output[height - 1 - (int)j, i] = character;
    }

    // Calculate and write the histogram bars for additionalData if provided
    if (additionalData != null) {
      for (var i = 0; i < width; ++i) {
        var binHeight = additionalData[i] * (ulong)height / max;
        for (ulong j = 0; j < binHeight; ++j)
          output[height + 1 + (int)j, i] = character;
      }
    }

    // Convert the array to a string and print it
    Console.WriteLine($"{title}:");
    for (var i = 0; i < outputHeight + 1; ++i) {
      for (var j = 0; j < width; ++j)
        Console.Write(output[i, j]);

      Console.WriteLine();
    }
  }
}