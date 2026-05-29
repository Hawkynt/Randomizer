using System;
using System.IO;
using Hawkynt.RandomNumberGenerators.Composites;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.NonUniform;
using Randomizer.Statistics;

namespace Randomizer;

internal static class GenerateDistributionImages {

  public static void Run(string outputDir) {
    Directory.CreateDirectory(outputDir);

    // Use a single high-quality uniform RNG seeded deterministically for all distributions.
    var rng = new ArbitraryNumberGenerator(new Xoshiro256SS());
    rng.Seed(131);

    Console.WriteLine("Rendering distribution shapes...");

    // Standard normal via MarsagliaPolar (returns pairs, take first)
    var mp = new MarsagliaPolar(rng);
    DistributionImage.RenderContinuous(
      "Standard Normal (μ=0, σ=1) via Marsaglia-Polar",
      () => mp.Next().Item1,
      new FileInfo(Path.Combine(outputDir, "dist_normal.png")),
      min: -4, max: 4);

    // Exponential via InverseTransformSampling
    var exp = new InverseTransformSampling(rng, lambda: 1.0);
    DistributionImage.RenderContinuous(
      "Exponential (λ=1)",
      () => exp.Next(),
      new FileInfo(Path.Combine(outputDir, "dist_exponential.png")),
      min: 0, max: 8);

    // Gamma(shape=2, scale=2)
    var gamma = new Gamma(rng, shape: 2.0, scale: 2.0);
    DistributionImage.RenderContinuous(
      "Gamma (shape=2, scale=2)",
      () => gamma.Next(),
      new FileInfo(Path.Combine(outputDir, "dist_gamma.png")),
      min: 0, max: 16);

    // Beta(2, 5) - skewed left
    var beta = new Beta(rng, alpha: 2.0, beta: 5.0);
    DistributionImage.RenderContinuous(
      "Beta (α=2, β=5)",
      () => beta.Next(),
      new FileInfo(Path.Combine(outputDir, "dist_beta.png")),
      min: 0, max: 1);

    // Cauchy - heavy tails (clip to make it visible)
    var cauchy = new Cauchy(rng, location: 0.0, scale: 1.0);
    DistributionImage.RenderContinuous(
      "Cauchy (location=0, scale=1) — note heavy tails",
      () => cauchy.Next(),
      new FileInfo(Path.Combine(outputDir, "dist_cauchy.png")),
      min: -6, max: 6);

    // Lognormal
    var ln = new Lognormal(rng, mu: 0.0, sigma: 0.5);
    DistributionImage.RenderContinuous(
      "Lognormal (μ=0, σ=0.5)",
      () => ln.Next(),
      new FileInfo(Path.Combine(outputDir, "dist_lognormal.png")),
      min: 0, max: 5);

    // Weibull with three shapes (separate plots)
    foreach (var (shape, name) in new[] { (0.5, "0.5"), (1.0, "1"), (2.5, "2.5") }) {
      var w = new Weibull(rng, shape: shape, scale: 1.0);
      DistributionImage.RenderContinuous(
        $"Weibull (shape={name}, scale=1)",
        () => w.Next(),
        new FileInfo(Path.Combine(outputDir, $"dist_weibull_{name.Replace(".", "_")}.png")),
        min: 0, max: 4);
    }

    // Triangular
    var tri = new Triangular(rng, min: 0.0, mode: 2.0, max: 10.0);
    DistributionImage.RenderContinuous(
      "Triangular (min=0, mode=2, max=10)",
      () => tri.Next(),
      new FileInfo(Path.Combine(outputDir, "dist_triangular.png")),
      min: 0, max: 10);

    // Pareto
    var pareto = new Pareto(rng, shape: 2.0, scale: 1.0);
    DistributionImage.RenderContinuous(
      "Pareto (shape=2, scale=1)",
      () => pareto.Next(),
      new FileInfo(Path.Combine(outputDir, "dist_pareto.png")),
      min: 1, max: 6);

    // Chi-Squared
    var chi = new ChiSquared(rng, degreesOfFreedom: 4.0);
    DistributionImage.RenderContinuous(
      "Chi-Squared (k=4)",
      () => chi.Next(),
      new FileInfo(Path.Combine(outputDir, "dist_chisquared.png")),
      min: 0, max: 16);

    // Poisson - discrete
    var poisson = new Poisson(rng, lambda: 4.0);
    DistributionImage.RenderDiscrete(
      "Poisson (λ=4)",
      () => poisson.Next(),
      new FileInfo(Path.Combine(outputDir, "dist_poisson.png")),
      maxValue: 16);

    // Binomial - discrete
    var binomial = new Binomial(rng, trials: 20, probability: 0.4);
    DistributionImage.RenderDiscrete(
      "Binomial (n=20, p=0.4)",
      () => binomial.Next(),
      new FileInfo(Path.Combine(outputDir, "dist_binomial.png")),
      maxValue: 20);

    // Geometric - discrete
    var geom = new Geometric(rng, probability: 0.3);
    DistributionImage.RenderDiscrete(
      "Geometric (p=0.3)",
      () => geom.Next(),
      new FileInfo(Path.Combine(outputDir, "dist_geometric.png")),
      maxValue: 20);

    Console.WriteLine($"Done. Images written to {outputDir}");
  }
}
