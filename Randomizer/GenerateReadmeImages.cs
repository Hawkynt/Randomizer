using System;
using System.IO;
using System.Linq;
using Hawkynt.RandomNumberGenerators.Cryptographic;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;
using Hawkynt.RandomNumberGenerators.QuasiRandom;
using Randomizer.Statistics;

namespace Randomizer;

/// <summary>
///   Regenerates only the PNG files actually referenced from <c>Readme.md</c> and
///   <c>RandomNumberGenerators/ReadMe.md</c>. Keep the file list here in sync with
///   the Readme references — earlier versions produced 7 images per family but
///   only ~1 per family were used in the Readme, leaving 83 orphaned PNGs in the
///   repo. Anything written here must be a file the Readme actually links to.
/// </summary>
internal static class GenerateReadmeImages {

  public static void Run(string outputDir) {
    Directory.CreateDirectory(outputDir);

    // ── Family randograms ──────────────────────────────────────────────────
    // Each of these gets exactly one randogram referenced from the
    // family-comparison panel in Readme.md.
    (string name, IRandomNumberGenerator gen)[] randogramShowcase = [
      ("LinearCongruentialGenerator", new LinearCongruentialGenerator()),
      ("MultiplicativeLCG", new MultiplicativeLinearCongruentialGenerator()),
      ("MersenneTwister", new MersenneTwister()),
      ("Xoshiro256SS", new Xoshiro256SS()),
      ("PermutedCongruentialXslRr", new PermutedCongruentialXslRr()),
      ("Philox", new Philox()),
      ("RomuTrio", new RomuTrio()),
      ("Lehmer128", new Lehmer128()),
      ("WyRand", new WyRand()),
      ("SHISHUA", new Shishua()),
      ("ChaCha20", new ChaCha20()),
      ("AesCtrDrbg", new AesCtrDrbg()),
      ("Isaac", new Isaac()),
      ("Rule30", new Rule30()),
      ("Halton", new Halton()),
      ("MiddleSquare", new MiddleSquare()),
    ];

    // PCG convention (Visualizing the heart of some PRNGs): each pixel encodes
    // an (output_n, output_{n+1}) pair of *consecutive outputs*, with 65,536
    // samples so that the expected visit count per cell is exactly 1.
    // PCG convention (Visualizing the heart of some PRNGs): each pixel encodes
    // an (output_n, output_{n+1}) pair of *consecutive outputs*, with 65,536
    // samples so that the expected visit count per cell is exactly 1.
    foreach (var (name, gen) in randogramShowcase) {
      gen.Seed(131);
      var safeName = string.Concat(name.Select(c => char.IsLetterOrDigit(c) ? c : '_'));
      var randogram = new Randogram(16, Randogram.BitSelectionMethod.Consecutive,
        new FileInfo(Path.Combine(outputDir, $"{safeName}_randogram.png")));
      for (var i = 0; i < 65_536; ++i)
        randogram.Feed(gen.Next());
      randogram.Print();
    }

    // ── Multi-method randogram comparison ──────────────────────────────────
    // For a curated subset, also generate the Adjacent, OppositeHalves and
    // StartAndReverse projections so the Readme can show how different
    // bit-selection methods reveal different structural failures. Each
    // method draws (x, y) from different parts of the output stream:
    //   - Consecutive    : (output_n_low8, output_{n+1}_low8) - serial correlation
    //   - Adjacent       : (output_low8, output_bits_8-15)    - within-value bit dependency
    //   - OppositeHalves : (output_low8, output_bits_32-39)   - distant-bit dependency
    //   - StartAndReverse: (output_low8, reverse(output)_low8) - high/low symmetry
    (string name, Func<IRandomNumberGenerator> factory)[] multiMethodShowcase = [
      ("LinearCongruentialGenerator", () => new LinearCongruentialGenerator()),
      ("MultiplicativeLCG", () => new MultiplicativeLinearCongruentialGenerator()),
      ("MersenneTwister", () => new MersenneTwister()),
      ("Xoshiro256SS", () => new Xoshiro256SS()),
      ("PermutedCongruentialXslRr", () => new PermutedCongruentialXslRr()),
      ("ChaCha20", () => new ChaCha20()),
      ("MiddleSquare", () => new MiddleSquare()),
    ];

    var extraMethods = new[] {
      Randogram.BitSelectionMethod.Adjacent,
      Randogram.BitSelectionMethod.OppositeHalves,
      Randogram.BitSelectionMethod.StartAndReverse,
    };

    foreach (var (name, factory) in multiMethodShowcase) {
      var safeName = string.Concat(name.Select(c => char.IsLetterOrDigit(c) ? c : '_'));
      foreach (var method in extraMethods) {
        var gen = factory();
        gen.Seed(131);
        var methodSuffix = method.ToString().ToLowerInvariant();
        var randogram = new Randogram(16, method,
          new FileInfo(Path.Combine(outputDir, $"{safeName}_randogram_{methodSuffix}.png")));
        for (var i = 0; i < 65_536; ++i)
          randogram.Feed(gen.Next());
        randogram.Print();
      }
    }

    // ── Histogram comparison panels ─────────────────────────────────────────
    // Four generators get the full 6-histogram treatment because the Readme's
    // per-test comparison panels (Bit Index, Bit Count, Spacing, Repetition,
    // Hamming, Longest Run) link to them.
    (string name, IRandomNumberGenerator gen)[] histogramShowcase = [
      ("Xoshiro256SS", new Xoshiro256SS()),
      ("ChaCha20", new ChaCha20()),
      ("MultiplicativeLCG", new MultiplicativeLinearCongruentialGenerator()),
      ("MiddleSquare", new MiddleSquare()),
    ];
    ImageGenerator.GenerateAll(outputDir, histogramShowcase);

    // ── Halton special case ─────────────────────────────────────────────────
    // The Halton randogram is degenerate (low bits structurally zero), so the
    // Readme links to the bit-index histogram instead. Generate just that one.
    {
      ImageGenerator.GenerateAll(outputDir, [("Halton", new Halton())]);
      foreach (var suffix in new[] { "bit_count", "hamming", "longest_run", "repetition", "spacing" }) {
        var path = Path.Combine(outputDir, $"Halton_{suffix}.png");
        if (File.Exists(path))
          File.Delete(path);
      }
    }

    // ── Trim unreferenced histograms ───────────────────────────────────────
    // ChaCha20, MultiplicativeLCG, MiddleSquare only need a subset of histograms.
    // Delete the ones the Readme doesn't link to so the Images/ folder stays clean.
    var unreferenced = new[] {
      "ChaCha20_bit_count.png", "ChaCha20_bit_index.png", "ChaCha20_hamming.png",
      "ChaCha20_longest_run.png", "ChaCha20_repetition.png",
      "MultiplicativeLCG_bit_count.png", "MultiplicativeLCG_longest_run.png",
      "MultiplicativeLCG_repetition.png", "MultiplicativeLCG_spacing.png",
      "MiddleSquare_hamming.png", "MiddleSquare_longest_run.png", "MiddleSquare_spacing.png",
    };
    foreach (var name in unreferenced) {
      var path = Path.Combine(outputDir, name);
      if (File.Exists(path))
        File.Delete(path);
    }

    Console.WriteLine($"\nDone. Images written to {outputDir}");
  }
}
