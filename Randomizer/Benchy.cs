using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Randomizer;

internal class Benchy {

  public static readonly TimeSpan TIME_TO_MEASURE = TimeSpan.FromSeconds(30);

  public void MeasureThroughput() {

    var sources = new RandomSources();

    var allCalls = new (string name, Func<ulong> factory)[] {
      ("Measuring Overhead", () => 131),
    }.Concat(sources.FactorySource()).ToArray();

    var totalBudget = TimeSpan.FromSeconds(TIME_TO_MEASURE.TotalSeconds * allCalls.Length);
    Console.WriteLine($"Warming up {allCalls.Length} generators...");
    WarmUp();
    Console.WriteLine($"Each generator runs for {TIME_TO_MEASURE.TotalSeconds:F0}s. Estimated total benchmark time: {totalBudget.TotalMinutes:F0} min.");
    Console.WriteLine();

    var overallStopwatch = Stopwatch.StartNew();
    var overhead = Measure(0, allCalls.Length, allCalls[0].name, allCalls[0].factory);
    Console.WriteLine($"  Overhead floor measured: {overhead:#,0} iter/s (cost of the harness itself).");
    Console.WriteLine($"  Reported speedups below are vs. the first real generator.");
    Console.WriteLine();

    var referenceIterations = Measure(1, allCalls.Length, allCalls[1].name, allCalls[1].factory, emptyIterationsPerSecond: overhead);
    for (var i = 2; i < allCalls.Length; ++i)
      Measure(i, allCalls.Length, allCalls[i].name, allCalls[i].factory, referenceIterations, overhead);

    Console.WriteLine();
    Console.WriteLine($"  Benchmark phase done in {overallStopwatch.Elapsed.TotalMinutes:F1} min.");

    return;

    void WarmUp() {
      for (var i = 0; i < 100; ++i)
        foreach (var (_, rng) in allCalls)
          rng();
    }

    double Measure(int index, int total, string name, Func<ulong> call, double referenceIterationsPerSecond = 0, double emptyIterationsPerSecond = 0) {
      var measureStopwatch = Stopwatch.StartNew();

      // Live in-place line: "  [N/Total] name... running 12s/30s"
      var prefix = $"  [{index + 1,3}/{total}] {name,-38}";
      using var ticker = new InlineCountdownTicker(prefix, measureStopwatch, TIME_TO_MEASURE);

      var dummy = 0UL;
      var iterations = 0UL;
      var ticksPerSecond = Stopwatch.Frequency;
      var ticksToMeasure = (long)(ticksPerSecond * TIME_TO_MEASURE.TotalSeconds);

      var startTicks = Stopwatch.GetTimestamp();
      var endTicks = startTicks + ticksToMeasure;
      long ticksTaken;
      do {
        ++iterations;
        dummy ^= call();
      } while ((ticksTaken = Stopwatch.GetTimestamp()) < endTicks);
      ticksTaken -= startTicks;

      ticker.Dispose(); // clears the live line before we print the final result

      var iterationsPerTick = (double)iterations / ticksTaken;
      var iterationsPerSecond = iterationsPerTick * ticksPerSecond;
      if (referenceIterationsPerSecond <= 0)
        referenceIterationsPerSecond = iterationsPerSecond;

      Console.WriteLine($"{prefix} {iterationsPerSecond,15:0} iter/s   {iterationsPerSecond / referenceIterationsPerSecond,5:0.0}× ({100 * (iterationsPerSecond / referenceIterationsPerSecond - 1),6:0.0}%)");
      return iterationsPerSecond;
    }

  }

  /// <summary>
  ///   Writes a single line in place ("prefix running Ns/Ms") that updates every 200 ms
  ///   while a measurement runs, then clears itself on Dispose so the caller can write
  ///   the final result line on top.
  /// </summary>
  private sealed class InlineCountdownTicker : IDisposable {
    private readonly string _prefix;
    private readonly Stopwatch _stopwatch;
    private readonly TimeSpan _total;
    private readonly Timer _timer;
    private int _disposed;

    public InlineCountdownTicker(string prefix, Stopwatch stopwatch, TimeSpan total) {
      this._prefix = prefix;
      this._stopwatch = stopwatch;
      this._total = total;
      // Print an initial line immediately so the user sees the generator name right away.
      this._Tick(null);
      this._timer = new Timer(this._Tick, null, 200, 200);
    }

    private void _Tick(object? state) {
      if (Volatile.Read(ref this._disposed) != 0)
        return;

      var elapsed = this._stopwatch.Elapsed.TotalSeconds;
      var total = this._total.TotalSeconds;
      // Carriage return + line content; no newline. Padding ensures no trailing remnants.
      Console.Error.Write($"\r{this._prefix} running {elapsed,4:F1}s / {total,4:F0}s ...                       ");
    }

    public void Dispose() {
      if (Interlocked.Exchange(ref this._disposed, 1) != 0)
        return;

      this._timer.Dispose();
      // Wipe the live line so the caller can println the final result cleanly.
      Console.Error.Write("\r                                                                                            \r");
    }
  }
}
