using System;
using System.IO;
using System.Linq;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Randomizer.Statistics;

internal static class ImageGenerator {

  private const int SAMPLE_COUNT = 100_000;

  public static void GenerateAll(string outputDir, params (string name, IRandomNumberGenerator gen)[] generators) {
    Directory.CreateDirectory(outputDir);
    foreach (var (name, gen) in generators) {
      gen.Seed(131);
      Console.WriteLine($"Rendering {name}...");

      var bitIndexOnes = new ulong[64];
      var bitIndexZeros = new ulong[64];
      var bitCount = new ulong[65];
      var spacing = new ulong[64];
      var repetition = new ulong[64];
      var hammingDistance = new ulong[65];
      const double scaleFactor = 64.0 / ulong.MaxValue;

      var longestOnes = new ulong[64];
      var longestZeros = new ulong[64];

      ulong? lastValue = null;
      for (var i = 0; i < SAMPLE_COUNT; ++i) {
        var value = gen.Next();

        // Bit index
        for (var b = 0; b < 64; ++b)
          ++((value & (1UL << b)) == 0 ? bitIndexZeros : bitIndexOnes)[b];

        // Bit count (popcount)
        ++bitCount[ulong.PopCount(value)];

        // Repetition
        ++repetition[Math.Min((int)(value * scaleFactor), 63)];

        // Spacing & Hamming
        if (lastValue.HasValue) {
          var delta = value > lastValue.Value ? value - lastValue.Value : lastValue.Value - value;
          ++spacing[Math.Min((int)(delta * scaleFactor), 63)];
          ++hammingDistance[ulong.PopCount(value ^ lastValue.Value)];
        }

        // Longest run (per value: track longest 1-run and 0-run within the 64 bits)
        var (maxOnes, maxZeros) = _RunsIn(value);
        ++longestOnes[Math.Min(maxOnes, 63)];
        ++longestZeros[Math.Min(maxZeros, 63)];

        lastValue = value;
      }

      var safeName = string.Concat(name.Select(c => char.IsLetterOrDigit(c) ? c : '_'));
      HistogramImage.Render($"{name} — Bit Index (1s above, 0s below)", bitIndexOnes, new FileInfo(Path.Combine(outputDir, $"{safeName}_bit_index.png")), bitIndexZeros);
      HistogramImage.Render($"{name} — Bit Count (popcount distribution)", bitCount, new FileInfo(Path.Combine(outputDir, $"{safeName}_bit_count.png")));
      HistogramImage.Render($"{name} — Spacing between consecutive values", spacing, new FileInfo(Path.Combine(outputDir, $"{safeName}_spacing.png")));
      HistogramImage.Render($"{name} — Repetition (value distribution)", repetition, new FileInfo(Path.Combine(outputDir, $"{safeName}_repetition.png")));
      HistogramImage.Render($"{name} — Hamming distance between consecutive outputs", hammingDistance, new FileInfo(Path.Combine(outputDir, $"{safeName}_hamming.png")));
      HistogramImage.Render($"{name} — Longest run (1s above, 0s below)", longestOnes, new FileInfo(Path.Combine(outputDir, $"{safeName}_longest_run.png")), longestZeros);
    }
  }

  private static (int maxOnes, int maxZeros) _RunsIn(ulong value) {
    int maxOnes = 0, maxZeros = 0;
    int currentOnes = 0, currentZeros = 0;
    for (var b = 0; b < 64; ++b) {
      if ((value & (1UL << b)) != 0) {
        ++currentOnes;
        currentZeros = 0;
        if (currentOnes > maxOnes)
          maxOnes = currentOnes;
      } else {
        ++currentZeros;
        currentOnes = 0;
        if (currentZeros > maxZeros)
          maxZeros = currentZeros;
      }
    }
    return (maxOnes, maxZeros);
  }
}
