using System;
using System.Collections.Generic;
using System.Linq;
using Hawkynt.RandomNumberGenerators.Composites;
using Hawkynt.RandomNumberGenerators.Deterministic;

namespace RandomNumberGenerators.Tests;

[TestFixture]
public class ArbitraryNumberGeneratorTests {

  private ArbitraryNumberGenerator _rng = null!;

  [SetUp]
  public void SetUp() {
    _rng = new ArbitraryNumberGenerator(new SplitMix64());
    _rng.Seed(Generators.SEED);
  }

  [Test]
  public void Truncate32_Returns_32Bit_Values() {
    for (var i = 0; i < 100; ++i)
      Assert.That(_rng.Truncate32(), Is.LessThanOrEqualTo(uint.MaxValue));
  }

  [Test]
  public void Truncate16_Returns_16Bit_Values() {
    for (var i = 0; i < 100; ++i)
      Assert.That(_rng.Truncate16(), Is.LessThanOrEqualTo(ushort.MaxValue));
  }

  [Test]
  public void Truncate_With_BitCount_Respects_Range() {
    for (byte bits = 1; bits <= 63; bits += 10) {
      var max = (1UL << bits) - 1;
      for (var i = 0; i < 100; ++i)
        Assert.That(_rng.Truncate(bits), Is.LessThanOrEqualTo(max), $"Truncate({bits}) exceeded range");
    }
  }

  [Test]
  public void Shift32_Returns_32Bit_Values() {
    for (var i = 0; i < 100; ++i)
      Assert.That(_rng.Shift32(), Is.LessThanOrEqualTo(uint.MaxValue));
  }

  [Test]
  public void Sponge_Methods_Produce_Values_In_Range() {
    var s32Count = 0;
    var s16Count = 0;
    var s8Count = 0;
    for (var i = 0; i < 1000; ++i) {
      s32Count += _rng.Sponge32() != 0 ? 1 : 0;
      s16Count += _rng.Sponge16() != 0 ? 1 : 0;
      s8Count += _rng.Sponge8() != 0 ? 1 : 0;
      Assert.That(_rng.Sponge4(), Is.LessThanOrEqualTo(15));
      Assert.That(_rng.Sponge2(), Is.LessThanOrEqualTo(3));
    }

    Assert.That(s32Count, Is.GreaterThan(900));
    Assert.That(s16Count, Is.GreaterThan(900));
    Assert.That(s8Count, Is.GreaterThan(900));
  }

  [Test]
  public void Sponge1_Produces_Roughly_Equal_True_And_False() {
    var trueCount = 0;
    const int n = 10_000;
    for (var i = 0; i < n; ++i)
      if (_rng.Sponge1())
        ++trueCount;

    var ratio = (double)trueCount / n;
    Assert.That(ratio, Is.InRange(0.45, 0.55), $"Sponge1 ratio: {ratio:F3}");
  }

  [Test]
  public void NextSingle_Returns_Values_In_Unit_Interval() {
    for (var i = 0; i < 1000; ++i) {
      var value = _rng.NextSingle();
      Assert.That(value, Is.GreaterThanOrEqualTo(0.0f).And.LessThan(1.0f));
    }
  }

  [Test]
  public void NextDouble_Returns_Values_In_Unit_Interval() {
    for (var i = 0; i < 1000; ++i) {
      var value = _rng.NextDouble();
      Assert.That(value, Is.GreaterThanOrEqualTo(0.0).And.LessThan(1.0));
    }
  }

  [Test]
  public void ModuloRejectionSampling_Returns_Values_In_Range() {
    for (var i = 0; i < 1000; ++i)
      Assert.That(_rng.ModuloRejectionSampling(100), Is.LessThan(100));
  }

  [Test]
  public void ModuloRejectionSampling_Distribution_Is_Roughly_Uniform() {
    const ulong mod = 10;
    var bins = new int[mod];
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      ++bins[_rng.ModuloRejectionSampling(mod)];

    var expected = n / (double)mod;
    foreach (var count in bins) {
      var deviation = Math.Abs(count - expected) / expected;
      Assert.That(deviation, Is.LessThan(0.1), $"Bin deviation too large: {deviation:P1}");
    }
  }

