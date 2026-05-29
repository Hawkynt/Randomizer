# Hawkynt.RandomNumberGenerators

[![Build](https://github.com/Hawkynt/Randomizer/actions/workflows/Build.yml/badge.svg)](https://github.com/Hawkynt/Randomizer/actions/workflows/Build.yml)[![Last Commit](https://img.shields.io/github/last-commit/Hawkynt/Randomizer?branch=main)](https://github.com/Hawkynt/Randomizer/commits/main/RandomNumberGenerators)[![NuGet](https://img.shields.io/nuget/v/Hawkynt.RandomNumberGenerators)](https://www.nuget.org/packages/Hawkynt.RandomNumberGenerators/)![License](https://img.shields.io/github/license/Hawkynt/Randomizer)

> A comprehensive C# library of random number generators — algorithms across pseudo-random, cryptographically secure, quasi-random and non-uniform distributions, all behind a single `IRandomNumberGenerator` interface plus an `ArbitraryNumberGenerator` helper that exposes ergonomic methods on top of any generator.

The long-form article that motivates and explains every algorithm in this package lives at [Randomizer](https://github.com/Hawkynt/Randomizer).

## Install

```powershell
dotnet add package Hawkynt.RandomNumberGenerators
```

Target framework: `net8.0`. The package is AOT-compatible and trimmable.

## Namespaces

| Namespace                                      | Contains                                                                                                                                                                                                                                                                                                                                                                                                         |
| ---------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Hawkynt.RandomNumberGenerators.Deterministic` | Fast PRNGs: Xoshiro family (256SS/+, 128**/++/+, 512**/++/+), Xoroshiro128++/+, PCG (RXS-M-XS, XSL-RR, XSH-RR), Mersenne Twister, TinyMT, WELL, RomuTrio/Quad/Duo/DuoJr, JSF64, Lehmer128, LXM, SplitMix64, Philox, Threefry, Squares, sfc64, MRG32k3a, WyRand, SHISHUA, LCG, MLCG, XorShift family, Wichmann-Hill, ACORN, LFG, KISS, ICG, LFSR, FCSR, MWC, CMWC, SWB, MixMax, Middle Square (and MSWS), Rule 30 |
| `Hawkynt.RandomNumberGenerators.Cryptographic` | CSPRNGs: ChaCha20/12/8, Salsa20/12/8, AES-CTR DRBG, HMAC-SHA256 DRBG, Hash DRBG (SHA-256), Ascon-PRF, ISAAC-64, Trivium, Blum-Blum-Shub, Blum-Micali, Self-Shrinking Generator, Yarrow, Fortuna, ANSI X9.31                                                                                                                                                                                                      |
| `Hawkynt.RandomNumberGenerators.QuasiRandom`   | Low-discrepancy sequences for Monte Carlo integration: Halton, Sobol                                                                                                                                                                                                                                                                                                                                             |
| `Hawkynt.RandomNumberGenerators.NonUniform`    | Distribution samplers: BoxMuller, MarsagliaPolar, Ziggurat, InverseTransformSampling (Exponential), Poisson, Gamma, Beta, Bernoulli, Binomial, Geometric, ChiSquared, Cauchy, Lognormal, Weibull, Triangular, Pareto, DiscreteUniform, Categorical, Hypergeometric, StudentT, NegativeBinomial                                                                                                                   |
| `Hawkynt.RandomNumberGenerators.Composites`    | `ArbitraryNumberGenerator` — wraps any generator and exposes `NextDouble`, `NextRange`, `NextBoolean`, `Shuffle`, `Choice`, `Sample`, `NextBytes`, `NextGuid`, …                                                                                                                                                                                                                                                 |
| `Hawkynt.RandomNumberGenerators.Interfaces`    | `IRandomNumberGenerator` (the core 64-bit interface), `IDoubleRandomNumberGenerator`, `CombinationMode` enum                                                                                                                                                                                                                                                                                                     |

## The core interface

Every PRNG, CSPRNG and quasi-random sequence in the package implements:

```csharp
public interface IRandomNumberGenerator {
  void Seed(ulong seed);
  ulong Next();
}
```

`Seed` sets the initial state; calling `Seed` again is allowed and will restart the sequence. `Next` returns a uniformly distributed 64-bit value (for PRNG/CSPRNG) or the next element of the sequence (for quasi-random).

## Quick examples

### 1. Pick a generator, produce raw 64-bit values

```csharp
using Hawkynt.RandomNumberGenerators.Deterministic;

var rng = new Xoshiro256SS();
rng.Seed(42);
ulong a = rng.Next();
ulong b = rng.Next();
```

### 2. Use the high-level helpers

```csharp
using Hawkynt.RandomNumberGenerators.Composites;
using Hawkynt.RandomNumberGenerators.Deterministic;

var rng = new ArbitraryNumberGenerator(new Xoshiro256SS());
rng.Seed(42);

double d    = rng.NextDouble();            // uniform [0, 1)
ulong  r    = rng.NextRange(1, 100);       // uniform [1, 100)
bool   flip = rng.NextBoolean(0.7);        // true with probability 0.7
int    sign = rng.NextSign();              // -1 or +1
float  f    = rng.NextSingle();            // uniform [0, 1f)

byte[] buffer = new byte[32];
rng.NextBytes(buffer);                     // fill the span
Guid g = rng.NextGuid();                   // RFC 4122 v4

var deck = Enumerable.Range(1, 52).ToArray();
rng.Shuffle(deck);                         // Fisher-Yates, in place
int card  = rng.Choice(deck);              // one random element
int[] hand = rng.Sample(deck, 5);          // 5 distinct elements (reservoir)
```

### 3. Sample from non-uniform distributions

```csharp
using Hawkynt.RandomNumberGenerators.NonUniform;

var u = new ArbitraryNumberGenerator(new Xoshiro256SS());
u.Seed(42);

var normal = new MarsagliaPolar(u);
var (z0, z1) = normal.Next();              // two independent N(0, 1)

var requests = new Poisson(u, lambda: 4.2);
int arrivals = requests.Next();            // discrete count

var price = new Lognormal(u, mu: 0, sigma: 0.3);
double next = price.Next();                // strictly positive

// Weighted choice from a list
var loot = new Categorical<string>(u, new[] {
  ("Common", 70.0), ("Rare", 25.0), ("Legendary", 5.0)
});
string drop = loot.Next();
```

### 4. Cryptographic use

For anything security-sensitive (session IDs, password salts, key generation) use a CSPRNG from the `Cryptographic` namespace, **not** a PRNG. They are slower but designed to resist state recovery.

```csharp
using Hawkynt.RandomNumberGenerators.Cryptographic;

using var csprng = new AesCtrDrbg();        // implements IDisposable
csprng.Seed(GetEntropyFromOsOrHsm());
ulong sessionId = csprng.Next();
```

> [!WARNING]
> **None of the generators in this package are thread-safe.** Create one instance per thread, or guard access with a lock. The `Cryptographic` generators implement `IDisposable` because they hold underlying `Aes` / `SHA256` resources — wrap them in `using` statements.

## Choosing an algorithm

| Need                                        | Recommended                                                |
| ------------------------------------------- | ---------------------------------------------------------- |
| General-purpose, modern default             | `Xoshiro256SS`                                             |
| Fastest general-purpose (BMI2 platforms)    | `Lehmer128`                                                |
| Parallel / GPU workloads                    | `Philox` or `Threefry`                                     |
| Very large period needed                    | `MersenneTwister` or `WellEquidistributedLongperiodLinear` |
| Tiny state, embedded                        | `JenkinsSmallFast` or `Squares`                            |
| `System.Random`-compatible output (.NET 6+) | `Xoshiro256Plus`                                           |
| Cryptographic                               | `ChaCha20` or `AesCtrDrbg`                                 |
| Monte Carlo integration                     | `Halton` or `Sobol` (run as quasi-Monte-Carlo)             |
| Educational comparison only                 | `LinearCongruentialGenerator`, `MiddleSquare`, ...         |

The full comparison table with state sizes, periods, speed and quality classes is in the [main article](https://github.com/Hawkynt/Randomizer#comparative-tests).

## License

LGPL-3.0-or-later. See the [`LICENSE`](../LICENSE) file.

## Contributing

Issues and pull requests welcome at [Randomizer](https://github.com/Hawkynt/Randomizer).
