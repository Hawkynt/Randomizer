# Hawkynt.RandomNumberGenerators

[![Build](https://github.com/Hawkynt/Randomizer/actions/workflows/Build.yml/badge.svg)](https://github.com/Hawkynt/Randomizer/actions/workflows/Build.yml)[![Last Commit](https://img.shields.io/github/last-commit/Hawkynt/Randomizer?branch=main)](https://github.com/Hawkynt/Randomizer/commits/main/RandomNumberGenerators)[![NuGet](https://img.shields.io/nuget/v/Hawkynt.RandomNumberGenerators)](https://www.nuget.org/packages/Hawkynt.RandomNumberGenerators/)![License](https://img.shields.io/github/license/Hawkynt/Randomizer)

> A comprehensive C# library of random number generators — algorithms across pseudo-random, cryptographically secure, quasi-random and non-uniform distributions, all behind a single `IRandomNumberGenerator` interface plus an `ArbitraryNumberGenerator` helper that exposes ergonomic methods on top of any generator.

The long-form article that motivates and explains every algorithm in this package lives at [Randomizer](https://github.com/Hawkynt/Randomizer).

## 📦 Installation

```powershell
dotnet add package Hawkynt.RandomNumberGenerators
```

Target framework: `net8.0`. The package is AOT-compatible and trimmable.

## ✨ Features

- One `IRandomNumberGenerator` interface over every generator, so an algorithm is swapped by changing a constructor.
- Deterministic PRNGs, cryptographically secure generators, low-discrepancy quasi-random sequences and non-uniform distribution samplers in one package.
- `ArbitraryNumberGenerator` layers ergonomic helpers — ranges, booleans, shuffles, sampling, GUIDs — on top of any generator.
- AOT-compatible and trimmable.

## 🧩 Support matrix

| Namespace                                      | Contains                                                                                                                                                                                                                                                                                                                                                                                                         |
| ---------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Hawkynt.RandomNumberGenerators.Deterministic` | Fast PRNGs: Xoshiro family (256SS/+, 128**/++/+, 512**/++/+), Xoroshiro128++/+, PCG (RXS-M-XS, XSL-RR, XSH-RR), Mersenne Twister, TinyMT, WELL, RomuTrio/Quad/Duo/DuoJr, JSF64, Lehmer128, LXM, SplitMix64, Philox, Threefry, Squares, sfc64, MRG32k3a, WyRand, SHISHUA, LCG, MLCG, XorShift family, Wichmann-Hill, ACORN, LFG, KISS, ICG, LFSR, FCSR, MWC, CMWC, SWB, MixMax, Middle Square (and MSWS), Rule 30 |
| `Hawkynt.RandomNumberGenerators.Cryptographic` | CSPRNGs: ChaCha20/12/8, Salsa20/12/8, AES-CTR DRBG, HMAC-SHA256 DRBG, Hash DRBG (SHA-256), Ascon-PRF, ISAAC-64, Trivium, Blum-Blum-Shub, Blum-Micali, Self-Shrinking Generator, Yarrow, Fortuna, ANSI X9.31                                                                                                                                                                                                      |
| `Hawkynt.RandomNumberGenerators.QuasiRandom`   | Low-discrepancy sequences for Monte Carlo integration: Halton, Sobol                                                                                                                                                                                                                                                                                                                                             |
| `Hawkynt.RandomNumberGenerators.NonUniform`    | Distribution samplers: BoxMuller, MarsagliaPolar, Ziggurat, InverseTransformSampling (Exponential), Poisson, Gamma, Beta, Bernoulli, Binomial, Geometric, ChiSquared, Cauchy, Lognormal, Weibull, Triangular, Pareto, DiscreteUniform, Categorical, Hypergeometric, StudentT, NegativeBinomial                                                                                                                   |
| `Hawkynt.RandomNumberGenerators.Composites`    | `ArbitraryNumberGenerator` — wraps any generator and exposes `NextDouble`, `NextRange`, `NextBoolean`, `Shuffle`, `Choice`, `Sample`, `NextBytes`, `NextGuid`, …                                                                                                                                                                                                                                                 |
| `Hawkynt.RandomNumberGenerators.Interfaces`    | `IRandomNumberGenerator` (the core 64-bit interface), `IDoubleRandomNumberGenerator`, `CombinationMode` enum                                                                                                                                                                                                                                                                                                     |

### The core interface

Every PRNG, CSPRNG and quasi-random sequence in the package implements:

```csharp
public interface IRandomNumberGenerator {
  void Seed(ulong seed);
  ulong Next();
}
```

`Seed` sets the initial state; calling `Seed` again is allowed and will restart the sequence. `Next` returns a uniformly distributed 64-bit value (for PRNG/CSPRNG) or the next element of the sequence (for quasi-random).

## 🚀 Quick start

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

## 🧭 Choosing an algorithm

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

## 📚 API reference

<!-- API:BEGIN generated by Hawkynt/RepositoryTemplate/package-readme — edit the XML docs in source, not here -->

### Namespace `Hawkynt.RandomNumberGenerators.Composites`

[`ArbitraryNumberGenerator`](#arbitrarynumbergenerator)

#### `ArbitraryNumberGenerator`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `ArbitraryNumberGenerator` | `ArbitraryNumberGenerator(IRandomNumberGenerator rng)` |  |
| `Choice` | `T Choice<T>(IReadOnlyList<T> source)` | Returns one uniformly random element from the given list. |
| `CipherGenerator` | `IEnumerable<byte> CipherGenerator(SymmetricAlgorithm instance)` |  |
| `CipherGenerator` | `IEnumerable<byte> CipherGenerator<TCipher>()` | Generates an infinite sequence of random bytes using a specified symmetric encryption algorithm type in a simulated CTR mode. |
| `Concat128` | `UInt128 Concat128()` | Generates a random 128-bit unsigned integer by concatenating two 64-bit unsigned integers. |
| `Concat256` | `Vector256<ulong> Concat256()` | Generates a random 256-bit vector containing four 64-bit unsigned integers by concatenating four random 64-bit values. |
| `Concat512` | `Vector512<ulong> Concat512()` | Generates a random 512-bit vector containing eight 64-bit unsigned integers by concatenating eight random 64-bit values. |
| `ConcatGenerator` | `IEnumerable<byte> ConcatGenerator()` | Generates an infinite sequence of random bytes by repeatedly slicing a 64-bit unsigned integer into eight 8-bit parts. |
| `ConcatGenerator` | `byte[] ConcatGenerator(int count)` | Generates an array of random bytes with a specified count. |
| `Construct` | `ulong Construct(byte bitsTotal, ulong mask)` | Constructs a random unsigned integer by repeatedly applying a mask to random 64-bit unsigned integers. |
| `FeistelGenerator` | `IEnumerable<byte> FeistelGenerator()` | Generates an infinite sequence of random bytes using a Feistel network-based random number generator. |
| `HashGenerator` | `IEnumerable<byte> HashGenerator(HashAlgorithm instance)` | Generates an infinite sequence of random bytes using a specified hash algorithm. |
| `HashGenerator` | `IEnumerable<byte> HashGenerator<THash>()` | Generates an infinite sequence of random bytes using a specified hash algorithm. |
| `Mask16` | `ushort Mask16(ulong mask)` | Generates a random 16-bit unsigned integer by applying a mask to a random 64-bit unsigned integer. |
| `Mask1` | `bool Mask1(ulong mask)` | Generates a random boolean value by applying a single-bit mask to a random 64-bit unsigned integer. |
| `Mask32` | `uint Mask32(ulong mask)` | Generates a random 32-bit unsigned integer by applying a mask to a random 64-bit unsigned integer. |
| `Mask8` | `byte Mask8(ulong mask)` | Generates a random 8-bit unsigned integer by applying a mask to a random 64-bit unsigned integer. |
| `Mask` | `ulong Mask(ulong mask)` | Generates a random unsigned integer by applying a mask to a random 64-bit unsigned integer. |
| `ModuloRejectionSampling` | `ulong ModuloRejectionSampling(ulong mod)` | Generates a random unsigned integer within a specified range using a combination of modulo and rejection sampling. |
| `Modulo` | `ulong Modulo(ulong mod)` | Generates a random unsigned integer by applying the modulus operation to a random 64-bit unsigned integer. |
| `NextBoolean` | `bool NextBoolean(double probability)` | Generates a weighted boolean returning `true` with the given probability. |
| `NextBytes` | `void NextBytes(Span<byte> buffer)` | Generates an infinite sequence of random bytes using a symmetric encryption algorithm in a simulated CTR mode. |
| `NextDouble` | `double NextDouble()` | Generates a random double-precision floating-point number uniformly distributed between 0 (inclusive) and 1 (exclusive). |
| `NextGuid` | `Guid NextGuid()` | Generates a random RFC 4122 version-4 (random) `Guid`. |
| `NextNonZero` | `ulong NextNonZero()` | Returns a random 64-bit unsigned integer that is guaranteed to be non-zero. |
| `NextRange` | `ulong NextRange(ulong min, ulong max)` | Generates a uniform random unsigned integer in the half-open interval [`min`, `max`). |
| `NextSign` | `int NextSign()` | Returns either +1 or -1 with equal probability. |
| `NextSingle` | `float NextSingle()` | Generates a random single-precision floating-point number uniformly distributed between 0 (inclusive) and 1 (exclusive). |
| `Next` | `ulong Next()` |  |
| `RejectionSampling` | `ulong RejectionSampling(ulong mod)` | Generates a random unsigned integer using rejection sampling to ensure the result is less than the specified modulus. |
| `Sample` | `T[] Sample<T>(IReadOnlyList<T> source, int count)` | Returns `count` uniformly random elements from `source` without replacement. Uses Algorithm R (reservoir sampling) for O(count) memory. |
| `Scale` | `double Scale(double scale)` | Scales a random number to a specified range by multiplying it by a scale factor and normalizing it against `MaxValue`. |
| `Seed` | `void Seed(ulong seed)` |  |
| `Shift16` | `ushort Shift16()` | Generates a random 16-bit unsigned integer by truncating the lower 48 bits of a random 64-bit unsigned integer. |
| `Shift1` | `bool Shift1()` | Generates a random boolean value by truncating the lower 63 bits of a random 64-bit unsigned integer. |
| `Shift32` | `uint Shift32()` | Generates a random 32-bit unsigned integer by truncating the lower 32 bits of a random 64-bit unsigned integer. |
| `Shift8` | `byte Shift8()` | Generates a random 8-bit unsigned integer by truncating the lower 56 bits of a random 64-bit unsigned integer. |
| `Shift` | `ulong Shift(byte bitCount)` | Generates a random unsigned integer by shifting the specified number of bits from a random 64-bit unsigned integer. |
| `Shuffle` | `void Shuffle<T>(IList<T> list)` | In-place Fisher-Yates shuffle of the given list. |
| `Slice16x4` | `ValueTuple<ushort, ushort, ushort, ushort> Slice16x4()` | Generates four random 16-bit unsigned integers by slicing a random 64-bit unsigned integer into four parts. |
| `Slice32x2` | `ValueTuple<uint, uint> Slice32x2()` | Generates two random 32-bit unsigned integers by slicing a random 64-bit unsigned integer into two parts. |
| `Slice8x8` | `ValueTuple<byte, byte, byte, byte, byte, byte, byte, ValueTuple<byte>> Slice8x8()` | Generates eight random 8-bit unsigned integers by slicing a random 64-bit unsigned integer into eight parts. |
| `SplitMix128` | `UInt128 SplitMix128()` | Generates a random 128-bit unsigned integer using the SplitMix64 algorithm for the lower 64 bits. |
| `SplitMix256` | `Vector256<ulong> SplitMix256()` | Generates a random 256-bit vector containing four 64-bit unsigned integers using the SplitMix64 algorithm for three of the elements. |
| `SplitMix512` | `Vector512<ulong> SplitMix512()` | Generates a random 512-bit vector containing eight 64-bit unsigned integers using the SplitMix64 algorithm for seven of the elements. |
| `Sponge16` | `ushort Sponge16()` | Generates a random 16-bit unsigned integer by applying a sponge function to a random 64-bit unsigned integer. |
| `Sponge1` | `bool Sponge1()` | Generates a random boolean value by applying a sponge function to a random 64-bit unsigned integer. |
| `Sponge2` | `byte Sponge2()` | Generates a random 2-bit unsigned integer by applying a sponge function to a random 64-bit unsigned integer. |
| `Sponge32` | `uint Sponge32()` | Generates a random 32-bit unsigned integer by applying a sponge function to a random 64-bit unsigned integer. |
| `Sponge4` | `byte Sponge4()` | Generates a random 4-bit unsigned integer by applying a sponge function to a random 64-bit unsigned integer. |
| `Sponge8` | `byte Sponge8()` | Generates a random 8-bit unsigned integer by applying a sponge function to a random 64-bit unsigned integer. |
| `SpreadBits128` | `UInt128 SpreadBits128(UInt128 mask)` | Spreads bits of a random 64-bit unsigned integer across a 128-bit unsigned integer according to the provided mask. |
| `SpreadBits256` | `Vector256<ulong> SpreadBits256(Vector256<ulong> mask)` | Spreads bits of a random 64-bit unsigned integer across a 256-bit vector according to the provided mask. |
| `SpreadBits512` | `Vector512<ulong> SpreadBits512(Vector512<ulong> mask)` | Spreads bits of a random 64-bit unsigned integer across a 512-bit vector according to the provided mask. |
| `Truncate16` | `ushort Truncate16()` | Generates a random 16-bit unsigned integer by truncating the high bits of a random 64-bit unsigned integer. |
| `Truncate1` | `bool Truncate1()` | Generates a random boolean value by extracting the least significant bit of a random 64-bit unsigned integer. |
| `Truncate32` | `uint Truncate32()` | Generates a random 32-bit unsigned integer by truncating the high bits of a random 64-bit unsigned integer. |
| `Truncate8` | `byte Truncate8()` | Generates a random 8-bit unsigned integer by truncating the high bits of a random 64-bit unsigned integer. |
| `Truncate` | `ulong Truncate(byte bitCount)` | Generates a random unsigned integer by truncating a specified number of bits from a random 64-bit unsigned integer. |

### Namespace `Hawkynt.RandomNumberGenerators.Cryptographic`

[`AesCtrDrbg`](#aesctrdrbg) · [`AnsiX931`](#ansix931) · [`AsconPrf`](#asconprf) · [`BlumBlumShub`](#blumblumshub) · [`BlumMicali`](#blummicali) · [`ChaCha12`](#chacha12) · [`ChaCha20`](#chacha20) · [`ChaCha8`](#chacha8) · [`Fortuna`](#fortuna) · [`HashDrbg`](#hashdrbg) · [`HmacDrbg`](#hmacdrbg) · [`Isaac`](#isaac) · [`Salsa12`](#salsa12) · [`Salsa20`](#salsa20) · [`Salsa8`](#salsa8) · [`SelfShrinkingGenerator`](#selfshrinkinggenerator) · [`Trivium`](#trivium) · [`Yarrow`](#yarrow)

#### `AesCtrDrbg`

AES-CTR DRBG approximation (NIST SP 800-90A Rev. 1). Maintains a 256-bit key and a 128-bit counter; each call encrypts the counter under AES-256 to produce a fresh 128-bit block, increments the counter, and returns the upper 64 bits. After each Generate, the internal state is updated by encrypting two further counter blocks and folding them into (K, V) to provide backtracking resistance.

Implements `IDisposable`, `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `AesCtrDrbg` | `AesCtrDrbg()` |  |
| `Dispose` | `void Dispose()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `AnsiX931`

ANSI X9.31 — the AES update of the older ANSI X9.17 standard (1985). Maintains a secret key K and an internal state V; each step encrypts a counter substitute (originally date/time), XORs that with V, encrypts again to produce the output, then re-encrypts to advance V.

Implements `IDisposable`, `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `AnsiX931` | `AnsiX931()` |  |
| `Dispose` | `void Dispose()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `AsconPrf`

Ascon-PRF — the variable-output pseudorandom function from the Ascon family that won NIST's lightweight cryptography competition (standardised as SP 800-232 in 2025). Uses the same 320-bit Ascon permutation as Ascon-Hash and Ascon-AEAD; here it is driven as a duplex sponge to produce a keystream.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `AsconPrf` | `AsconPrf()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `BlumBlumShub`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `BlumBlumShub` | `BlumBlumShub()` |  |
| `BlumBlumShub` | `BlumBlumShub(ulong p, ulong q, int bitsPerIteration)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `BlumMicali`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `BlumMicali` | `BlumMicali()` |  |
| `BlumMicali` | `BlumMicali(ulong p, ulong g)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `ChaCha12`

ChaCha12 — 12-round variant of `ChaCha20` (Bernstein). The recommended balance of speed and security margin for general CSPRNG use; Rust's standard CSPRNG (rand_chacha) defaults to ChaCha12.

Inherits `ChaCha20`. Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `ChaCha12` | `ChaCha12()` | ChaCha12 — 12-round variant of `ChaCha20` (Bernstein). The recommended balance of speed and security margin for general CSPRNG use; Rust's standard CSPRNG (rand_chacha) defaults to ChaCha12. |

#### `ChaCha20`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `ChaCha20` | `ChaCha20(Vector128<uint> constants)` |  |
| `ChaCha20` | `ChaCha20(int rounds = 20, uint constant1 = 1634760805, uint constant2 = 857760878, uint constant3 = 2036477234, uint constant4 = 1797285236)` |  |
| `ChaCha20` | `ChaCha20(int rounds, Vector128<uint> constants)` |  |
| `ChaCha20` | `ChaCha20(int rounds, ulong constants12, ulong constants34)` |  |
| `ChaCha20` | `ChaCha20(ulong constants12, ulong constants34)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `ChaCha8`

ChaCha8 — 8-round variant of `ChaCha20` (Bernstein). Roughly 2.5× faster than ChaCha20 with a reduced security margin; used as the default PRNG in Go (since 1.22) and recommended for non-security-critical applications where ChaCha's diffusion quality matters but full-strength margin does not.

Inherits `ChaCha20`. Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `ChaCha8` | `ChaCha8()` | ChaCha8 — 8-round variant of `ChaCha20` (Bernstein). Roughly 2.5× faster than ChaCha20 with a reduced security margin; used as the default PRNG in Go (since 1.22) and recommended for non-security-critical applications where ChaCha's diffusion quality matters but full-strength margin does not. |

#### `Fortuna`

Fortuna-style CSPRNG (Schneier and Ferguson, 2003) — the successor to Yarrow. Uses 32 entropy pools and AES-256 in CTR mode for output. Reseeding is triggered when pool 0 fills past a threshold; on the n-th reseed only the pools i where 2^i divides n are mixed in, giving a geometric schedule that frustrates any attempt to compromise the state by poisoning a single pool.

Implements `IDisposable`, `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Fortuna` | `Fortuna()` |  |
| `Dispose` | `void Dispose()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `HashDrbg`

Hash DRBG (SHA-256 instantiation, NIST SP 800-90A Rev. 1). Maintains a 440-bit secret state V and a 440-bit constant C; each Generate call repeatedly hashes V to produce output bytes, then updates V via V = V + H(0x03 \|\| V) + C + reseed_counter to provide backtracking resistance.

Implements `IDisposable`, `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `HashDrbg` | `HashDrbg()` |  |
| `Dispose` | `void Dispose()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `HmacDrbg`

HMAC-SHA-256 DRBG approximation (NIST SP 800-90A Rev. 1). Maintains a 256-bit key K and a 256-bit state V; each Generate call computes V = HMAC(K, V), returns the upper 64 bits, then runs an Update step with empty additional input to provide backtracking resistance.

Implements `IDisposable`, `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `HmacDrbg` | `HmacDrbg()` |  |
| `Dispose` | `void Dispose()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Isaac`

ISAAC-64 (Indirection, Shift, Accumulate, Add, and Count) by Bob Jenkins (1996). A cryptographically secure PRNG using array indirection for non-linearity.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Isaac` | `Isaac()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Salsa12`

Salsa20/12 — 12-round variant of `Salsa20` (Bernstein). The eSTREAM portfolio member: chosen as the "middle" speed-security trade-off in the Salsa family.

Inherits `Salsa20`. Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Salsa12` | `Salsa12()` | Salsa20/12 — 12-round variant of `Salsa20` (Bernstein). The eSTREAM portfolio member: chosen as the "middle" speed-security trade-off in the Salsa family. |

#### `Salsa20`

Salsa20 stream cipher by Daniel J. Bernstein, the direct predecessor to `ChaCha20`. Operates on a 512-bit state (16 × 32-bit words) composed of four constants, an 8-word key, a 2-word block counter and a 2-word nonce. Each output block runs the state through 20 rounds (alternating four column rounds and four row rounds), adds the original state back and serialises the result. The default round count of 20 is the standard "Salsa20/20" recommendation; 12 and 8 round variants ("Salsa20/12", "Salsa20/8") are also defined for higher speed at lower security margin.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Salsa20` | `Salsa20(int rounds = 20)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Salsa8`

Salsa20/8 — 8-round variant of `Salsa20` (Bernstein). The fastest member of the Salsa family at the cost of the narrowest security margin; later cryptanalysis has progressively reduced the gap to a practical attack, so this variant is recommended only for non-cryptographic use such as Monte Carlo simulation.

Inherits `Salsa20`. Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Salsa8` | `Salsa8()` | Salsa20/8 — 8-round variant of `Salsa20` (Bernstein). The fastest member of the Salsa family at the cost of the narrowest security margin; later cryptanalysis has progressively reduced the gap to a practical attack, so this variant is recommended only for non-cryptographic use such as Monte Carlo simulation. |

#### `SelfShrinkingGenerator`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `SelfShrinkingGenerator` | `SelfShrinkingGenerator()` |  |
| `SelfShrinkingGenerator` | `SelfShrinkingGenerator(ulong polynom)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Trivium`

Trivium stream cipher (eSTREAM portfolio, 2008). A hardware-oriented lightweight stream cipher with 288-bit state composed of three NLFSRs. Used as a CSPRNG by generating 64 keystream bits per Next() call.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Trivium` | `Trivium()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Yarrow`

Yarrow-160-style CSPRNG (Schneier, Kelsey, Ferguson, 1999), updated to use AES-256 and SHA-256. The design keeps two entropy pools — a fast pool that reseeds the generator key frequently, and a slow pool that reseeds rarely but with high-confidence entropy. When the fast pool reaches a threshold we mix it into the key via SHA-256 to provide post-compromise recovery.

Implements `IDisposable`, `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Yarrow` | `Yarrow()` |  |
| `Dispose` | `void Dispose()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

### Namespace `Hawkynt.RandomNumberGenerators.Deterministic`

[`AdditiveCongruentialRandomNumberGenerator`](#additivecongruentialrandomnumbergenerator) · [`CombinedLinearCongruentialGenerator`](#combinedlinearcongruentialgenerator) · [`ComplementaryMultiplyWithCarry`](#complementarymultiplywithcarry) · [`FeedbackWithCarryShiftRegister`](#feedbackwithcarryshiftregister) · [`InversiveCongruentialGenerator`](#inversivecongruentialgenerator) · [`JenkinsSmallFast`](#jenkinssmallfast) · [`KeepItSimpleStupid`](#keepitsimplestupid) · [`LaggedFibonacciGenerator`](#laggedfibonaccigenerator) · [`Lehmer128`](#lehmer128) · [`LinearCongruentialGenerator`](#linearcongruentialgenerator) · [`LinearFeedbackShiftRegister`](#linearfeedbackshiftregister) · [`Lxm`](#lxm) · [`MersenneTwister`](#mersennetwister) · [`MiddleSquare`](#middlesquare) · [`MiddleSquareWeylSequence`](#middlesquareweylsequence) · [`Mixmax`](#mixmax) · [`Mrg32k3a`](#mrg32k3a) · [`MultiplicativeLinearCongruentialGenerator`](#multiplicativelinearcongruentialgenerator) · [`MultiplyWithCarry`](#multiplywithcarry) · [`PermutedCongruentialGenerator`](#permutedcongruentialgenerator) · [`PermutedCongruentialXshRr`](#permutedcongruentialxshrr) · [`PermutedCongruentialXslRr`](#permutedcongruentialxslrr) · [`Philox`](#philox) · [`RomuDuo`](#romuduo) · [`RomuDuoJr`](#romuduojr) · [`RomuQuad`](#romuquad) · [`RomuTrio`](#romutrio) · [`Rule30`](#rule30) · [`Sfc64`](#sfc64) · [`Shishua`](#shishua) · [`SplitMix64`](#splitmix64) · [`Squares`](#squares) · [`SubtractWithBorrow`](#subtractwithborrow) · [`Threefry`](#threefry) · [`TinyMt`](#tinymt) · [`WellEquidistributedLongperiodLinear`](#wellequidistributedlongperiodlinear) · [`WichmannHill`](#wichmannhill) · [`WyRand`](#wyrand) · [`XorShift`](#xorshift) · [`XorShiftPlus`](#xorshiftplus) · [`XorShiftStar`](#xorshiftstar) · [`XorWow`](#xorwow) · [`Xoroshiro128Plus`](#xoroshiro128plus) · [`Xoroshiro128PlusPlus`](#xoroshiro128plusplus) · [`Xoshiro128Plus`](#xoshiro128plus) · [`Xoshiro128PlusPlus`](#xoshiro128plusplus) · [`Xoshiro128StarStar`](#xoshiro128starstar) · [`Xoshiro256Plus`](#xoshiro256plus) · [`Xoshiro256SS`](#xoshiro256ss) · [`Xoshiro512Plus`](#xoshiro512plus) · [`Xoshiro512PlusPlus`](#xoshiro512plusplus) · [`Xoshiro512StarStar`](#xoshiro512starstar)

#### `AdditiveCongruentialRandomNumberGenerator`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `AdditiveCongruentialRandomNumberGenerator` | `AdditiveCongruentialRandomNumberGenerator(ulong modulo = 0, int order = 12)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `CombinedLinearCongruentialGenerator`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `CombinedLinearCongruentialGenerator` | `CombinedLinearCongruentialGenerator()` |  |
| `CombinedLinearCongruentialGenerator` | `CombinedLinearCongruentialGenerator(CombinationMode mode)` |  |
| `CombinedLinearCongruentialGenerator` | `CombinedLinearCongruentialGenerator(CombinationMode mode, LinearCongruentialGenerator first, LinearCongruentialGenerator second, params LinearCongruentialGenerator[] others)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `ComplementaryMultiplyWithCarry`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `ComplementaryMultiplyWithCarry` | `ComplementaryMultiplyWithCarry()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `FeedbackWithCarryShiftRegister`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `FeedbackWithCarryShiftRegister` | `FeedbackWithCarryShiftRegister(ulong polynom = 10186522075381554023)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `InversiveCongruentialGenerator`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `InversiveCongruentialGenerator` | `InversiveCongruentialGenerator(ulong a = 6364136223846793005, ulong c = 1442695040888963407, ulong q = 18446744073709551557)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `JenkinsSmallFast`

Jenkins Small Fast 64-bit (JSF64) by Bob Jenkins. A compact, high-quality PRNG with 256-bit state.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `JenkinsSmallFast` | `JenkinsSmallFast()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `KeepItSimpleStupid`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `KeepItSimpleStupid` | `KeepItSimpleStupid()` |  |
| `KeepItSimpleStupid` | `KeepItSimpleStupid(CombinationMode mode)` |  |
| `KeepItSimpleStupid` | `KeepItSimpleStupid(CombinationMode mode, IRandomNumberGenerator first, params IRandomNumberGenerator[] others)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `LaggedFibonacciGenerator`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `LaggedFibonacciGenerator` | `LaggedFibonacciGenerator(int size = 56, int shortLag = 0, int longLag = 21, CombinationMode mode = 0)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Lehmer128`

128-bit Lehmer (MCG) generator. Multiplies a 128-bit state by a 64-bit constant and returns the upper 64 bits. Recommended by Steele and Vigna for its speed and statistical quality, distinct from the classical 64-bit MLCG.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Lehmer128` | `Lehmer128()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `LinearCongruentialGenerator`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `LinearCongruentialGenerator` | `LinearCongruentialGenerator(ulong multiplier = 6364136223846793005, ulong increment = 1442695040888963407, ulong modulo = 0)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `LinearFeedbackShiftRegister`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `LinearFeedbackShiftRegister` | `LinearFeedbackShiftRegister(ulong polynom = 1778762)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Lxm`

L64X128MixRandom from Java 17's LXM generator family. Combines a 64-bit LCG with a Xoroshiro128 subgenerator and applies the Lea64 mixing function.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Lxm` | `Lxm()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `MersenneTwister`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `MersenneTwister` | `MersenneTwister()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `MiddleSquare`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `MiddleSquare` | `MiddleSquare()` |  |
| `MiddleSquare` | `MiddleSquare(ulong modulo = 0)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `MiddleSquareWeylSequence`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `MiddleSquareWeylSequence` | `MiddleSquareWeylSequence(ulong modulo, ulong weylConstant = 13091206342165455529)` |  |
| `MiddleSquareWeylSequence` | `MiddleSquareWeylSequence(ulong weylConstant = 13091206342165455529)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Mixmax`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Mixmax` | `Mixmax()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Mrg32k3a`

MRG32k3a by Pierre L'Ecuyer (1999). A Multiple Recursive Generator combining two parallel third-order recurrences modulo two distinct primes. The default generator in MATLAB, R, SAS, and L'Ecuyer's stochastic simulation library.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Mrg32k3a` | `Mrg32k3a()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `MultiplicativeLinearCongruentialGenerator`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `MultiplicativeLinearCongruentialGenerator` | `MultiplicativeLinearCongruentialGenerator(ulong multiplier = 6364136223846793005, ulong modulo = 0)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `MultiplyWithCarry`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `MultiplyWithCarry` | `MultiplyWithCarry(ulong multiplier = 6364136223846793005, ulong modulo = 0)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `PermutedCongruentialGenerator`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `PermutedCongruentialGenerator` | `PermutedCongruentialGenerator()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `PermutedCongruentialXshRr`

PCG-XSH-RR with 128-bit state and 64-bit output (an extension of O'Neill's pcg_xsh_rr_64_32 variant to 128-bit state). Shares the LCG core with `PermutedCongruentialGenerator` (RXS-M-XS) and `PermutedCongruentialXslRr`, applying the alternative "XOR-Shift-High, Random-Rotation" output transform: XOR a high-shifted copy of the state into itself to mix high bits down, then take the upper 64 bits and rotate by a count derived from the top 6 bits of the state.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `PermutedCongruentialXshRr` | `PermutedCongruentialXshRr()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `PermutedCongruentialXslRr`

PCG-XSL-RR variant of the Permuted Congruential Generator family (O'Neill, 2014). Shares the 128-bit LCG core with the existing `PermutedCongruentialGenerator` (which uses the RXS-M-XS output permutation), but applies the simpler XSL-RR output function: XOR the high half of the state into the low half, then rotate by a count derived from the top bits of the state. XSL-RR is the canonical PCG output transform for 128-bit → 64-bit generators.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `PermutedCongruentialXslRr` | `PermutedCongruentialXslRr()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Philox`

Philox2x64-10 counter-based RNG from the Random123 library (Salmon et al., 2011). Widely used in GPU computing (NumPy, TensorFlow, JAX).

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Philox` | `Philox(int rounds = 10)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `RomuDuo`

RomuDuo by Mark Overton (2020) — the two-register variant of the Romu family. 128-bit state, faster per call than `RomuTrio` at the cost of smaller capacity.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `RomuDuo` | `RomuDuo()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `RomuDuoJr`

RomuDuoJr by Mark Overton (2020) — the simplest, fastest Romu variant. 128-bit state and only three operations per output. Recommended when speed is the top priority and short streams (no parallel overlap concerns) are sufficient.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `RomuDuoJr` | `RomuDuoJr()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `RomuQuad`

RomuQuad by Mark Overton (2020) — the four-register variant of the Romu family. 256-bit state, the largest capacity in the family; recommended when massively-parallel work needs many independent streams (the large state space makes overlap probabilities vanishingly small).

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `RomuQuad` | `RomuQuad()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `RomuTrio`

RomuTrio by Mark Overton (2020). A very fast, high-quality PRNG with 192-bit state using rotation-based mixing.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `RomuTrio` | `RomuTrio()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Rule30`

Rule 30 elementary cellular automaton, used by Stephen Wolfram as a random number generator in early versions of Mathematica. Each cell of a 1D bit array updates simultaneously by the local rule `new = left XOR (center OR right)`. Despite the deterministic, fully local nature of the rule, the centre column of the evolving pattern passes many randomness tests.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Rule30` | `Rule30()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Sfc64`

Small Fast Counting 64-bit (sfc64) by Chris Doty-Humphrey. A counter-augmented chaotic generator. Used in NumPy alongside Philox.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Sfc64` | `Sfc64()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Shishua`

SHISHUA by Thaddée Tyl (2020) — designed around 256-bit SIMD registers (AVX2 on x86, NEON+ on ARM). Each generator round produces 32 random bytes in parallel by mixing two 256-bit state lanes with a counter, shifting, shuffling and adding. Passes BigCrush and 32 TiB of PractRand; one of the fastest known PRNGs of its quality class.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Shishua` | `Shishua()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `SplitMix64`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `SplitMix64` | `SplitMix64()` |  |
| `Next` | `static ulong Next(ref ulong z)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Squares`

Squares RNG by Bernard Widynski (2022). A counter-based generator using iterated squaring with a Weyl sequence key.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Squares` | `Squares()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `SubtractWithBorrow`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `SubtractWithBorrow` | `SubtractWithBorrow()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Threefry`

Threefry2x64-20 counter-based RNG from the Random123 library (Salmon et al., 2011). Based on the Threefish block cipher reduced for RNG use.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Threefry` | `Threefry(int rounds = 20)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `TinyMt`

TinyMT (Tiny Mersenne Twister) by Saito and Matsumoto (2011). A small-state variant of the Mersenne Twister: 127-bit state, period $2^{127}-1$. Designed for memory-constrained environments where the original MT's ~2.5 KiB state is prohibitive.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `TinyMt` | `TinyMt()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `WellEquidistributedLongperiodLinear`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `WellEquidistributedLongperiodLinear` | `WellEquidistributedLongperiodLinear()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `WichmannHill`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `WichmannHill` | `WichmannHill()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `WyRand`

WyRand from the wyhash library by Wang Yi. An extremely fast counter-based generator using 128-bit multiply mixing. Widely used in hash tables and general-purpose applications.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `WyRand` | `WyRand()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `XorShift`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `XorShift` | `XorShift()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `XorShiftPlus`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `XorShiftPlus` | `XorShiftPlus()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `XorShiftStar`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `XorShiftStar` | `XorShiftStar()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `XorWow`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `XorWow` | `XorWow()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Xoroshiro128Plus`

Xoroshiro128+ by Blackman and Vigna. Shares the state-evolution function with `Xoroshiro128PlusPlus` but uses the simpler "+" output scrambler (s0 + s1) instead of the "++" rotation. The lowest three bits are LFSR-linear so this variant targets generating floating-point doubles, not integer work that depends on the low bits.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Xoroshiro128Plus` | `Xoroshiro128Plus()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Xoroshiro128PlusPlus`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Xoroshiro128PlusPlus` | `Xoroshiro128PlusPlus()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Xoshiro128Plus`

Xoshiro128+ by Blackman and Vigna — the 32-bit + variant designed for generating IEEE-754 floats. The three low-order bits are LFSR-linear, so do not use this for integer work that relies on the low bits.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Xoshiro128Plus` | `Xoshiro128Plus()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Xoshiro128PlusPlus`

Xoshiro128++ by Blackman and Vigna — the 32-bit ++ variant. Shares the state-evolution function with `Xoshiro128StarStar` but applies the "++" output scrambler (rotl(s0+s3, 7) + s0).

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Xoshiro128PlusPlus` | `Xoshiro128PlusPlus()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Xoshiro128StarStar`

Xoshiro128** by Blackman and Vigna — the 32-bit-output sibling of `Xoshiro256SS`, with 128 bits of state (four 32-bit words) and the "**" output scrambler. The intended use case is 32-bit / embedded / GPU code; for 64-bit output we concatenate two 32-bit calls.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Xoshiro128StarStar` | `Xoshiro128StarStar()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Xoshiro256Plus`

Xoshiro256+ by Blackman and Vigna. Shares its state-evolution function with `Xoshiro256SS`, but uses the simpler "+" output scrambler (s[0] + s[3]) instead of the "**" multiplication. This is the default PRNG of .NET 6+ `Random`; the low three bits are linear so it is intended for generating floating-point values, not for integer work where the lowest bits matter.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Xoshiro256Plus` | `Xoshiro256Plus()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Xoshiro256SS`

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Xoshiro256SS` | `Xoshiro256SS()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Xoshiro512Plus`

Xoshiro512+ by Blackman and Vigna — 512-bit state, "+" output for floats. Low bits are LFSR-linear; intended for generating doubles, not integer work.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Xoshiro512Plus` | `Xoshiro512Plus()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Xoshiro512PlusPlus`

Xoshiro512++ by Blackman and Vigna — 512-bit state, "++" output scrambler.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Xoshiro512PlusPlus` | `Xoshiro512PlusPlus()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Xoshiro512StarStar`

Xoshiro512** by Blackman and Vigna — the 512-bit-state sibling of `Xoshiro256SS`. Eight 64-bit state words give a $2^{512}-1$ period; the "**" output scrambler provides excellent statistical quality.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Xoshiro512StarStar` | `Xoshiro512StarStar()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

### Namespace `Hawkynt.RandomNumberGenerators.Interfaces`

[`CombinationMode`](#combinationmode) · [`IDoubleRandomNumberGenerator`](#idoublerandomnumbergenerator) · [`IRandomNumberGenerator`](#irandomnumbergenerator)

#### `CombinationMode`

| Value | Numeric | Summary |
| --- | --- | --- |
| `Additive` | `0` |  |
| `Subtractive` | `1` |  |
| `Multiplicative` | `2` |  |
| `Xor` | `3` |  |

#### `IDoubleRandomNumberGenerator`

| Member | Signature | Summary |
| --- | --- | --- |
| `NextPair` | `ValueTuple<double, double> NextPair()` |  |
| `Next` | `double Next()` |  |

#### `IRandomNumberGenerator`

Defines an interface for a random number generator with seeding capability.

| Member | Signature | Summary |
| --- | --- | --- |
| `Next` | `ulong Next()` | Generates the next random 64-bit unsigned integer. |
| `Seed` | `void Seed(ulong seed)` | Seeds the random number generator with the specified seed value. |

### Namespace `Hawkynt.RandomNumberGenerators.NonUniform`

[`Bernoulli`](#bernoulli) · [`Beta`](#beta) · [`Binomial`](#binomial) · [`BoxMuller`](#boxmuller) · [`Categorical<T>`](#categoricalt) · [`Cauchy`](#cauchy) · [`ChiSquared`](#chisquared) · [`DiscreteUniform`](#discreteuniform) · [`Gamma`](#gamma) · [`Geometric`](#geometric) · [`Hypergeometric`](#hypergeometric) · [`InverseTransformSampling`](#inversetransformsampling) · [`Lognormal`](#lognormal) · [`MarsagliaPolar`](#marsagliapolar) · [`NegativeBinomial`](#negativebinomial) · [`Pareto`](#pareto) · [`Poisson`](#poisson) · [`StudentT`](#studentt) · [`Triangular`](#triangular) · [`Weibull`](#weibull) · [`Ziggurat`](#ziggurat)

#### `Bernoulli`

Bernoulli distribution: returns true with probability `probability`, false otherwise. The simplest non-uniform distribution and the building block for Binomial, Geometric and most other discrete distributions.

| Member | Signature | Summary |
| --- | --- | --- |
| `Bernoulli` | `Bernoulli(ArbitraryNumberGenerator generator, double probability = 0.5)` |  |
| `Next` | `bool Next()` |  |

#### `Beta`

Beta-distributed samples on (0, 1) with shape parameters `alpha` and `beta`. Uses the standard construction Beta(α, β) = X / (X + Y) where X ~ Gamma(α, 1) and Y ~ Gamma(β, 1).

| Member | Signature | Summary |
| --- | --- | --- |
| `Beta` | `Beta(ArbitraryNumberGenerator generator, double alpha = 1, double beta = 1)` |  |
| `Next` | `double Next()` |  |

#### `Binomial`

Binomial distribution: count of successes in n independent Bernoulli trials, each with success probability p. Mean = np, variance = np(1-p).

| Member | Signature | Summary |
| --- | --- | --- |
| `Binomial` | `Binomial(ArbitraryNumberGenerator generator, int trials, double probability = 0.5)` |  |
| `Next` | `int Next()` |  |

#### `BoxMuller`

Implements `IDoubleRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `BoxMuller` | `BoxMuller(ArbitraryNumberGenerator generator)` |  |
| `Next` | `ValueTuple<double, double> Next()` |  |

#### `Categorical<T>`

Categorical distribution: returns one of N items, each with its own probability. The discrete generalisation of a Bernoulli — used wherever you need a weighted random choice from a finite set (loot tables, mixture models, weighted A/B).

| Member | Signature | Summary |
| --- | --- | --- |
| `Categorical` | `Categorical(ArbitraryNumberGenerator generator, IEnumerable<ValueTuple<T, double>> weighted)` |  |
| `Next` | `T Next()` |  |

#### `Cauchy`

Cauchy (Lorentz) distribution: a heavy-tailed continuous distribution with undefined mean and variance, parameterised by a location (median) and a half-width-at-half-maximum scale parameter.

| Member | Signature | Summary |
| --- | --- | --- |
| `Cauchy` | `Cauchy(ArbitraryNumberGenerator generator, double location = 0, double scale = 1)` |  |
| `Next` | `double Next()` |  |

#### `ChiSquared`

Chi-squared distribution with k degrees of freedom. Identical to Gamma(shape = k/2, scale = 2), and used in hypothesis testing (variance tests, goodness-of-fit, contingency tables).

| Member | Signature | Summary |
| --- | --- | --- |
| `ChiSquared` | `ChiSquared(ArbitraryNumberGenerator generator, double degreesOfFreedom)` |  |
| `Next` | `double Next()` |  |

#### `DiscreteUniform`

Discrete uniform distribution: returns an integer in [`min`, `max`] (inclusive on both ends), each value equally likely.

| Member | Signature | Summary |
| --- | --- | --- |
| `DiscreteUniform` | `DiscreteUniform(ArbitraryNumberGenerator generator, long min, long max)` |  |
| `Next` | `long Next()` |  |

#### `Gamma`

Gamma-distributed samples with given shape and scale parameters. Uses Marsaglia-Tsang's squeeze method (2000), which is fast and works uniformly for shape ≥ 1. For shape < 1 we sample with shape + 1 and apply Stuart's correction: G(α) = G(α+1) · U^(1/α).

| Member | Signature | Summary |
| --- | --- | --- |
| `Gamma` | `Gamma(ArbitraryNumberGenerator generator, double shape = 1, double scale = 1)` |  |
| `Next` | `double Next()` |  |

#### `Geometric`

Geometric distribution: number of failures before the first success in a sequence of independent Bernoulli(p) trials. Mean = (1-p)/p.

| Member | Signature | Summary |
| --- | --- | --- |
| `Geometric` | `Geometric(ArbitraryNumberGenerator generator, double probability = 0.5)` |  |
| `Next` | `int Next()` |  |

#### `Hypergeometric`

Hypergeometric distribution: count of "success" items drawn when sampling `draws` items without replacement from a population of `populationSize` items, `successCount` of which are successes. The discrete distribution behind the urn problem.

| Member | Signature | Summary |
| --- | --- | --- |
| `Hypergeometric` | `Hypergeometric(ArbitraryNumberGenerator generator, int populationSize, int successCount, int draws)` |  |
| `Next` | `int Next()` |  |

#### `InverseTransformSampling`

Implements `IDoubleRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `InverseTransformSampling` | `InverseTransformSampling(ArbitraryNumberGenerator generator, double lambda = 1)` |  |
| `Next` | `double Next()` |  |

#### `Lognormal`

Log-normal distribution: a continuous distribution whose logarithm is normally distributed. Used wherever multiplicative effects compound — biological growth, stock prices, income distributions, particle sizes.

| Member | Signature | Summary |
| --- | --- | --- |
| `Lognormal` | `Lognormal(ArbitraryNumberGenerator generator, double mu = 0, double sigma = 1)` |  |
| `Next` | `double Next()` |  |

#### `MarsagliaPolar`

Implements `IDoubleRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `MarsagliaPolar` | `MarsagliaPolar(ArbitraryNumberGenerator generator)` |  |
| `Next` | `ValueTuple<double, double> Next()` |  |

#### `NegativeBinomial`

Negative Binomial distribution: number of failures observed before the `successesRequired`-th success in independent Bernoulli(p) trials. Generalises the geometric distribution (which is the special case successes = 1).

| Member | Signature | Summary |
| --- | --- | --- |
| `NegativeBinomial` | `NegativeBinomial(ArbitraryNumberGenerator generator, int successesRequired, double probability = 0.5)` |  |
| `Next` | `int Next()` |  |

#### `Pareto`

Pareto (Type I) distribution: a heavy-tailed power-law distribution on [`scale`, ∞). The classic model for the "80/20 rule" and for any quantity (income, file size, city population, web-page hits) whose extremes dominate the average.

| Member | Signature | Summary |
| --- | --- | --- |
| `Pareto` | `Pareto(ArbitraryNumberGenerator generator, double shape = 1, double scale = 1)` |  |
| `Next` | `double Next()` |  |

#### `Poisson`

Poisson-distributed integer samples with given rate parameter `lambda`. For small `lambda` uses Knuth's multiplicative algorithm; for large `lambda` switches to Atkinson's "PA" rejection method, which keeps the per-sample cost bounded as λ grows.

| Member | Signature | Summary |
| --- | --- | --- |
| `Poisson` | `Poisson(ArbitraryNumberGenerator generator, double lambda = 1)` |  |
| `Next` | `int Next()` |  |

#### `StudentT`

Student's t-distribution with `degreesOfFreedom` df. Symmetric around zero, heavier-tailed than the normal for small df, and approaches the standard normal as df → ∞. The workhorse distribution for small-sample hypothesis testing.

| Member | Signature | Summary |
| --- | --- | --- |
| `StudentT` | `StudentT(ArbitraryNumberGenerator generator, double degreesOfFreedom)` |  |
| `Next` | `double Next()` |  |

#### `Triangular`

Triangular distribution on [`min`, `max`] with mode at `mode`. Popular in risk modeling and PERT analysis where only an optimistic / most-likely / pessimistic estimate is available.

| Member | Signature | Summary |
| --- | --- | --- |
| `Triangular` | `Triangular(ArbitraryNumberGenerator generator, double min = 0, double mode = 0.5, double max = 1)` |  |
| `Next` | `double Next()` |  |

#### `Weibull`

Weibull distribution: a continuous distribution on [0, ∞) widely used in reliability engineering and survival analysis. Shape `shape` < 1 models "infant mortality" (failure rate decreasing with time); shape = 1 reduces to the exponential distribution; shape > 1 models wear-out (failure rate increasing with time).

| Member | Signature | Summary |
| --- | --- | --- |
| `Weibull` | `Weibull(ArbitraryNumberGenerator generator, double shape = 1, double scale = 1)` |  |
| `Next` | `double Next()` |  |

#### `Ziggurat`

Implements `IDoubleRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Ziggurat` | `Ziggurat(ArbitraryNumberGenerator generator)` |  |
| `Next` | `double Next()` |  |

### Namespace `Hawkynt.RandomNumberGenerators.QuasiRandom`

[`Halton`](#halton) · [`Sobol`](#sobol)

#### `Halton`

Halton low-discrepancy sequence (Halton, 1960). A deterministic, space-filling sequence designed for Monte Carlo integration rather than statistical randomness. The base-2 variant (the default) is identical to the van der Corput sequence and reduces to a simple bit reversal of the counter.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Halton` | `Halton(int base = 2)` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

#### `Sobol`

Sobol' low-discrepancy sequence (Sobol', 1967). Like `Halton`, this is a deterministic space-filling sequence rather than a random one, but it is generated incrementally using Gray-code traversal and a table of precomputed "direction numbers". The 1-dimensional Sobol' sequence used here shares its base-2 mathematics with the van der Corput / base-2 Halton sequence but produces points in a different order, making it convenient for incremental Monte Carlo refinement.

Implements `IRandomNumberGenerator`.

| Member | Signature | Summary |
| --- | --- | --- |
| `Sobol` | `Sobol()` |  |
| `Next` | `ulong Next()` |  |
| `Seed` | `void Seed(ulong seed)` |  |

<!-- API:END -->

## 🔌 Dependencies

None. The package targets `net8.0` and uses only the base class library.

## ⚠️ Limitations

- **No generator here is thread-safe.** Create one instance per thread, or guard access with a lock.
- The `Cryptographic` generators implement `IDisposable` because they hold `Aes` / `SHA256` resources — wrap them in `using`.
- The `Deterministic` generators are **not** suitable for anything security-sensitive, however good their statistical quality. Use the `Cryptographic` namespace for session IDs, salts and key material.
- Some generators are included for study and comparison rather than use — `LinearCongruentialGenerator`, `MiddleSquare` and similar are known-weak by design.

## 🤝 Contributing

Issues and pull requests welcome at [Randomizer](https://github.com/Hawkynt/Randomizer).

## ❤️ Support

If this project saves you time or money, consider supporting its development:

[![GitHub Sponsors](https://img.shields.io/badge/GitHub-Sponsor-EA4AAA?logo=githubsponsors)](https://github.com/sponsors/Hawkynt)
[![PayPal](https://img.shields.io/badge/PayPal-Donate-00457C?logo=paypal)](https://www.paypal.me/hawkynt)

## 📜 License

Licensed under LGPL-3.0-or-later — see the repository [LICENSE](https://github.com/Hawkynt/Randomizer/blob/main/LICENSE).
