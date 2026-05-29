using System.Collections;
using System.Collections.Generic;
using Hawkynt.RandomNumberGenerators.Cryptographic;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;
using Hawkynt.RandomNumberGenerators.QuasiRandom;

namespace RandomNumberGenerators.Tests;

internal static class Generators {

  public const ulong SEED = 131;

  public static IRandomNumberGenerator[] All => [
    new AdditiveCongruentialRandomNumberGenerator(),
    new CombinedLinearCongruentialGenerator(CombinationMode.Additive),
    new ComplementaryMultiplyWithCarry(),
    new FeedbackWithCarryShiftRegister(),
    new InversiveCongruentialGenerator(),
    new KeepItSimpleStupid(CombinationMode.Xor),
    new LaggedFibonacciGenerator(mode: CombinationMode.Additive),
    new LinearCongruentialGenerator(),
    new LinearFeedbackShiftRegister(),
    new MersenneTwister(),
    new MiddleSquare(),
    new MiddleSquareWeylSequence(),
    new Mixmax(),
    new MultiplicativeLinearCongruentialGenerator(),
    new MultiplyWithCarry(),
    new PermutedCongruentialGenerator(),
    new SplitMix64(),
    new SubtractWithBorrow(),
    new WellEquidistributedLongperiodLinear(),
    new WichmannHill(),
    new Xoroshiro128PlusPlus(),
    new XorShift(),
    new XorShiftPlus(),
    new XorShiftStar(),
    new XorWow(),
    new Xoshiro256SS(),
    new BlumBlumShub(),
    new ChaCha20(),
    new BlumMicali(),
    new SelfShrinkingGenerator(),
    new Isaac(),
    new Philox(),
    new Threefry(),
    new Squares(),
    new JenkinsSmallFast(),
    new RomuTrio(),
    new Lehmer128(),
    new Lxm(),
    new WyRand(),
    new Sfc64(),
    new Mrg32k3a(),
    new Trivium(),
    new Xoshiro256Plus(),
    new PermutedCongruentialXslRr(),
    new AesCtrDrbg(),
    new HmacDrbg(),
    new Halton(),
    new Sobol(),
    new AnsiX931(),
    new Yarrow(),
    new Fortuna(),
    new Xoroshiro128Plus(),
    new PermutedCongruentialXshRr(),
    new Rule30(),
    new Salsa20(),
    new HashDrbg(),
    new ChaCha8(),
    new ChaCha12(),
    new Salsa12(),
    new Salsa8(),
    new Xoshiro128StarStar(),
    new Xoshiro128PlusPlus(),
    new Xoshiro128Plus(),
    new Xoshiro512StarStar(),
    new Xoshiro512PlusPlus(),
    new Xoshiro512Plus(),
    new RomuQuad(),
    new RomuDuo(),
    new RomuDuoJr(),
    new TinyMt(),
    new Shishua(),
    new AsconPrf(),
  ];

  public class AllGeneratorsSource : IEnumerable {
    public IEnumerator GetEnumerator() {
      foreach (var gen in All)
        yield return new TestCaseData(gen).SetArgDisplayNames(gen.GetType().Name);
    }
  }

  // Generators excluded from statistical-quality tests because they are
  // historically/structurally weak. They still appear in known-output and
  // seed-reproducibility tests.
  private static readonly HashSet<string> _WEAK_NAMES = new() {
    nameof(AdditiveCongruentialRandomNumberGenerator), // needs warm-up
    nameof(MiddleSquare),                              // famously degenerate (can get stuck)
    nameof(MultiplicativeLinearCongruentialGenerator), // weak low-order bits
    nameof(CombinedLinearCongruentialGenerator),       // weak under default Additive mode
    nameof(Halton),                                    // quasi-random: fills space evenly, not randomly
    nameof(Sobol),                                     // quasi-random: fills space evenly, not randomly
  };

  public class WellBehavedGeneratorsSource : IEnumerable {
    public IEnumerator GetEnumerator() {
      foreach (var gen in All) {
        if (_WEAK_NAMES.Contains(gen.GetType().Name))
          continue;
        yield return new TestCaseData(gen).SetArgDisplayNames(gen.GetType().Name);
      }
    }
  }
}
