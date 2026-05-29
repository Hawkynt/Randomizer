using System;
using System.Collections.Generic;
using System.Linq;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace RandomNumberGenerators.Tests;

[TestFixture]
public class StatisticalTests {

  private const int SAMPLE_COUNT = 100_000;
  private const int BIN_COUNT = 256;

  [TestCaseSource(typeof(Generators.AllGeneratorsSource))]
  public void Output_Is_Not_Constant(IRandomNumberGenerator gen) {
    gen.Seed(Generators.SEED);
    var first = gen.Next();
    var allSame = true;
    for (var i = 0; i < 100; ++i)
      if (gen.Next() != first) {
        allSame = false;
        break;
      }

    Assert.That(allSame, Is.False, "Generator produced constant output");
  }

  [TestCaseSource(typeof(Generators.WellBehavedGeneratorsSource))]
  public void No_Immediate_Repetition_In_First_1000(IRandomNumberGenerator gen) {
    gen.Seed(Generators.SEED);
    var prev = gen.Next();
    var repeats = 0;
    for (var i = 0; i < 999; ++i) {
      var current = gen.Next();
      if (current == prev)
        ++repeats;
      prev = current;
    }

    Assert.That(repeats, Is.LessThan(10), $"Too many consecutive duplicates: {repeats}");
  }

  [TestCaseSource(typeof(Generators.WellBehavedGeneratorsSource))]
  public void Serial_Correlation_Is_Low(IRandomNumberGenerator gen) {
    gen.Seed(Generators.SEED);
    var prev = gen.Next();
    double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0, sumY2 = 0;
    const int n = SAMPLE_COUNT;

    for (var i = 0; i < n; ++i) {
      var current = gen.Next();
      double x = prev, y = current;
      sumX += x;
      sumY += y;
      sumXY += x * y;
      sumX2 += x * x;
      sumY2 += y * y;
      prev = current;
    }

    var numerator = n * sumXY - sumX * sumY;
    var denominator = Math.Sqrt((n * sumX2 - sumX * sumX) * (n * sumY2 - sumY * sumY));
    var correlation = denominator == 0 ? 0 : numerator / denominator;

    Assert.That(Math.Abs(correlation), Is.LessThan(0.05), $"Serial correlation too high: {correlation:F6}");
  }

  [TestCaseSource(typeof(Generators.WellBehavedGeneratorsSource))]
  public void Bit_Distribution_Is_Roughly_Balanced(IRandomNumberGenerator gen) {
    gen.Seed(Generators.SEED);
    var ones = new int[64];

    for (var i = 0; i < SAMPLE_COUNT; ++i) {
      var value = gen.Next();
      for (var bit = 0; bit < 64; ++bit)
        if ((value & (1UL << bit)) != 0)
          ++ones[bit];
    }

    var worstBias = 0.0;
    for (var bit = 0; bit < 64; ++bit) {
      var ratio = (double)ones[bit] / SAMPLE_COUNT;
      var bias = Math.Abs(ratio - 0.5);
      if (bias > worstBias)
        worstBias = bias;
    }

    Assert.That(worstBias, Is.LessThan(0.05), $"Worst bit bias: {worstBias:F4} (expect < 0.05)");
  }

  [TestCaseSource(typeof(Generators.WellBehavedGeneratorsSource))]
  public void Hamming_Distance_Between_Consecutive_Outputs_Is_Centered(IRandomNumberGenerator gen) {
    gen.Seed(Generators.SEED);
    var prev = gen.Next();
    var totalDistance = 0UL;

    for (var i = 0; i < SAMPLE_COUNT; ++i) {
      var current = gen.Next();
      totalDistance += ulong.PopCount(current ^ prev);
      prev = current;
    }

    var avgDistance = (double)totalDistance / SAMPLE_COUNT;
    Assert.That(avgDistance, Is.InRange(28.0, 36.0), $"Average Hamming distance: {avgDistance:F2} (expect ~32)");
  }
}
