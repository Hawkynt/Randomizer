using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Hawkynt.RandomNumberGenerators.Composites;
using Hawkynt.RandomNumberGenerators.Cryptographic;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

//BenchmarkDotNet.Running.BenchmarkRunner.Run<Benchy>();
//return;

var generator = new ArbitraryNumberGenerator(new Xoroshiro128PlusPlus());
const ulong seedNumber = 131;
generator.Seed(seedNumber);

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
  
  [Params(10, 100, 1000000)]
  public int N { get; set; } = 1;

  [Benchmark(Baseline = true, Description = "Built-In")]
  [Arguments("Random", null)]
  public ulong BuiltIn(string name, IRandomNumberGenerator instance) {
    var result = 0UL;
    for (var i = 0; i < this.N; ++i)
      result^=(ulong)this._builtIn.NextInt64();

    return result;
  }

  public IEnumerable<object[]> AlgorithmSource() {
    yield return ["ACORN", new AdditiveCongruentialRandomNumberGenerator()];
    yield return ["Combined LCG", new CombinedLinearCongruentialGenerator()];
    yield return ["Complementary MWC", new ComplementaryMultiplyWithCarry()];
    yield return ["Feedback with Carry-Shift-Register", new FeedbackWithCarryShiftRegister()];
    yield return ["Inversive Congruential Generator", new InversiveCongruentialGenerator()];
    yield return ["KISS", new KeepItSimpleStupid()];
    yield return ["Lagged Fibonacci Generator (add)", new LaggedFibonacciGenerator(mode: LaggedFibonacciGenerator.Mode.Additive)];
    yield return ["Lagged Fibonacci Generator (sub)", new LaggedFibonacciGenerator(mode: LaggedFibonacciGenerator.Mode.Subtractive)];
    yield return ["Lagged Fibonacci Generator (mul)", new LaggedFibonacciGenerator(mode: LaggedFibonacciGenerator.Mode.Multiplicative)];
    yield return ["Lagged Fibonacci Generator (xor)", new LaggedFibonacciGenerator(mode: LaggedFibonacciGenerator.Mode.Xor)];
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
    yield return ["Self Shrinking Generator", new SelfShrinkingGenerator()];
  }

  [Benchmark]
  [ArgumentsSource(nameof(AlgorithmSource))]
  public ulong Algorithm(string name, IRandomNumberGenerator instance) {
    var result = 0UL;
    for (var i = 0; i < this.N; ++i)
      result ^= instance.Next();

    return result;
  }

}
