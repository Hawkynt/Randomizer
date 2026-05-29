using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Hawkynt.RandomNumberGenerators.Cryptographic;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;
using Hawkynt.RandomNumberGenerators.QuasiRandom;

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
    yield return ("LCG (MINSTD)", new MultiplicativeLinearCongruentialGenerator(multiplier: 16807, modulo: 2147483647));
    yield return ("LCG (Numerical Recipes)", new LinearCongruentialGenerator(multiplier: 1664525, increment: 1013904223, modulo: (ulong)1 << 32));
    yield return ("LCG (glibc)", new LinearCongruentialGenerator(multiplier: 1103515245, increment: 12345, modulo: (ulong)1 << 31));
    yield return ("LCG (Borland/Delphi)", new LinearCongruentialGenerator(multiplier: 22695477, increment: 1, modulo: (ulong)1 << 32));
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
    yield return ("ISAAC-64", new Isaac());
    yield return ("Philox 2x64-10", new Philox());
    yield return ("Threefry 2x64-20", new Threefry());
    yield return ("Squares", new Squares());
    yield return ("Jenkins Small Fast (JSF64)", new JenkinsSmallFast());
    yield return ("RomuTrio", new RomuTrio());
    yield return ("Lehmer128", new Lehmer128());
    yield return ("LXM (L64X128Mix)", new Lxm());
    yield return ("WyRand", new WyRand());
    yield return ("sfc64", new Sfc64());
    yield return ("MRG32k3a", new Mrg32k3a());
    yield return ("Trivium", new Trivium());
    yield return ("Xoshiro256+", new Xoshiro256Plus());
    yield return ("PCG XSL-RR", new PermutedCongruentialXslRr());
    yield return ("AES-CTR DRBG", new AesCtrDrbg());
    yield return ("HMAC DRBG", new HmacDrbg());
    yield return ("Halton (base 2)", new Halton());
    yield return ("Sobol (1D)", new Sobol());
    yield return ("ANSI X9.31", new AnsiX931());
    yield return ("Yarrow", new Yarrow());
    yield return ("Fortuna", new Fortuna());
    yield return ("Xoroshiro128+", new Xoroshiro128Plus());
    yield return ("PCG XSH-RR", new PermutedCongruentialXshRr());
    yield return ("Rule 30", new Rule30());
    yield return ("Salsa20", new Salsa20());
    yield return ("Hash DRBG (SHA-256)", new HashDrbg());
    yield return ("ChaCha8", new ChaCha8());
    yield return ("ChaCha12", new ChaCha12());
    yield return ("Salsa12", new Salsa12());
    yield return ("Salsa8", new Salsa8());
    yield return ("Xoshiro128**", new Xoshiro128StarStar());
    yield return ("Xoshiro128++", new Xoshiro128PlusPlus());
    yield return ("Xoshiro128+", new Xoshiro128Plus());
    yield return ("Xoshiro512**", new Xoshiro512StarStar());
    yield return ("Xoshiro512++", new Xoshiro512PlusPlus());
    yield return ("Xoshiro512+", new Xoshiro512Plus());
    yield return ("RomuQuad", new RomuQuad());
    yield return ("RomuDuo", new RomuDuo());
    yield return ("RomuDuoJr", new RomuDuoJr());
    yield return ("TinyMT", new TinyMt());
    yield return ("SHISHUA", new Shishua());
    yield return ("Ascon-PRF", new AsconPrf());
  }
}
