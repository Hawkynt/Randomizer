using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Hawkynt.RandomNumberGenerators.Cryptographic;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Randomizer;

internal class RandomSources {

  public const ulong SEED = 131;
  private readonly Random _builtIn = new();
  private readonly Random _builtInOld = new((int)SEED);

  private readonly RNGCryptoServiceProvider _crypto = new();

  public IEnumerable<(string name, Func<ulong> factory)> FactorySource() {
    yield return ("Reference RNG(with Seed)", () => (ulong)this._builtInOld.NextInt64());
    yield return ("Reference RNG(without Seed)", () => (ulong)this._builtIn.NextInt64());

    var longStorage = new byte[sizeof(ulong)];
    yield return ("Reference CSRNG", () => {
          this._crypto.GetBytes(longStorage);
          return Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference<byte>(longStorage));
        }
      );

    foreach (var rngData in this.SeededAlgorithmSource())
      yield return (rngData.name, rngData.generator.Next);
  }
  
  public IEnumerable<(string name, IRandomNumberGenerator generator)> SeededAlgorithmSource() {
    foreach (var rngdata in this._AlgorithmSource()) {
      rngdata.generator.Seed(SEED);
      yield return rngdata;
    }
  }

  private IEnumerable<(string name, IRandomNumberGenerator generator)> _AlgorithmSource() {
    yield return ("ACORN", new AdditiveCongruentialRandomNumberGenerator());
    yield return ("Combined LCG (add)", new CombinedLinearCongruentialGenerator(CombinationMode.Additive));
    yield return ("Combined LCG (sub)", new CombinedLinearCongruentialGenerator(CombinationMode.Subtractive));
    yield return ("Combined LCG (mul)", new CombinedLinearCongruentialGenerator(CombinationMode.Multiplicative));
    yield return ("Combined LCG (xor)", new CombinedLinearCongruentialGenerator(CombinationMode.Xor));
    yield return ("Complementary MWC", new ComplementaryMultiplyWithCarry());
    yield return ("Feedback with Carry-Shift-Register", new FeedbackWithCarryShiftRegister());
    yield return ("Inversive Congruential Generator", new InversiveCongruentialGenerator());
    yield return ("KISS (add)", new KeepItSimpleStupid(CombinationMode.Additive));
    yield return ("KISS (sub)", new KeepItSimpleStupid(CombinationMode.Subtractive));
    yield return ("KISS (mul)", new KeepItSimpleStupid(CombinationMode.Multiplicative));
    yield return ("KISS (xor)", new KeepItSimpleStupid(CombinationMode.Xor));
    yield return ("Lagged Fibonacci Generator (add)", new LaggedFibonacciGenerator(mode: CombinationMode.Additive));
    yield return ("Lagged Fibonacci Generator (sub)", new LaggedFibonacciGenerator(mode: CombinationMode.Subtractive));
    yield return ("Lagged Fibonacci Generator (mul)", new LaggedFibonacciGenerator(mode: CombinationMode.Multiplicative));
    yield return ("Lagged Fibonacci Generator (xor)", new LaggedFibonacciGenerator(mode: CombinationMode.Xor));
    yield return ("Linear Congruential Generator", new LinearCongruentialGenerator());
    yield return ("Linear Feedback Shift Register", new LinearFeedbackShiftRegister());
    yield return ("Mersenne Twister", new MersenneTwister());
    yield return ("Middle Square", new MiddleSquare());
    yield return ("MS with Weyl Sequence", new MiddleSquareWeylSequence());
    yield return ("MixMax", new Mixmax());
    yield return ("Multiplicative LCG", new MultiplicativeLinearCongruentialGenerator());
    yield return ("Multiply with Carry", new MultiplyWithCarry());
    yield return ("Permutated Congruential Generator", new PermutedCongruentialGenerator());
    yield return ("SplitMix", new SplitMix64());
    yield return ("Substract with borrow", new SubtractWithBorrow());
    yield return ("WELL", new WellEquidistributedLongperiodLinear());
    yield return ("Wichmann Hill", new WichmannHill());
    yield return ("XoRoShiRo 128++", new Xoroshiro128PlusPlus());
    yield return ("XorShift", new XorShift());
    yield return ("XorShift+", new XorShiftPlus());
    yield return ("XorShift*", new XorShiftStar());
    yield return ("XorWow", new XorWow());
    yield return ("XoShiRo 256 SS", new Xoshiro256SS());
    yield return ("Blum-Blum-Shub", new BlumBlumShub());
    yield return ("ChaCha20", new ChaCha20());
    yield return ("BlumMicali", new BlumMicali());
    yield return ("Self Shrinking Generator", new SelfShrinkingGenerator());
  }
}
