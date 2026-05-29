using System;
using System.Collections.Generic;
using Hawkynt.RandomNumberGenerators.Composites;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.NonUniform;

namespace RandomNumberGenerators.Tests;

[TestFixture]
public class NonUniformTests {

  private ArbitraryNumberGenerator _rng = null!;

  [SetUp]
  public void SetUp() {
    _rng = new ArbitraryNumberGenerator(new Xoshiro256SS());
    _rng.Seed(Generators.SEED);
  }

  [Test]
  public void BoxMuller_Produces_Values_Near_Zero_Mean() {
    var bm = new BoxMuller(_rng);
    double sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i) {
      var (z0, z1) = bm.Next();
      sum += z0 + z1;
    }

    var mean = sum / (n * 2);
    Assert.That(Math.Abs(mean), Is.LessThan(0.02), $"BoxMuller mean: {mean:F4}");
  }

  [Test]
  public void MarsagliaPolar_Produces_Values_Near_Zero_Mean() {
    var mp = new MarsagliaPolar(_rng);
    double sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i) {
      var (z0, z1) = mp.Next();
      sum += z0 + z1;
    }

    var mean = sum / (n * 2);
    Assert.That(Math.Abs(mean), Is.LessThan(0.02), $"MarsagliaPolar mean: {mean:F4}");
  }

  [Test]
  public void Ziggurat_Produces_Values_Near_Zero_Mean() {
    var zig = new Ziggurat(_rng);
    double sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      sum += zig.Next();

    var mean = sum / n;
    Assert.That(Math.Abs(mean), Is.LessThan(0.02), $"Ziggurat mean: {mean:F4}");
  }

  [Test]
  public void InverseTransformSampling_Produces_Positive_Values() {
    var its = new InverseTransformSampling(_rng);
    for (var i = 0; i < 1000; ++i)
      Assert.That(its.Next(), Is.GreaterThanOrEqualTo(0.0));
  }

  [Test]
  public void InverseTransformSampling_Mean_Approximates_One_Over_Lambda() {
    const double lambda = 2.0;
    var its = new InverseTransformSampling(_rng, lambda);
    double sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      sum += its.Next();

    var mean = sum / n;
    var expected = 1.0 / lambda;
    Assert.That(mean, Is.InRange(expected * 0.95, expected * 1.05), $"ITS mean: {mean:F4}, expected: {expected:F4}");
  }

  [Test]
  public void Poisson_Mean_Approximates_Lambda() {
    const double lambda = 5.0;
    var p = new Poisson(_rng, lambda);
    long sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      sum += p.Next();

    var mean = (double)sum / n;
    Assert.That(mean, Is.InRange(lambda * 0.97, lambda * 1.03), $"Poisson mean: {mean:F4}, expected ~{lambda}");
  }

  [Test]
  public void Poisson_LargeLambda_Branch_Works() {
    const double lambda = 100.0;
    var p = new Poisson(_rng, lambda);
    long sum = 0;
    const int n = 50_000;
    for (var i = 0; i < n; ++i)
      sum += p.Next();

    var mean = (double)sum / n;
    Assert.That(mean, Is.InRange(lambda * 0.98, lambda * 1.02));
  }

  [Test]
  public void Poisson_Variance_Approximates_Lambda() {
    const double lambda = 4.0;
    var p = new Poisson(_rng, lambda);
    double sum = 0, sum2 = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i) {
      var v = p.Next();
      sum += v;
      sum2 += v * v;
    }
    var mean = sum / n;
    var variance = sum2 / n - mean * mean;
    Assert.That(variance, Is.InRange(lambda * 0.9, lambda * 1.1), $"Poisson variance: {variance:F4}, expected ~{lambda}");
  }

  [Test]
  public void Gamma_Mean_Approximates_Shape_Times_Scale() {
    const double shape = 2.5;
    const double scale = 3.0;
    var g = new Gamma(_rng, shape, scale);
    double sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      sum += g.Next();

    var mean = sum / n;
    var expected = shape * scale;
    Assert.That(mean, Is.InRange(expected * 0.97, expected * 1.03), $"Gamma mean: {mean:F4}, expected ~{expected}");
  }

  [Test]
  public void Gamma_SmallShape_Branch_Works() {
    var g = new Gamma(_rng, shape: 0.5, scale: 1.0);
    double sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i) {
      var v = g.Next();
      Assert.That(v, Is.GreaterThanOrEqualTo(0.0));
      sum += v;
    }
    var mean = sum / n;
    Assert.That(mean, Is.InRange(0.45, 0.55));
  }

  [Test]
  public void Beta_Mean_Approximates_Alpha_Over_AlphaPlusBeta() {
    const double alpha = 2.0;
    const double beta = 5.0;
    var b = new Beta(_rng, alpha, beta);
    double sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i) {
      var v = b.Next();
      Assert.That(v, Is.GreaterThan(0.0).And.LessThan(1.0));
      sum += v;
    }
    var mean = sum / n;
    var expected = alpha / (alpha + beta);
    Assert.That(mean, Is.InRange(expected * 0.97, expected * 1.03), $"Beta mean: {mean:F4}, expected ~{expected}");
  }

  [Test]
  public void Bernoulli_Distribution_Matches_Probability() {
    const double p = 0.3;
    var bern = new Bernoulli(_rng, p);
    var trueCount = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      if (bern.Next())
        ++trueCount;

    var ratio = (double)trueCount / n;
    Assert.That(ratio, Is.InRange(p - 0.01, p + 0.01));
  }

  [Test]
  public void Binomial_Mean_Approximates_NP() {
    const int trials = 50;
    const double p = 0.4;
    var bin = new Binomial(_rng, trials, p);
    long sum = 0;
    const int n = 50_000;
    for (var i = 0; i < n; ++i) {
      var v = bin.Next();
      Assert.That(v, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(trials));
      sum += v;
    }
    var mean = (double)sum / n;
    var expected = trials * p;
    Assert.That(mean, Is.InRange(expected * 0.98, expected * 1.02));
  }

  [Test]
  public void Geometric_Mean_Approximates_OneMinusP_Over_P() {
    const double p = 0.25;
    var geom = new Geometric(_rng, p);
    long sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i) {
      var v = geom.Next();
      Assert.That(v, Is.GreaterThanOrEqualTo(0));
      sum += v;
    }
    var mean = (double)sum / n;
    var expected = (1.0 - p) / p;
    Assert.That(mean, Is.InRange(expected * 0.95, expected * 1.05));
  }

  [Test]
  public void ChiSquared_Mean_Approximates_DegreesOfFreedom() {
    const double df = 4.0;
    var chi = new ChiSquared(_rng, df);
    double sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      sum += chi.Next();
    var mean = sum / n;
    Assert.That(mean, Is.InRange(df * 0.97, df * 1.03));
  }

  [Test]
  public void Cauchy_Median_Approximates_Location() {
    var cauchy = new Cauchy(_rng, location: 5.0, scale: 1.0);
    var samples = new double[10_000];
    for (var i = 0; i < samples.Length; ++i)
      samples[i] = cauchy.Next();
    Array.Sort(samples);
    var median = samples[samples.Length / 2];
    // Cauchy has no mean but a stable median == location parameter.
    Assert.That(median, Is.InRange(4.85, 5.15));
  }

  [Test]
  public void Lognormal_Median_Approximates_ExpMu() {
    var ln = new Lognormal(_rng, mu: 1.0, sigma: 0.5);
    var samples = new double[50_000];
    for (var i = 0; i < samples.Length; ++i) {
      samples[i] = ln.Next();
      Assert.That(samples[i], Is.GreaterThan(0.0));
    }
    Array.Sort(samples);
    var median = samples[samples.Length / 2];
    var expected = Math.Exp(1.0); // exp(mu)
    Assert.That(median, Is.InRange(expected * 0.97, expected * 1.03));
  }

  [Test]
  public void Weibull_Mean_Approximates_ScaleTimesGamma() {
    const double shape = 2.0;
    const double scale = 3.0;
    var w = new Weibull(_rng, shape, scale);
    double sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i) {
      var v = w.Next();
      Assert.That(v, Is.GreaterThanOrEqualTo(0.0));
      sum += v;
    }
    var mean = sum / n;
    // For shape=2, mean = scale * Γ(3/2) = scale * √π / 2 ≈ 0.8862 * scale.
    var expected = scale * Math.Sqrt(Math.PI) / 2.0;
    Assert.That(mean, Is.InRange(expected * 0.97, expected * 1.03));
  }

  [Test]
  public void Triangular_Mean_Approximates_Average() {
    var tri = new Triangular(_rng, min: 0.0, mode: 2.0, max: 10.0);
    double sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i) {
      var v = tri.Next();
      Assert.That(v, Is.InRange(0.0, 10.0));
      sum += v;
    }
    var mean = sum / n;
    var expected = (0.0 + 2.0 + 10.0) / 3.0;
    Assert.That(mean, Is.InRange(expected * 0.97, expected * 1.03));
  }

  [Test]
  public void Pareto_All_Samples_Above_Scale() {
    const double shape = 2.5;
    const double scale = 4.0;
    var pareto = new Pareto(_rng, shape, scale);
    for (var i = 0; i < 10_000; ++i)
      Assert.That(pareto.Next(), Is.GreaterThanOrEqualTo(scale));
  }

  [Test]
  public void Pareto_Mean_Matches_For_Shape_GreaterThan_One() {
    const double shape = 3.0;
    const double scale = 2.0;
    var pareto = new Pareto(_rng, shape, scale);
    double sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      sum += pareto.Next();
    var mean = sum / n;
    // For shape > 1, mean = shape * scale / (shape - 1) = 3.0 * 2.0 / 2.0 = 3.0
    var expected = shape * scale / (shape - 1.0);
    Assert.That(mean, Is.InRange(expected * 0.95, expected * 1.05));
  }

  [Test]
  public void Categorical_Distribution_Matches_Weights() {
    var items = new[] { ("A", 1.0), ("B", 2.0), ("C", 3.0), ("D", 4.0) };
    var cat = new Categorical<string>(_rng, items);
    var counts = new Dictionary<string, int> { ["A"] = 0, ["B"] = 0, ["C"] = 0, ["D"] = 0 };
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      ++counts[cat.Next()];

    // Expected fractions: 0.1, 0.2, 0.3, 0.4 (total weight = 10)
    Assert.That((double)counts["A"] / n, Is.InRange(0.09, 0.11));
    Assert.That((double)counts["B"] / n, Is.InRange(0.19, 0.21));
    Assert.That((double)counts["C"] / n, Is.InRange(0.29, 0.31));
    Assert.That((double)counts["D"] / n, Is.InRange(0.39, 0.41));
  }

  [Test]
  public void Categorical_Throws_On_Empty_Source() {
    Assert.Throws<ArgumentException>(() => new Categorical<int>(_rng, Array.Empty<(int, double)>()));
  }

  [Test]
  public void DiscreteUniform_Stays_Within_Range() {
    var u = new DiscreteUniform(_rng, 10, 20);
    for (var i = 0; i < 10_000; ++i)
      Assert.That(u.Next(), Is.GreaterThanOrEqualTo(10L).And.LessThanOrEqualTo(20L));
  }

  [Test]
  public void DiscreteUniform_Covers_All_Values() {
    var u = new DiscreteUniform(_rng, 1, 6); // a six-sided die
    var counts = new int[7];
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      ++counts[u.Next()];
    for (var v = 1; v <= 6; ++v) {
      var ratio = (double)counts[v] / n;
      Assert.That(ratio, Is.InRange(1.0 / 6.0 - 0.01, 1.0 / 6.0 + 0.01));
    }
  }

  [Test]
  public void DiscreteUniform_Throws_When_Min_Greater_Than_Max() {
    Assert.Throws<ArgumentException>(() => new DiscreteUniform(_rng, 10, 5));
  }

  [Test]
  public void Hypergeometric_Mean_Approximates_NK_Over_Population() {
    const int populationSize = 100;
    const int successCount = 30;
    const int draws = 20;
    var hg = new Hypergeometric(_rng, populationSize, successCount, draws);
    long sum = 0;
    const int n = 50_000;
    for (var i = 0; i < n; ++i) {
      var v = hg.Next();
      Assert.That(v, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(draws));
      sum += v;
    }
    var mean = (double)sum / n;
    // Mean of Hypergeometric is n*K/N = 20*30/100 = 6.
    var expected = (double)(draws * successCount) / populationSize;
    Assert.That(mean, Is.InRange(expected * 0.97, expected * 1.03));
  }

  [Test]
  public void StudentT_Symmetric_Around_Zero() {
    var t = new StudentT(_rng, degreesOfFreedom: 10.0);
    double sum = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i)
      sum += t.Next();
    var mean = sum / n;
    Assert.That(Math.Abs(mean), Is.LessThan(0.05));
  }

  [Test]
  public void StudentT_Variance_Matches_Formula_For_LargeDf() {
    const double df = 30.0;
    var t = new StudentT(_rng, df);
    double sum = 0, sum2 = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i) {
      var v = t.Next();
      sum += v;
      sum2 += v * v;
    }
    var mean = sum / n;
    var variance = sum2 / n - mean * mean;
    var expected = df / (df - 2.0); // = 30/28 ≈ 1.0714
    Assert.That(variance, Is.InRange(expected * 0.9, expected * 1.1));
  }

  [Test]
  public void NegativeBinomial_Mean_Approximates_K_Times_OneMinusP_Over_P() {
    const int k = 5;
    const double p = 0.4;
    var nb = new NegativeBinomial(_rng, k, p);
    long sum = 0;
    const int n = 50_000;
    for (var i = 0; i < n; ++i) {
      var v = nb.Next();
      Assert.That(v, Is.GreaterThanOrEqualTo(0));
      sum += v;
    }
    var mean = (double)sum / n;
    var expected = k * (1.0 - p) / p; // = 5 * 0.6 / 0.4 = 7.5
    Assert.That(mean, Is.InRange(expected * 0.97, expected * 1.03));
  }

  [Test]
  public void Gaussian_Generators_Approximate_Unit_Variance() {
    var mp = new MarsagliaPolar(_rng);
    double sum = 0, sum2 = 0;
    const int n = 100_000;
    for (var i = 0; i < n; ++i) {
      var (z0, _) = mp.Next();
      sum += z0;
      sum2 += z0 * z0;
    }

    var mean = sum / n;
    var variance = sum2 / n - mean * mean;
    Assert.That(variance, Is.InRange(0.9, 1.1), $"MarsagliaPolar variance: {variance:F4}");
  }
}
