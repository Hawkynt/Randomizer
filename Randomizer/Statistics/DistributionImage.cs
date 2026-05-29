using System;
using System.IO;

namespace Randomizer.Statistics;

internal static class DistributionImage {

  private const int DEFAULT_SAMPLES = 200_000;
  private const int DEFAULT_BINS = 80;

  public static void RenderContinuous(
    string title,
    Func<double> sampler,
    FileInfo file,
    int samples = DEFAULT_SAMPLES,
    int bins = DEFAULT_BINS,
    double? min = null,
    double? max = null) {

    var data = new double[samples];
    for (var i = 0; i < samples; ++i)
      data[i] = sampler();

    if (min == null || max == null) {
      // Use a robust 1st-99th percentile range so heavy tails don't squash the body.
      var sorted = (double[])data.Clone();
      Array.Sort(sorted);
      min ??= sorted[samples / 100];
      max ??= sorted[samples - samples / 100 - 1];
    }

    var lo = min.Value;
    var hi = max.Value;
    if (hi <= lo)
      hi = lo + 1;

    var hist = new ulong[bins];
    var binWidth = (hi - lo) / bins;
    for (var i = 0; i < samples; ++i) {
      var bin = (int)Math.Floor((data[i] - lo) / binWidth);
      if (bin >= 0 && bin < bins)
        ++hist[bin];
    }

    HistogramImage.Render(title, hist, file);
  }

  public static void RenderDiscrete(
    string title,
    Func<int> sampler,
    FileInfo file,
    int samples = DEFAULT_SAMPLES,
    int? maxValue = null) {

    var data = new int[samples];
    var observedMax = 0;
    for (var i = 0; i < samples; ++i) {
      data[i] = sampler();
      if (data[i] > observedMax)
        observedMax = data[i];
    }

    var max = maxValue ?? observedMax;
    var hist = new ulong[max + 1];
    for (var i = 0; i < samples; ++i)
      if (data[i] >= 0 && data[i] <= max)
        ++hist[data[i]];

    HistogramImage.Render(title, hist, file);
  }
}
