using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml.Linq;
using BenchmarkDotNet.Attributes;
using Hawkynt.RandomNumberGenerators.Composites;
using Hawkynt.RandomNumberGenerators.Cryptographic;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;
using Hawkynt.RandomNumberGenerators.NonUniform;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Diagnostics.Runtime.Utilities;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using static System.IO.VolumeExtensions;


//BenchmarkDotNet.Running.BenchmarkRunner.Run<Benchy>();
//return;
var benchy = new Benchy();
benchy.MeasureThroughput();
return;

var generator = new ArbitraryNumberGenerator(new BlumMicali());
const ulong seedNumber = 131;
generator.Seed(seedNumber);

var bytes = generator.ConcatGenerator(1<<10);
var concatHex = bytes.ToHex();
var concatBin = bytes.ToBin();
var aes = generator.CipherGenerator(Aes.Create()).Take(8192).ToArray().ToHex();

var z = new InverseTransformSampling(generator);
var histogram = new ulong[256];
for (var i = 0; i < 1000000; ++i) {
  double random;
  do {
    random = z.Next() / 3.72;
  } while (random is < -1 or > 1);
  random=++random*0.5;

  var limited=(int)(random * 256);
  ++histogram[limited];
}

var values = Enumerable.Range(0, 16).Select(_ => generator.Mask16(0b11100011100111111001111)).ToArray();

var alreadySeen = new HashSet<ulong>();
var counter = 0;
ulong number;
var timer = new Stopwatch();
var lastStats = Stopwatch.StartNew();
do {
  timer.Start();
  number = generator.Next();
  timer.Stop();

  if (lastStats.Elapsed.TotalSeconds > 0.25) {
    Console.WriteLine($"#{counter}: {number} which took {timer.ElapsedMilliseconds}ms ({counter / timer.Elapsed.TotalSeconds:#,###.0} per second).");
    lastStats.Restart();
  } 
  
  ++counter;
} while (alreadySeen.Add(number));
timer.Stop();

Console.WriteLine($"We seeded with {seedNumber} and have repeated ourselves after {counter} steps with {number} which took {timer.ElapsedMilliseconds}ms ({counter / timer.Elapsed.TotalSeconds} per second).");

public class Benchy {
  
  private readonly Random _builtIn = new();
  private readonly Random _builtInOld = new((int)SEED);
  
  private readonly RNGCryptoServiceProvider _crypto = new();
  private readonly byte[] _longStorage = new byte[sizeof(ulong)];

  //[Params(16, 128, 1024)]
  //[Params(1024)]
  public const int N=1024;
  public const ulong SEED = 131;
  public static readonly TimeSpan TIME_TO_MEASURE = TimeSpan.FromSeconds(30);

  [Benchmark(Baseline = true, Description = "Built-In (w/o Seed)")]
  [Arguments("Random", null)]
  public ulong BuiltIn(string name, IRandomNumberGenerator instance) {
    var result = 0UL;
    for (var i = 0; i < N; ++i)
      result ^= (ulong)this._builtIn.NextInt64();

    return result;
  }

  [Benchmark(Baseline = true, Description = "Built-In (w Seed)")]
  [Arguments("Random", null)]
  public ulong BuiltInOldRng(string name, IRandomNumberGenerator instance) {
    var result = 0UL;
    for (var i = 0; i < N; ++i)
      result ^= (ulong)this._builtInOld.NextInt64();

    return result;
  }

  [Benchmark(Baseline = true, Description = "Built-In CSRNG")]
  [Arguments("Random", null)]
  public ulong BuiltInCsrng(string name, IRandomNumberGenerator instance) {
    var result = 0UL;
    var buffer = this._longStorage;
    for (var i = 0; i < N; ++i) {
      this._crypto.GetBytes(buffer);
      result ^= Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference<byte>(buffer));
    }