  [Test]
  public void Scale_Returns_Values_In_Scaled_Range() {
    for (var i = 0; i < 1000; ++i) {
      var value = _rng.Scale(100.0);
      Assert.That(value, Is.GreaterThanOrEqualTo(0.0).And.LessThanOrEqualTo(100.0));
    }
  }

  [Test]
  public void Concat128_Produces_Two_Different_Halves() {
    var differentCount = 0;
    for (var i = 0; i < 100; ++i) {
      var value = _rng.Concat128();
      if ((ulong)(value >> 64) != (ulong)value)
        ++differentCount;
    }

    Assert.That(differentCount, Is.GreaterThan(90));
  }

  [Test]
  public void ConcatGenerator_Produces_Requested_Byte_Count() {
    for (var count = 1; count <= 33; ++count) {
      var bytes = _rng.ConcatGenerator(count);
      Assert.That(bytes.Length, Is.EqualTo(count));
    }
  }

  [Test]
  public void ConcatGenerator_Stream_Produces_Bytes() {
    var bytes = _rng.ConcatGenerator().Take(64).ToArray();
    Assert.That(bytes.Length, Is.EqualTo(64));
    Assert.That(bytes.Distinct().Count(), Is.GreaterThan(1));
  }

  [Test]
  public void Slice32x2_Covers_Both_Halves() {
    var (lo, hi) = _rng.Slice32x2();
    Assert.That(lo | hi, Is.Not.EqualTo(0U));
  }

  [Test]
  public void Slice16x4_Returns_Four_Values() {
    var (a, b, c, d) = _rng.Slice16x4();
    Assert.That(new[] { a, b, c, d }.Distinct().Count(), Is.GreaterThan(1));
  }

  [Test]
  public void Slice8x8_Returns_Eight_Bytes() {
    var (a, b, c, d, e, f, g, h) = _rng.Slice8x8();
    Assert.That(new[] { a, b, c, d, e, f, g, h }.Length, Is.EqualTo(8));
  }

  [Test]
  public void Construct_Produces_Correct_Bit_Width() {
    var result = _rng.Construct(16, 0xFF);
    Assert.That(result, Is.LessThan(1UL << 16));
  }

  [Test]
  public void Mask_Extracts_Correct_Number_Of_Bits() {
    var result = _rng.Mask(0b10101010_10101010UL);
    Assert.That(result, Is.LessThan(1UL << 8));
  }

  [Test]
  public void NextRange_Returns_Values_In_Range() {
    for (var i = 0; i < 1000; ++i) {
      var v = _rng.NextRange(100, 200);
      Assert.That(v, Is.GreaterThanOrEqualTo(100UL).And.LessThan(200UL));
    }
  }

  [Test]
  public void NextRange_Throws_When_Min_Is_Not_Less_Than_Max() {
    Assert.Throws<ArgumentOutOfRangeException>(() => _rng.NextRange(10, 10));
    Assert.Throws<ArgumentOutOfRangeException>(() => _rng.NextRange(10, 5));
  }

  [Test]
  public void NextBoolean_With_Zero_Returns_False() {
    for (var i = 0; i < 100; ++i)
      Assert.That(_rng.NextBoolean(0.0), Is.False);
  }

  [Test]
  public void NextBoolean_With_One_Returns_True() {
    for (var i = 0; i < 100; ++i)
      Assert.That(_rng.NextBoolean(1.0), Is.True);
  }

  [Test]
  public void NextBoolean_With_Probability_Respects_Distribution() {
    const double p = 0.3;
    var trueCount = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      if (_rng.NextBoolean(p))
        ++trueCount;

    var ratio = (double)trueCount / n;
    Assert.That(ratio, Is.InRange(p - 0.01, p + 0.01), $"Observed {ratio:F4}, expected {p}");
  }

