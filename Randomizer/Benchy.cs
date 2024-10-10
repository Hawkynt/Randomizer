using System;
using System.Diagnostics;
using System.Linq;

namespace Randomizer;

internal class Benchy {

  public static readonly TimeSpan TIME_TO_MEASURE = TimeSpan.FromSeconds(30);

  public void MeasureThroughput() {

    var sources = new RandomSources();
    
    var allCalls = new (string name, Func<ulong> factory)[]{
      ("Measuring Overhead", () => 131),
    }.Concat(sources.FactorySource()).ToArray();

    WarmUp();
    var overhead = Measure(allCalls[0].name, allCalls[0].factory);
    Console.WriteLine("Benchmarking...");

    var referenceIterations = Measure(allCalls[1].name, allCalls[1].factory, emptyIterationsPerSecond: overhead);
    for (var i = 2; i < allCalls.Length; ++i)
      Measure(allCalls[i].name, allCalls[i].factory, referenceIterations, overhead);

    return;

    void WarmUp() {
      Console.WriteLine("Warming up...");
      for (var i = 0; i < 100; ++i)
        foreach (var (_, rng) in allCalls)
          rng();
    }

    double Measure(string name, Func<ulong> call, double referenceIterationsPerSecond = 0, double emptyIterationsPerSecond = 0) {
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

      var overheadTicks = 0L;
      if (emptyIterationsPerSecond != 0) {
        var secondsPerEmptyIteration = 1 / emptyIterationsPerSecond;
        var ticksPerEmptyIteration = ticksPerSecond * secondsPerEmptyIteration;
        overheadTicks = (long)(iterations * ticksPerEmptyIteration);
      }

      //ticksTaken -= overheadTicks;

      var iterationsPerTick = (double)iterations / ticksTaken;
      var iterationsPerSecond = iterationsPerTick * ticksPerSecond;
      if (referenceIterationsPerSecond <= 0)
        referenceIterationsPerSecond = iterationsPerSecond;

      Console.WriteLine($"{name,-38}: {iterationsPerSecond,15:0} iterations per second. {iterationsPerSecond / referenceIterationsPerSecond,5:0.0}x ({100 * (iterationsPerSecond / referenceIterationsPerSecond - 1),6:0.0}%)");
      return iterationsPerSecond;
    }

  }
  
}