    return result;
  }

  public void MeasureThroughput() {
    
    (string, Func<ulong>)[] allCalls = new (string, Func<ulong>)[]{
      ("Measuring Overhead", () => SEED),
      ("Reference RNG(with Seed)", () => (ulong)this._builtInOld.NextInt64()),
      ("Reference RNG(without Seed)", () => (ulong)this._builtIn.NextInt64()),
      ("Reference CSRNG", () => {
        this._crypto.GetBytes(this._longStorage);
        return Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference<byte>(this._longStorage));
      })
    }.Concat(
      this.SeededAlgorithmSource().Select(rngData=> {
        var name = (string)rngData[0];
        var rng = (IRandomNumberGenerator)rngData[1];
        return (name, (Func<ulong>)rng.Next);
      })
    ).ToArray();

    WarmUp();
    var overhead = Measure(allCalls[0].Item1, allCalls[0].Item2);
    Console.WriteLine("Benchmarking...");

    var referenceIterations = Measure(allCalls[1].Item1, allCalls[1].Item2, emptyIterationsPerSecond: overhead);
    for (var i = 2; i < allCalls.Length; ++i)
      Measure(allCalls[i].Item1, allCalls[i].Item2, referenceIterations, overhead);
    
    return;

    void WarmUp() {
      Console.WriteLine("Warming up...");
      for (var i = 0; i < 100; ++i)
        foreach ((string name, Func<ulong> rng) in allCalls)
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

      ticksTaken -= overheadTicks;

      var iterationsPerTick = (double)iterations / ticksTaken;
      var iterationsPerSecond = iterationsPerTick * ticksPerSecond;
      if (referenceIterationsPerSecond <= 0)
        referenceIterationsPerSecond = iterationsPerSecond;

      Console.WriteLine($"{name,-38}: {iterationsPerSecond,15:0} iterations per second. {iterationsPerSecond / referenceIterationsPerSecond,5:0.0}x ({100 * (iterationsPerSecond / referenceIterationsPerSecond - 1),6:0.0}%)");
      return iterationsPerSecond;
    }

  }

  public IEnumerable<object[]> SeededAlgorithmSource() {
    foreach (var rngdata in this._AlgorithmSource()) {
      ((IRandomNumberGenerator)rngdata[1]).Seed(SEED);
      yield return rngdata;
    }
  }

  private IEnumerable<object[]> _AlgorithmSource() {
    yield return ["ACORN", new AdditiveCongruentialRandomNumberGenerator()];
    yield return ["Combined LCG (add)", new CombinedLinearCongruentialGenerator(CombinationMode.Additive)];
    yield return ["Combined LCG (sub)", new CombinedLinearCongruentialGenerator(CombinationMode.Subtractive)];
    yield return ["Combined LCG (mul)", new CombinedLinearCongruentialGenerator(CombinationMode.Multiplicative)];
    yield return ["Combined LCG (xor)", new CombinedLinearCongruentialGenerator(CombinationMode.Xor)];
    yield return ["Complementary MWC", new ComplementaryMultiplyWithCarry()];
    yield return ["Feedback with Carry-Shift-Register", new FeedbackWithCarryShiftRegister()];
    yield return ["Inversive Congruential Generator", new InversiveCongruentialGenerator()];
    yield return ["KISS (add)", new KeepItSimpleStupid(CombinationMode.Additive)];
    yield return ["KISS (sub)", new KeepItSimpleStupid(CombinationMode.Subtractive)];
    yield return ["KISS (mul)", new KeepItSimpleStupid(CombinationMode.Multiplicative)];
    yield return ["KISS (xor)", new KeepItSimpleStupid(CombinationMode.Xor)];
    yield return ["Lagged Fibonacci Generator (add)", new LaggedFibonacciGenerator(mode: CombinationMode.Additive)];
    yield return ["Lagged Fibonacci Generator (sub)", new LaggedFibonacciGenerator(mode: CombinationMode.Subtractive)];
    yield return ["Lagged Fibonacci Generator (mul)", new LaggedFibonacciGenerator(mode: CombinationMode.Multiplicative)];
    yield return ["Lagged Fibonacci Generator (xor)", new LaggedFibonacciGenerator(mode: CombinationMode.Xor)];
    yield return ["Linear Congruential Generator", new LinearCongruentialGenerator()];
    yield return ["Linear Feedback Shift Register", new LinearFeedbackShiftRegister()];
    yield return ["Mersenne Twister", new MersenneTwister()];
    yield return ["Middle Square", new MiddleSquare()];
    yield return ["MS with Weyl Sequence", new MiddleSquareWeylSequence()];
    yield return ["MixMax", new Mixmax()];
    yield return ["Multiplicative LCG", new MultiplicativeLinearCongruentialGenerator()];
    yield return ["Multiply with Carry", new MultiplyWithCarry()];
    yield return ["Permutated Congruential Generator", new PermutedCongruentialGenerator()];
    yield return ["SplitMix", new SplitMix64()];
    yield return ["Substract with borrow", new SubtractWithBorrow()];
    yield return ["WELL", new WellEquidistributedLongperiodLinear()];
    yield return ["Wichmann Hill", new WichmannHill()];
    yield return ["XoRoShiRo 128++", new Xoroshiro128PlusPlus()];
    yield return ["XorShift", new XorShift()];
    yield return ["XorShift+", new XorShiftPlus()];
    yield return ["XorShift*", new XorShiftStar()];
    yield return ["XorWow", new XorWow()];
    yield return ["XoShiRo 256 SS", new Xoshiro256SS()];
    yield return ["Blum-Blum-Shub", new BlumBlumShub()];
    yield return ["ChaCha20", new ChaCha20()];
    yield return ["BlumMicali", new BlumMicali()];
    yield return ["Self Shrinking Generator", new SelfShrinkingGenerator()];
  }

  [Benchmark(OperationsPerInvoke = N)]
  [ArgumentsSource(nameof(Benchy.SeededAlgorithmSource))]
  public ulong Algorithm(string name, IRandomNumberGenerator instance) {
    var result = 0UL;
    for (var i = 0; i < N; ++i)
      result ^= instance.Next();

    return result;
  }

}

