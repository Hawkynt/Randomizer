using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Randomizer;

if (args.Length > 0 && args[0] == "--generate-images") {
  Randomizer.GenerateReadmeImages.Run(args.Length > 1 ? args[1] : "Images");
  return;
}

if (args.Length > 0 && args[0] == "--generate-distribution-images") {
  Randomizer.GenerateDistributionImages.Run(args.Length > 1 ? args[1] : "Images");
  return;
}

// ── Banner ────────────────────────────────────────────────────────────────
var totalStopwatch = Stopwatch.StartNew();
var sources = new RandomSources().FactorySource().ToArray();
var sourceCount = sources.Length;

Console.WriteLine("===================================================================");
Console.WriteLine($"  Randomizer — statistical analysis & benchmarking of {sourceCount} RNGs");
Console.WriteLine("===================================================================");
Console.WriteLine();
Console.WriteLine($"This run will execute two phases against {sourceCount} generators:");
Console.WriteLine("  1. Statistical analysis (100k samples each, run in parallel)");
Console.WriteLine($"  2. Throughput benchmark ({(int)Benchy.TIME_TO_MEASURE.TotalSeconds}s per generator, sequential)");
Console.WriteLine();
Console.WriteLine("Some cryptographic generators (Blum-Blum-Shub, BlumMicali, AES-CTR DRBG,");
Console.WriteLine("HMAC DRBG, Hash DRBG) are intrinsically slow — expect the parallel stats");
Console.WriteLine("phase to wait for them, and the benchmark phase to take ~30 min in total.");
Console.WriteLine("Press Ctrl+C at any time to abort.");
Console.WriteLine();

// ── Phase 1: statistical analysis ─────────────────────────────────────────
Console.WriteLine($"[Phase 1/2] Statistical analysis ({sourceCount} generators in parallel)...");
var phaseStopwatch = Stopwatch.StartNew();

var statsTracker = sources
  .Select(r => (r.name, r.factory, tracker: new StatsTracker(r.name)))
  .ToArray();

var completed = 0;
var inFlight = new ConcurrentDictionary<string, Stopwatch>();

using (var ticker = new ProgressTicker(
  readCount: () => Volatile.Read(ref completed),
  readInFlight: () => inFlight.Select(kv => (kv.Key, kv.Value.Elapsed.TotalSeconds)).ToList(),
  total: sourceCount,
  stopwatch: phaseStopwatch)) {

  statsTracker.ParallelForEach(t => {
    var sw = Stopwatch.StartNew();
    inFlight[t.name] = sw;

    for (var i = 0; i < 100_000; ++i)
      t.tracker.Feed(t.factory());

    inFlight.TryRemove(t.name, out _);
    Interlocked.Increment(ref completed);
  });
}

Console.WriteLine($"  Phase 1 done in {phaseStopwatch.Elapsed.TotalSeconds:F1}s.");
Console.WriteLine();

// ── Stats output ─────────────────────────────────────────────────────────
Console.WriteLine("===================================================================");
Console.WriteLine("  Per-generator statistical results");
Console.WriteLine("===================================================================");
Console.WriteLine();

foreach (var tracker in statsTracker) {
  Console.WriteLine($"── Stats for {tracker.name} ────────────────────────────────");
  tracker.tracker.Print();
  Console.WriteLine();
}

// ── Phase 2: benchmark ──────────────────────────────────────────────────
Console.WriteLine("===================================================================");
Console.WriteLine($"  [Phase 2/2] Throughput benchmark");
Console.WriteLine("===================================================================");
Console.WriteLine();

new Benchy().MeasureThroughput();

// ── Total ────────────────────────────────────────────────────────────────
Console.WriteLine();
Console.WriteLine($"All phases complete. Total elapsed: {totalStopwatch.Elapsed.TotalMinutes:F1} min.");