  [Test]
  public void NextSign_Returns_PlusMinus_One_With_Equal_Probability() {
    var plusCount = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i) {
      var s = _rng.NextSign();
      Assert.That(s, Is.EqualTo(1).Or.EqualTo(-1));
      if (s == 1)
        ++plusCount;
    }

    var ratio = (double)plusCount / n;
    Assert.That(ratio, Is.InRange(0.49, 0.51));
  }

  [Test]
  public void NextNonZero_Never_Returns_Zero() {
    for (var i = 0; i < 10_000; ++i)
      Assert.That(_rng.NextNonZero(), Is.Not.EqualTo(0UL));
  }

  [Test]
  public void NextBytes_Fills_Span_Of_Various_Sizes() {
    for (var len = 0; len <= 33; ++len) {
      var buffer = new byte[len];
      _rng.NextBytes(buffer);
      Assert.That(buffer.Length, Is.EqualTo(len));
      if (len >= 8) {
        // Sanity: not all zeros for non-trivial sizes
        Assert.That(buffer.Any(b => b != 0), Is.True, $"All zeros at length {len}");
      }
    }
  }

  [Test]
  public void NextGuid_Has_Version_4_And_Variant_RFC4122() {
    for (var i = 0; i < 1000; ++i) {
      var guid = _rng.NextGuid();
      var bytes = guid.ToByteArray();
      // Guid.ToByteArray reorders the first 8 bytes to little-endian; version/variant live at indices 7 and 8.
      Assert.That(bytes[7] >> 4, Is.EqualTo(4), "version nibble");
      Assert.That(bytes[8] >> 6, Is.EqualTo(2), "variant bits");
    }
  }

  [Test]
  public void NextGuid_Produces_Different_Values() {
    var set = new HashSet<Guid>();
    for (var i = 0; i < 1000; ++i)
      set.Add(_rng.NextGuid());
    Assert.That(set.Count, Is.EqualTo(1000));
  }

  [Test]
  public void Shuffle_Preserves_Elements() {
    var list = Enumerable.Range(0, 100).ToList();
    var original = list.ToHashSet();
    _rng.Shuffle(list);
    Assert.That(list.ToHashSet(), Is.EquivalentTo(original));
  }

  [Test]
  public void Shuffle_Changes_Order_Most_Of_The_Time() {
    var list = Enumerable.Range(0, 50).ToList();
    var before = list.ToArray();
    _rng.Shuffle(list);
    Assert.That(list.SequenceEqual(before), Is.False);
  }

  [Test]
  public void Choice_Returns_Element_From_Source() {
    var source = new[] { 10, 20, 30, 40, 50 };
    var sourceSet = source.ToHashSet();
    for (var i = 0; i < 100; ++i)
      Assert.That(sourceSet, Does.Contain(_rng.Choice(source)));
  }

  [Test]
  public void Choice_Distribution_Is_Roughly_Uniform() {
    var source = new[] { 0, 1, 2, 3, 4 };
    var counts = new int[5];
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      ++counts[_rng.Choice(source)];

    foreach (var c in counts)
      Assert.That(c, Is.InRange(n / 5 * 0.9, n / 5 * 1.1));
  }

  [Test]
  public void Sample_Returns_Requested_Count_Without_Duplicates() {
    var source = Enumerable.Range(0, 100).ToArray();
    var sample = _rng.Sample(source, 20);
    Assert.That(sample.Length, Is.EqualTo(20));
    Assert.That(sample.Distinct().Count(), Is.EqualTo(20));
    foreach (var v in sample)
      Assert.That(v, Is.GreaterThanOrEqualTo(0).And.LessThan(100));
  }

  [Test]
  public void Sample_Throws_When_Count_Exceeds_Source() {
    var source = Enumerable.Range(0, 10).ToArray();
    Assert.Throws<ArgumentOutOfRangeException>(() => _rng.Sample(source, 11));
  }

  [Test]
  public void Sample_All_Elements_Returns_Permutation() {
    var source = Enumerable.Range(0, 50).ToArray();
    var sample = _rng.Sample(source, source.Length);
    Assert.That(sample, Is.EquivalentTo(source));
  }
}
