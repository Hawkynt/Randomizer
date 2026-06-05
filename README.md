<!-- omit from toc -->
# Demystifying Randomness

[![License](https://img.shields.io/github/license/Hawkynt/Randomizer)](https://github.com/Hawkynt/Randomizer/blob/main/LICENSE)
[![Language](https://img.shields.io/github/languages/top/Hawkynt/Randomizer?color=8957D5)](https://github.com/Hawkynt/Randomizer)

[![CI](https://github.com/Hawkynt/Randomizer/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Hawkynt/Randomizer/actions/workflows/ci.yml)
![Last Commit](https://img.shields.io/github/last-commit/Hawkynt/Randomizer?branch=main)
![Activity](https://img.shields.io/github/commit-activity/m/Hawkynt/Randomizer)

[![Stars](https://img.shields.io/github/stars/Hawkynt/Randomizer?color=FFD700)](https://github.com/Hawkynt/Randomizer/stargazers)
[![Forks](https://img.shields.io/github/forks/Hawkynt/Randomizer?color=008080)](https://github.com/Hawkynt/Randomizer/network/members)
[![Issues](https://img.shields.io/github/issues/Hawkynt/Randomizer)](https://github.com/Hawkynt/Randomizer/issues)
![Code Size](https://img.shields.io/github/languages/code-size/Hawkynt/Randomizer?color=4CAF50)
![Repo Size](https://img.shields.io/github/repo-size/Hawkynt/Randomizer?color=FF9800)

[![Release](https://img.shields.io/github/v/release/Hawkynt/Randomizer?sort=semver)](https://github.com/Hawkynt/Randomizer/releases/latest)
[![Nightly](https://img.shields.io/github/v/release/Hawkynt/Randomizer?include_prereleases=true&sort=date&label=nightly&color=FF9800)](https://github.com/Hawkynt/Randomizer/releases)
[![Downloads](https://img.shields.io/github/downloads/Hawkynt/Randomizer/total)](https://github.com/Hawkynt/Randomizer/releases)
[![NuGet RandomNumberGenerators](https://img.shields.io/nuget/v/Hawkynt.RandomNumberGenerators?label=RandomNumberGenerators)](https://www.nuget.org/packages/Hawkynt.RandomNumberGenerators/)

> A Deep Dive into Random Number Generators and their Implementations.

# Introduction

In the world of computing, randomness is more than a theoretical concept—it's a cornerstone of numerous applications that drive modern technology. From securing sensitive information to simulating complex systems, random numbers play a pivotal role in ensuring efficiency, security, and fairness.

# Target Audience

This article is not for the faint of heart or those simply looking for quick results. It is intended for readers who are willing to delve deeply into the intricacies of randomness and its implementations. If you are someone who enjoys exploring complex concepts, understanding the underlying principles and questioning how things work beneath the surface, then this article is for you.

Be prepared for an in-depth exploration that reflects the author's current understanding and insights. While every effort has been made to present the information thoroughly and accurately, there is no guarantee that it is complete or error-free. This is a journey into the nuanced world of random number generators, and like all explorations, it is marked by both discovery and recognition of the limits of current knowledge.

# Background

Randomness is crucial in fields such as cryptography, where it underpins the generation of secure encryption keys and cryptographic protocols. Without high-quality randomness, data security could be compromised, making it easier for malicious actors to predict or reproduce secret keys.

In simulations and modeling, randomness enables the accurate representation of real-world phenomena. Techniques like [Monte Carlo simulations](https://en.wikipedia.org/wiki/Monte_Carlo_method) and [Particle filters](https://en.wikipedia.org/wiki/Particle_filter) use random sampling to solve mathematical problems and predict outcomes in finance, engineering, and scientific research. The accuracy of these simulations depends heavily on the quality of the random numbers used.

In gaming, randomness ensures that outcomes are unpredictable and fair, enhancing the gaming experience. Whether it’s the roll of a dice, the shuffle of a deck, or the spawn of in-game items, randomness provides the element of chance that keeps games exciting and engaging.

# Let's dive into it

## What is Randomness?

It refers to the lack of pattern or predictability in events. In the context of computing and mathematics, randomness is the concept of generating sequences of numbers or symbols that cannot be reasonably predicted better than by random chance. This unpredictability is essential to ensure fairness, security, and accurate simulation of real-world phenomena.

> [!TIP]
> The following examples use different programming languages on purpose to give the reader something usable no matter which language he chose to code in.

## Types of Randomness?

### Non-Deterministic (TRNG)

Non-deterministic randomness, often referred to as "true randomness," is derived from physical processes that are inherently unpredictable. These are generated through various physical phenomena such as:

* **Thermal Noise**: Electronic circuits generate thermal noise, which is the random fluctuation of electrical signals caused by the thermal agitation of electrons. This noise can be captured and digitized to produce random numbers.

* **Radioactive Decay**: The decay of radioactive isotopes is a random process governed by quantum mechanics. Geiger counters can detect these decay events, and the timing between decays can be used to generate random numbers.

* **Photon Emission**: The emission of photons from certain materials can also be used as a source of randomness. Devices that detect single photons can record the intervals between emissions to generate random sequences.

* **Quantum Randomness**: Quantum mechanics provides several phenomena that are inherently random. For example, measurements of quantum states, such as the polarization of photons or spin of electrons, yield results that are unpredictable.

* **Atmospheric Noise**: Variations in atmospheric conditions, such as radio noise from lightning or other natural sources, can be captured and used as a source of [true randomness](https://www.random.org).

#### Challenges in Generating True Randomness

While true random number generators offer high-quality randomness, they also come with certain challenges:

* **Hardware Complexity**: They often require specialized hardware to capture and process physical phenomena.

* **Environmental Sensitivity**: The quality of randomness can be affected by environmental factors such as temperature, humidity, and electromagnetic interference.

* **Speed**: They may not generate random numbers as quickly as deterministic algorithms, making them less suitable for applications requiring high-throughput random numbers.

##### True Randomness in Personal Computers

In personal computers, true randomness can be sourced from several hardware-based mechanisms:

* **Peripheral Devices**: Some peripherals, such as network cards, sound cards, and disk drives, can capture environmental noise and other random events to generate random numbers. Additionally, using keyboard, gamepad, and mouse inputs as an additional entropy source is common (especially when no other source is available like on certain [gaming consoles](https://www.smogon.com/ingame/rng/)).

* **Hardware Random Number Generators (HRNGs)**: Many modern hardware comes with built-in HRNGs that utilize physical phenomena to generate random numbers.

  * **CPU-Support**

    * **Intel RDRAND**: Intel processors with the Ivy Bridge architecture and later include the [`RDRAND`](https://www.intel.com/content/www/us/en/developer/articles/guide/intel-digital-random-number-generator-drng-software-implementation-guide.html) instruction, which provides random numbers generated by an onboard hardware RNG. This RNG uses thermal noise as its entropy source.

      Here’s an example in [C](https://en.wikipedia.org/wiki/C_(programming_language)), for using the RDRAND instruction to generate a 64-Bit number:

      ```c
      #include <immintrin.h>
      #include <stdio.h>

      int main() {
        unsigned long long random_number;
        
        if (_rdrand64_step(&random_number)) {
          printf("Random 64-bit number: %016llx\n", random_number);
        } else {
          printf("Failed to generate random number\n");
        }

        return 0;
      }
      ```

    * **AMD RDSEED**: Similar to Intel's `RDRAND`, AMD processors also include the [`RDSEED`](https://www.amd.com/content/dam/amd/en/documents/pdfs/developer/aocl/amd-secure-random-number-generator-library-2.0-whitepaper.pdf) instruction. The source of entropy for RDSEED in AMD processors, like Intel’s implementation, is based on a hardware noise source, typically using thermal noise within the silicon to produce true random bits. This entropy source is then conditioned to ensure high-quality randomness suitable for cryptographic applications.

      Here’s an example in [Rust](https://en.wikipedia.org/wiki/Rust_(programming_language)), utilizing the RDSEED intrinsic to generate a 64-Bit number:

      ```rust
      use std::arch::x86_64::_rdseed64_step;

      fn main() {
          let mut random_number: u64 = 0;
          let success: i32;

          unsafe {
              success = _rdseed64_step(&mut random_number);
          }

          if success == 1 {
              println!("Random 64-bit number: {:016x}", random_number);
          } else {
              println!("Failed to generate random number");
          }
      }
      ```

    * **ARM RNDR**: In ARM architecture, [`RNDR` and `RNDRRS`](https://developer.arm.com/documentation/ka005114/latest/) are instructions used to read random numbers. These instructions are part of the ARMv8.5-A architecture and later versions, providing access to random number generation functionality directly from the CPU.

      Here’s an example in [ARM-Assembler](https://en.wikipedia.org/wiki/Assembly_language), using the `RNDR` register to generate a 64-Bit number:

      ```armasm
      .global _start

      .section .data
      random_number: .space 8

      .section .text
      _start:
          mrs x0, RNDR
          str x0, random_number
          
          mov x8, 93
          mov x0, 0
          svc 0
      ```

  * **Operating System Support**
  
    * **Windows** operating systems provide access to hardware-based randomness through the "Cryptography: Next Generation"-API (CNG) using functions such as [`BCryptGenRandom`](https://learn.microsoft.com/de-de/windows/win32/api/bcrypt/nf-bcrypt-bcryptgenrandom).

      Here’s an example in [C++](https://en.wikipedia.org/wiki/C%2B%2B), using the bcrypt library to generate a 64-Bit number:

      ```c++
      #include <windows.h>
      #include <bcrypt.h>
      #include <iostream>

      #pragma comment(lib, "bcrypt.lib")

      int main() {
          UCHAR random_number[8]; // Generate 64-bit random number
          NTSTATUS status = BCryptGenRandom(
              NULL,
              random_number,
              sizeof(random_number),
              BCRYPT_USE_SYSTEM_PREFERRED_RNG
          );

          if (status == STATUS_SUCCESS) {
              std::cout << "Random 64-bit number: ";
              for (int i = 0; i < sizeof(random_number); i++) {
                  printf("%02x", random_number[i]);
              }
              std::cout << std::endl;
          } else {
              std::cerr << "Failed to generate random number" << std::endl;
          }

          return 0;
      }
      ```

    * **Linux** offers support for HRNGs through the virtual devices [`/dev/random` and `/dev/urandom`](https://en.wikipedia.org/wiki//dev/random), which include entropy from hardware sources. Additionally, the [`getrandom()`](https://man7.org/linux/man-pages/man2/getrandom.2.html) system call can be used for this purpose. The `getrandom()` system call is recommended because it blocks until enough entropy is available, ensuring high-quality random numbers, especially useful during the early boot phase.

      Here’s an example in [Perl](https://en.wikipedia.org/wiki/Perl), using the virtual devices to generate a 64-Bit number:

      ```perl
      use strict;
      use warnings;

      my $random_number;
      open my $fh, '<', '/dev/urandom' or die "Can't open /dev/urandom: $!";
        binmode $fh;
        read $fh, $random_number, 8; # Read 64-bit random number
      close $fh;

      print "Random 64-bit number: ";
      foreach my $byte (split //, $random_number) {
        printf "%02x", ord($byte);
      }
      print "\n";
      ```

    * **macOS** provides access to high-quality random numbers through the [`SecRandomCopyBytes`](https://developer.apple.com/documentation/security/1399291-secrandomcopybytes?language=objc) function in the Security framework.

      Here’s an example in [Python](https://en.wikipedia.org/wiki/Python_(programming_language)), using the OS-function to generate a 64-Bit number:

      ```python
      import ctypes
      import os

      def get_random_bytes(n):
        buf = (ctypes.c_ubyte * n)()
        result = ctypes.cdll.LoadLibrary(ctypes.util.find_library("Security")).SecRandomCopyBytes(None, n, buf)
        if result != 0:
          raise ValueError("Failed to generate random bytes")

        return bytes(buf)

      random_bytes = get_random_bytes(8)  # Generate 64-bit random number
      print("Random 64-bit number: ", random_bytes.hex())
      ```

### Deterministic

This type of randomness is generated by algorithms designed to produce sequences of numbers that appear random but are actually based on some initial value. Unlike true random numbers, these sequences are predictable if the state and algorithm are known. Despite this predictability, high-quality deterministic random number generators can produce sequences that are sufficiently random for many practical applications. All of them use an internal state (or the last known value) and perform various computational operations to generate each number in the sequence, updating the internal state with each invocation to ensure the sequence continues.

### Seeds - Sources of Entropy

They play a crucial role in generating deterministic random numbers. A seed is an initial value provided to a generator that determines the sequence of numbers it produces. The same seed will always result in the same sequence, which is essential for reproducibility in many applications. In most cases the seed will (and should!) not be the first value produced, but a starting point for the internal state machine.

#### Importance of Seeds

* **Reproducibility**: In simulations and testing, reproducibility is vital. Using the same seed allows researchers and developers to regenerate the same sequence of random numbers, ensuring consistent results and enabling the verification of experiments or debugging of programs.

* **Initialization**: Seeds are used to initialize the internal state of the generator. The quality of the seed impacts the quality of the random numbers generated. Poorly chosen seeds can lead to predictable sequences, undermining the randomness.

* **Entropy Sources**: For cryptographic applications, seeds must be derived from high-entropy sources to ensure unpredictability. Entropy sources can include any non-deterministic source, or system states that are difficult to predict.

#### Generating Seeds

* **Hardware Entropy**: Using any available HRNG to obtain entropy, which is then used as a seed.

  Here's an example in [JavaScript](https://en.wikipedia.org/wiki/JavaScript), that obtains a seed value using network latencies:

  ```javascript
  async function getNetworkLatencySeed() {
    return await _getNetworkLatencySeed(
      [
        'https://google.de',
        'https://bing.se',
        'https://br.search.yahoo.com',
        'https://duckduckgo.com',
        'https://ecosia.org',
      ], 
      10
    );
  }

  async function _getNetworkLatencySeed(hosts, count) {
    let seed = 0;

    const getLatency = (host) =>
      new Promise((resolve) => {
        const start = Date.now();
        fetch(host, { mode: 'no-cors' })
          .then(() => resolve(Date.now() - start))
          .catch(() => resolve(Date.now() - start)); // resolve even if the fetch fails
      });

    const combineSeed = (seed, latency) => (seed * 31 + latency) % Number.MAX_SAFE_INTEGER;

    for (let i = 0; i < count; i++) {
      const latency = await getLatency(hosts[i % hosts.length]);
      seed = combineSeed(seed, latency);
    }

    return seed;
  }
  ```

* **Time-based Seeds**: A common method is using the current time as a seed, leveraging the system clock. This can be predictable if the (tick-)exact time the seed was generated is known. However if one knows at least the interval in which the seeding occured, one can still try all ticks within this interval to break randomness.

  Here’s an example in [VB.NET](https://en.wikipedia.org/wiki/Visual_Basic_(.NET)), using the current time to seed a random number generator:

  ```vbnet
  Imports System

  Module Program
      Sub Main()
          Dim seed As Integer = DateTime.Now.Ticks And Integer.MaxValue
          Dim rand As New Random(seed)
          Console.WriteLine("Random 64-bit number: " & rand.NextInt64())
      End Sub
  End Module
  ```

* **Re-Seeding**: It is also very common to update the seed after a given amount of generated numbers or based on a timer, to make the outcomes even more unpredictable.

  Here’s an example in [Pascal](https://en.wikipedia.org/wiki/Pascal_(programming_language)), showing re-seeding after the first number was generated:

  ```pascal
  program GenerateRandom64Bit;

  uses
    SysUtils;

  function Generate64BitRandomNumber: UInt64;
  begin
    Result := (UInt64(Random(MaxInt)) shl 32) or UInt64(Random(MaxInt));
  end;

  var
    RandomNumber: UInt64;
  begin
    // Seed the random number generator with the current time
    Randomize;
    RandomNumber := Generate64BitRandomNumber;
    WriteLn('First 64-bit random number: ', RandomNumber);

    // Re-Seed the random number generator again with the current time
    Randomize;
    RandomNumber := Generate64BitRandomNumber;
    WriteLn('Second 64-bit random number: ', RandomNumber2);

    ReadLn;
  end.
  ```

  > [!WARNING]
  > If [Randomize](https://lazarus-ccr.sourceforge.io/docs/rtl/system/randomize.html) has only second precision (which depends on compiler implementation), this example may produce the same number twice because the same seed may be utilized.

Using a high-entropy seed source, especially in cryptographic applications, ensures that the generator produces a sequence that is difficult to predict and reverse-engineer.

## Types of Random Number Generators (RNG)

### Pseudo-Random Number Generators (PRNG)

#### PRNG Characteristics

* **Deterministic**: PRNGs generate sequences of numbers that are determined by an initial seed. The same seed will always produce the same sequence.

* **Fast**: PRNGs are typically faster than TRNG because they use algorithmic processes rather than physical phenomena.

* **Reproducible**: Essential for applications where reproducibility is necessary, such as simulations and debugging.

#### PRNG Applications

* **Simulations and Modeling**: PRNGs are widely used in simulations, such as statistical simulations, where large quantities of random numbers are needed.

* **Games**: PRNGs provide the randomness required for game mechanics, ensuring fair play and varied experiences.

* **Procedural Generation**: Used in applications like content generation in games and art, where large, diverse datasets are created algorithmically.

### Cryptographically Secure Random Number Generators (CSRNG)

#### CSRNG Characteristics

* **Unpredictable**: CSRNGs are designed to be cryptographically secure, meaning their outputs are unpredictable and resistant to reverse-engineering.

* **High Entropy**: They use high-entropy sources for seeding to ensure the randomness quality.

* **Security**: They incorporate mechanisms to prevent attackers from predicting future outputs even if some bits of internal states are known.

#### CSRNG Applications

* **Cryptography**: Generating keys, salts, nonces, initialization vectors, and other cryptographic values that require high security.

* **Secure Protocols**: Ensuring the unpredictability of random values (like [large primes](https://www.rfc-editor.org/rfc/rfc7919) or [elliptic curves](https://en.wikipedia.org/wiki/Elliptic-curve_cryptography)) in secure communication protocols.

#### CSRNG Construction

Although it is possible to use a stream cipher, a block cipher or a cryptographically secure hash function to create a generator, these constructions often do not have the desired statistical properties and therefore need to be further tweaked to be practically useful.

### Non-Uniform Random Number Generation (NURNG)

#### NURNG Characteristics

* **Distribution-Specific**: NURNGs generate random numbers according to specific probability distributions (e.g., Gaussian, Exponential, Poisson) rather than uniformly across an interval.

* **Transformative**: They typically transform uniformly distributed random numbers (generated by a PRNG) into the desired non-uniform distribution using techniques like the inverse transform method or rejection sampling.

* **Application-Specific**: These generators are tailored to the needs of specific applications where modeling or simulations require data that follow particular statistical properties.

#### NURNG Applications

* **Statistical Modeling**: Used extensively in simulations where the underlying processes follow non-uniform distributions, such as financial modeling, risk assessment, and scientific experiments.

* **Machine Learning**: In generating synthetic data that mimic real-world distributions, NURNGs are vital in training and validating models.

* **Complex System Simulation**: Critical in scenarios like virus transmission models, gene expression, and stock market dynamics, where outcomes must reflect the non-linear characteristics of real-world processes.

### Cascade Construction RNG

Most hardware RNGs, especially those found in CPUs, use a cascade: an on-chip thermal entropy source feeds a *conditioner* that distils raw biased bits into uniform ones, which in turn seeds a fast CSRNG. Intel's `RDRAND`/`RDSEED` pipeline, for example, runs raw thermal noise through an [AES](https://en.wikipedia.org/wiki/Advanced_Encryption_Standard)-[CBC-MAC](https://en.wikipedia.org/wiki/CBC-MAC) conditioner, then drives an AES-CTR-based [NIST DRBG](https://nvlpubs.nist.gov/nistpubs/specialpublications/nist.sp.800-90ar1.pdf) which is reseeded periodically. This two-stage construction produces high-throughput, hard-to-predict random numbers.

## Benchmarking RNGs

This involves evaluating their performance and quality to determine their suitability for various applications. This process covers several key aspects, which we will now dive into.

### Categories

#### Speed

That's a crucial metric for RNGs, especially in applications requiring a high throughput of random numbers, such as simulations and gaming. The generation rate is typically measured in terms of the number of random numbers generated per second or the average time taken to generate a single random number.

##### Methods for determining generation speed

* **Microbenchmarking:** Using precise timers to measure the time taken to generate a large sequence of random numbers.

* **Profiling Tools:** Using performance profiling tools (e.g., [`perf`](https://man7.org/linux/man-pages/man1/perf.1.html) on Linux, [`gprof`](https://man7.org/linux/man-pages/man1/gprof.1.html) on Unix) to analyze and record the time taken by RNG functions.

#### Memory

RAM usage is another critical factor, especially for applications with limited resources. Benchmarking memory usage involves evaluating the amount of memory required by the RNG to maintain its state and generate random numbers.

##### Methods for resource usage analysis

* **Static Analysis:** Evaluating the memory requirements of the RNG's data structures and state variables.

* **Dynamic Analysis:** Monitoring the actual memory usage during the execution of the RNG using profiling tools.

#### Uniformity

Uniform distribution is a fundamental property of high-quality RNGs, ensuring that each possible outcome is equally likely. Benchmarks for uniform distribution assess whether the numbers generated by the RNG follow a uniform distribution.

##### Methods for ensuring equal probability for all outcomes

* **Histogram Analysis:** Collecting a large number of random samples and plotting a histogram to visually inspect the distribution.

* **Chi-Square Test:** Performing a statistical chi-square test to determine if the observed distribution matches the expected uniform distribution.

#### Independence / Correlation

High-quality RNGs produce sequences where each number is independent of the others, meaning there should be no discernible patterns or correlations. Benchmarks for independence and correlation assess whether the generated numbers are truly independent.

##### Methods for ensuring lack of patterns or dependencies in sequences

* **Autocorrelation Test:** Measuring the correlation between numbers in the sequence separated by various lag distances.

* **Runs Test:** Evaluating the randomness of a sequence by analyzing the occurrence and length of runs (consecutive sequences of increasing or decreasing numbers).

#### Period Length

The period length of an RNG is the number of values it produces before the sequence starts repeating. A long period is essential for applications requiring large amounts of random data to ensure the sequence does not repeat prematurely.

##### Methods for assuring long periods in RNGs

* **Theoretical Calculation:** Analyzing the algorithm to determine its maximum period length based on its mathematical properties.

* **Empirical Testing:** Generating random numbers and detecting repetitions to estimate the actual period length.

#### Features

In addition to the above, it could be desirable for a RNG to have certain abilities to support distinct use-cases and scenarios.

* **Jump-Ahead:** The ability to advance an arbitrary number of items in the sequence without calculating intermediate values. This is often used to parallelize the generation of numbers across multiple threads.
* **Jump-Back:** The ability to go back a certain number of values in the sequence for debugging or unrolling in a simulation.
* **Multiple Streams:** The ability to produce independent sequences of numbers from the same seed that don't affect each other.
* **Periodicity Control:** It may be desirable to tweak the number of generated items before repitition occurs for certain simulations.
* **Seeding Flexibility:** Depending on the used environment it may be important to seed with more than a single value (e.g. an array retrieved from a remote source or a TRNG).
* **Cryptographic Strength:** Some PRNGs may show cryptographic strength under certain condition yet still not be as slow as a real CRNG.
* **Equidistribution:** The ability to ensure that k-dimensional tuples of numbers are evenly distributed across the k-dimensional unit cube. This feature is essential for simulations requiring multi-dimensional random inputs (e.g. matrices, vectors, tuples), such as Monte Carlo methods, ensuring uniformity across different dimensions.
* **Party Tricks:** Certain RNGs can be fine-tuned to produce a specific sequence of values at a desired point in the sequence, either from the start or after generating several numbers. While often viewed as a novelty, this can be invaluable in controlled simulations, educational demonstrations, or even testing/debugging by generating known outputs for comparison purposes. For example, an RNG could be set to generate a specific pattern, a ZIP-file, an image, [Shakespeare](https://en.wikipedia.org/wiki/Infinite_monkey_theorem) or anything else that may improve your job decision as a fortune teller on tea-parties ;)

### Test-Suites

[Proving](https://www.unitychain.io/blog/how-to-test-rngs-for-true-randomness/) the quality of an RNG is essential for determining its suitability for different applications. While no RNG can be perfectly random, especially in the case of PRNGs, rigorous testing ensures that its output is statistically close enough to randomness for its intended use. Well-established test suites are used to evaluate various aspects like randomness, uniformity, and independence. Inadequate randomness can lead to predictable patterns, security vulnerabilities, or faulty simulations. By testing RNGs, developers can assess how well the RNG meets the specific randomness requirements of their application.

#### Diehard [^1]

[^1]: [Diehard](https://github.com/eltonlaw/diehard)

These tests are a suite of statistical tests designed by George Marsaglia to assess the quality of RNGs. These tests evaluate various aspects of randomness, including distribution uniformity, independence, and the occurrence of patterns.

##### Key Tests of Diehard

* **Birthday Spacings:** Tests for the spacing between repeated values.

* **Overlapping Permutations:** Checks for the frequency of overlapping permutations in sequences.

* **Rank of Matrices:** Evaluates the rank of matrices constructed from random numbers.

#### TestU01 [^2]

[^2]: [TestU01](https://simul.iro.umontreal.ca/testu01/tu01.html)

This is a comprehensive software library developed by Pierre L'Ecuyer for testing RNGs. It includes several batteries of tests, ranging from small (Crush) to large (BigCrush), providing a thorough evaluation of an RNG's statistical properties.

##### Key Features of TestU01

* **SmallCrush:** A smaller battery for quick assessments
* **Crush:** A moderate battery of tests suitable for initial evaluations.
* **BigCrush:** A large and highly stringent battery for thorough testing.

#### NIST Statistical Test Suite [^3]

[^3]: [NIST-STS](https://nvlpubs.nist.gov/nistpubs/legacy/sp/nistspecialpublication800-22r1a.pdf)

This is a set of tests developed by the National Institute of Standards and Technology to evaluate the quality of random numbers, particularly for cryptographic applications. This suite assesses the randomness of binary sequences.

##### Key Features of NIST

* **Frequency (Monobit) Test:** Evaluates the proportion of ones and zeros.

* **Runs Test:** Analyzes the occurrence and length of runs in the sequence.

* **Approximate Entropy Test:** Measures the entropy, or unpredictability, of the sequence.

### Visualizations

Displaying the output of RNGs helps to intuitively understand their behavior and assess their quality. By using graphical representations, we can observe patterns, uniformity, and potential flaws in the generated sequences.

#### Histogram

This is a graphical representation that displays the distribution of random numbers generated by the RNG. It divides the range of possible values into intervals (bins) and counts how many numbers fall into each bin. A well-behaved non-NURNG should produce a histogram where all bins have approximately the same count, indicating a uniform distribution.

* **Bins:** The number of bins chosen for the histogram can influence the interpretation. Too few bins may obscure variations, while too many bins might exaggerate noise in the distribution. Typically, the number of bins is chosen based on the sample size and the range of the random numbers.

> [!WARNING]
> A short-period RNG may still produce a good looking histogram.

#### Randogramm [^4]

[^4]: [Randogram](https://www.pcg-random.org/posts/visualizing-the-heart-of-some-prngs.html)

Those are powerful visualization tools used to detect patterns or flaws in the output of RNGs. They provide a 2D representation of the state space, offering an intuitive way to assess the randomness of an RNG's output. By visualizing the output in this manner, we can quickly identify regularities or anomalies that might not be apparent through numerical analysis alone. Randograms extract distinct bits from different parts of the output value and convert them into (x, y) coordinates, using pixel intensity to represent occurrence frequency.

We can select bits using:

* Groups of adjacent bits
* Groups of bits from opposite halves of the output
* Groups of bits from the start and their reverse from the end
* Interleaved groups of bits
* Bits from two consecutive outputs of the RNG

##### Visualizing 64-Bit Output with 256x256 Randograms

For a comprehensive analysis of a 64-bit RNG, we can use four separate 256x256 randograms, each visualizing a 16-bit segment of the output. Each randogram will represent a different 16-bit slice, allowing us to scrutinize the entire 64-bit output in detail.

* **Pixel Intensity Representation**: In a randogram, each pixel represents the frequency of occurrence of a specific 16-bit value pair (x, y). The implementation in this repository uses the convention from Melissa O'Neill's [PCG analysis](https://www.pcg-random.org/posts/visualizing-the-heart-of-some-prngs.html): unvisited pairs are *white* (255), each visit *halves* the intensity (1 visit → 50% grey, 2 visits → 25% grey, …), saturating to black by ≈ 8 visits. With 65,536 samples on a 256×256 grid the expected count per cell is exactly 1, so a good RNG produces a uniform random-looking speckle (mix of white, light-grey and medium-grey pixels), while a structured generator concentrates many visits into a small set of cells (those cells go very dark very quickly) leaving the rest white.

This method allows us to identify potential patterns or repetitions that could indicate flaws in the RNG.

##### 8-Bit Visualizations

For smaller-scale analysis, especially when working with simpler or reduced versions of RNGs, an 8-bit segment of the output can be visualized using a tiny randogram. This smaller grid is particularly useful for identifying issues in the least significant bits, which are often more prone to predictability or bias.

* **16x16 Randogram**: In this visualization, each pixel corresponds to one of the possible 8-bit output values. The same intensity rules apply, with pixels turning darker as specific value pairs occur more frequently. A well-functioning RNG should produce a 16x16 randogram that appears random, with no discernible patterns.

##### Interpreting Randograms

* **"Random-looking"**: A high-quality RNG should produce randograms, whether 16x16 or 256x256, that appear as a uniform field of noise, without discernible patterns or regular structures. Such a visualization suggests that the RNG’s output is sufficiently random for practical use.

* **Patterns and Structures**: If a randogram exhibits clear patterns, such as diagonal lines, clusters, or grids, it indicates that the RNG might have structural flaws or that its output is not sufficiently random. These patterns suggest that certain pairs of output values are more likely to occur together, which could compromise the statistical quality of the RNG.

#### Random walks

A random walk visualises the *trajectory* of an RNG rather than its individual outputs. Starting from a fixed point on a 2D grid, each pair of random bits (or single sign per axis) selects one of four moves: up, down, left, or right. Plotting the resulting path over a few thousand steps reveals correlations the eye can spot immediately:

* A *good* generator produces a space-filling, self-crossing tangle that wanders without preferred direction.
* A *biased* generator drifts diagonally or returns to the origin too often.
* A *short-period* generator traces the same loop repeatedly, leaving visible cycles.

The 1D variant — running sum of +1/-1 picked by each random bit — is also useful: under the null hypothesis the maximum deviation grows like √n, which provides a sharp NIST-style test statistic.

#### Non-overlapping Template Matching

The NIST test suite's template-matching test counts how often a fixed bit pattern (e.g., `B = 0b00001`) appears in disjoint windows of the bit stream. The procedure is:

1. Pick a template `B` of length `m` bits (commonly 9, picking patterns expected to occur ~2 times per 1024-bit block).
2. Split the bit stream into blocks of length `M`.
3. In each block, slide a window of size `m`; on a match, count it and advance the window by `m` bits (*non-overlapping*); on a miss, advance by 1 bit.
4. Compare the per-block match counts against the expected mean `μ = (M − m + 1) / 2^m` and variance — a chi-squared statistic across blocks gives the p-value.

The test is sensitive to generators that systematically over- or under-produce specific bit patterns, which simpler tests like bit-frequency may miss.

## PRNG Algorithms

> [!NOTE]
> Within this section the algorithms appear roughly in the order they were added to the project rather than strictly by family. Older classical PRNGs (LCG, XorShift, MT, …) come first; modern counter-based and combined generators (Philox, Threefry, Squares, JSF, RomuTrio, Lehmer128, LXM, sfc64, MRG32k3a) appear toward the end of the PRNG block; the cryptographic algorithms (BBS, Blum-Micali, Self-Shrinking Generator, ChaCha20, ISAAC, Trivium) live in [CSRNG Algorithms](#csrng-algorithms). Cross-references between sections use anchor links.

> [!IMPORTANT]
> **Notation used throughout the formulas below.** All algorithm sections in this document follow the same conventions for mathematical symbols, so the same letter means the same thing whether you are reading about an LCG, a Mersenne Twister, or a chi-squared sampler:
>
> | Symbol | Meaning |
> | --- | --- |
> | $X_i$ | output value at step $i$ |
> | $S_i$ | hidden internal state at step $i$ (when distinct from the output) |
> | $X_0$, $S_0$ | initial value / seed |
> | $a$ | multiplier |
> | $c$ | additive constant (LCG-style increment) |
> | $C_i$ | carry at step $i$ (used by MWC / CMWC / SWB to disambiguate from $c$) |
> | $m$ | modulus |
> | $s$, $l$ | short-lag and long-lag indices (LFG, SWB) |
> | $P(x)$ | feedback polynomial (LFSR, FCSR) |
> | $X_i^{(j)}$ or $Q_i^{(j)}$ | component $j$ of a multi-stream generator (CLCG, WH, ACORN) |
> | $\oplus$ | bitwise XOR |
> | $\ll k$, $\gg k$ | logical left / right shift by $k$ bits |
> | $\lll k$, $\ggg k$ | rotate left / right by $k$ bits |
> | $\lfloor \cdot \rfloor$ | floor / integer truncation |
> | $\mathbb{Z}/m\mathbb{Z}$ | the residue ring of integers modulo $m$ |
>
> Where an algorithm uses a different letter in its primary literature (e.g. Wichmann-Hill's $Q_i^j$, Mixmax's matrix $A$), the original convention is preserved but mapped back to these symbols in prose.

The upcoming algorithms may contain sample implementation in [C#](https://en.wikipedia.org/wiki/C_Sharp_(programming_language)). For them we are gonna use a common interface to generate 64-Bit random integer numbers:

```c#
interface IRandomNumberGenerator {

  void Seed(ulong seed);
  ulong Next();

}
```

If a modulo is present in the calculations, it is implicitly set to $2^{32}$ or $2^{64}$ to cover the full range of `uint` or `ulong`. This means that all arithmetic operations automatically wrap around on [overflow and underflow](https://en.wikipedia.org/wiki/Integer_overflow). Mathematically, this results in all arithmetic being performed in the [residue ring](https://en.wikipedia.org/wiki/Modular_arithmetic) $`\mathbb{Z}/2^{32}\mathbb{Z}`$ or $`\mathbb{Z}/2^{64}\mathbb{Z}`$ (a ring, not a field, since powers of 2 are zero divisors).

### Middle Square (MS) [^5]

[^5]: [MS](http://bit-player.org/2022/the-middle-of-the-square)

This method was proposed by John von Neumann in 1946. It generates a sequence of n-digit pseudorandom numbers by squaring an n-digit starting value and extracting the middle n digits from the result. This process is repeated to generate additional numbers. The value of n must be even to ensure a well-defined middle portion of the digits. The maximum period length for an n-digit generator is bounded by $8^n$ (exponential in n), though most seeds collapse to a fixed point or short cycle long before approaching that bound. It is defined by this formula:

$$Q_i = Q_{i-1}^2$$

$$X_i= \left\lfloor \frac{Q_{i}}{m} \right\rfloor \mod m$$

Where:

* $Q$ is the internal state
* $m$ is the modulo (typical $2^{bits}$)
* $X$ is the result

Although it is historically significant, it is not widely used today due to its poor randomness quality. The middle-square method is often ineffective for practical applications because it typically has a very short period and significant shortcomings. With enough repetitions, it will start to generate the same number repeatedly (for example 0 or 1) or revert to a previous number in the sequence, causing an endless loop.

The code example will work on binary instead of decimal digits.

```cs
class MiddleSquare : IRandomNumberGenerator {

  private UInt128 _state;

  public void Seed(ulong seed) => this._state = (UInt128)seed << 64 | ~seed;

  public ulong Next() => (ulong)((this._state *= this._state) >> 32);

}
```

> [!TIP]
> The NuGet library may contain an implementation that supports a parametrized **modulo**.

### Middle Square Weyl Sequence (MSWS) [^6]

[^6]: [MSWS](https://arxiv.org/pdf/1704.00358)

This method was proposed by Bernard Widynski in 2017. This algorithm improves upon the classic [MS](#middle-square-ms) method by incorporating a Weyl sequence, which helps to avoid the short periods and cycles that the original Middle Square method suffers from. MSWS combines the squaring process of the MS method with an additional [Weyl sequence](https://en.wikipedia.org/wiki/Weyl_sequence) to improve randomness quality and performance.

The Weyl sequence is an integer stepping sequence $0, w, 2w, 3w, ...$ of period $2^{64}$, requiring $w$ to be odd (eitherwise half of all elements will never be taken), which is used to add an additional element of randomness to each iteration. This sequence ensures that the generator does not fall into short cycles or the "zero mechanism" problem where the generator would continue to produce zero outputs. It is defined by this formula:

$$W_i = W_{i-1}+w$$

$$Q_i = Q_{i-1}^2 + W_i$$

$$X_i= \left\lfloor \frac{Q_{i}}{m} \right\rfloor \mod m$$

Where:

* $Q$ is the internal state
* $W$ is the Weyl sequence
* $w$ is the Weyl constant
* $m$ is the modulo (typical $2^{bits}$)
* $X$ is the result

```cs
class MiddleSquareWeylSequence : IRandomNumberGenerator {

  private const ulong _WEYL_CONSTANT = 0xB5AD4ECEDA1CE2A9;
  private UInt128 _state;
  private UInt128 _weyl;

  public void Seed(ulong seed) {
    this._state = (UInt128)seed << 64 | ~seed;
    this._weyl = 0;
  }

  public ulong Next() {
    this._state *= this._state;
    this._state += this._weyl += _WEYL_CONSTANT;
    return (ulong)(this._state >> 32);
  }

}
```

> [!TIP]
> The NuGet library may contain an implementation that supports parametrized **weyl constant** and **modulo**.

### Multiplicative Linear Congruential Generator (MLCG) [^7]

[^7]: [MLCG](https://en.wikipedia.org/wiki/Lehmer_random_number_generator)

Originally introduced by D.H. Lehmer in 1951, this is a simple and efficient method for generating pseudo-random numbers. It uses the following formula:

$$X_i = (a \cdot X_{i-1}) \mod m$$

* $X$ is the sequence of pseudo-random values.
* $m$ is the modulus, $1 < m$.
* $a$ is the multiplier, $1 < a < m$.
* $X_0$ is the seed or start value, $1 \leq X_0 < m$.

In 1988, Stephen K. Park and Keith W. Miller proposed a widely adopted variant of the MLCG with specific parameters: $a = 16807$ and $m=2^{31}-1$ (which is a prime number known as the Mersenne prime). This [choice of parameters](https://www.ams.org/journals/mcom/1999-68-225/S0025-5718-99-00996-5/S0025-5718-99-00996-5.pdf) ensures a long period of $2^{31}-2$, good statistical properties, and efficient computation. However if $X_i$ ever happens to be zero, the generator will continue to produce zeros indefinitely.

```cs
class MultiplicativeLinearCongruentialGenerator : IRandomNumberGenerator {
  
  private const ulong _MULTIPLIER = 2227057010910366687;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed == 0 ? 1 : seed;

  public ulong Next() => _state *= _MULTIPLIER; // implicit mod 2^64

}
```

> [!TIP]
> The NuGet library may contain an implementation that supports parametrized **multiplier** and **modulo**.

### Wichmann-Hill (WH) [^8]

[^8]: [WH](https://www.researchgate.net/publication/220055967_Generating_good_pseudo-random_numbers)

This generator combines three separate [MLCG](#multiplicative-linear-congruential-generator-mlcg)s to produce a sequence of pseudorandom numbers with a very long period and good statistical properties. This generator was introduced by B. A. Wichmann and I. D. Hill in 1982.

The WH uses three individual MLCGs, each with its own modulus, multiplier, and seed. The output of these three generators is combined to produce a single pseudorandom number. The mathematical definitions of the three LCGs are as follows:

**First LCG:**

   $$Q^1_i = (171 \cdot Q^1_{i-1}) \mod 30269$$

**Second LCG:**

   $$Q^2_i = (172 \cdot Q^2_{i-1}) \mod 30307$$

**Third LCG:**

   $$Q^3_i = (170 \cdot Q^3_{i-1}) \mod 30323$$

The combined output $X_i$ of the Wichmann-Hill generator at step $i$ is given by:

$$X_i = Q^1_i + Q^2_i + Q^3_i$$

This combination ensures a long period. For the original 1982 parameters the LCM of the three primes is their product ≈ $2.78 \times 10^{13}$, and the actual period of the combined generator is approximately $6.95 \times 10^{12}$ (because each MLCG has period $m-1$). The output is bounded above by the sum of the three moduli — $90899$ for the original parameters.

> [!NOTE]
> The implementation below uses 64-bit moduli rather than the 1982 primes shown above. This scales the same combined-MLCG construction up to a 64-bit output range; the period and bounds above describe Wichmann & Hill's original parameter set, not the 64-bit variant.

```cs
class WichmannHill : IRandomNumberGenerator {
  private const ulong _MODULUS_X = 18446744073709551557;
  private const ulong _MODULUS_Y = 18446744073709551533;
  private const ulong _MODULUS_Z = 18446744073709551521;
  private const ulong _MULTIPLIER_X = 6364136223846793005;
  private const ulong _MULTIPLIER_Y = 1442695040888963407;
  private const ulong _MULTIPLIER_Z = 1229782938247303441;

  private UInt128 _stateX, _stateY, _stateZ;
  
  public void Seed(ulong seed) {
    // Note: because the moduli are ~ulong.MaxValue, the first DivRem makes q ≈ 0 for almost any seed,
    // so this minimal seeding scheme effectively only varies _stateX. Production code should expand
    // the seed via SplitMix64 (or similar) into three independent state words.
    var (q, r) = Math.DivRem(seed, _MODULUS_X);
    this._stateX = r == 0 ? ~r : r;
    (q, r) = Math.DivRem(q, _MODULUS_Y);
    this._stateY = r == 0 ? ~r : r;
    this._stateZ = q == 0 ? ~q : q;
  }

  public ulong Next() {
    this._stateX = this._stateX * _MULTIPLIER_X % _MODULUS_X;
    this._stateY = this._stateY * _MULTIPLIER_Y % _MODULUS_Y;
    this._stateZ = this._stateZ * _MULTIPLIER_Z % _MODULUS_Z;

    return (ulong)(this._stateX + this._stateY + this._stateZ);
  }
}
```

### Linear Congruential Generator (LCG) [^9]

[^9]: [LCG](https://en.wikipedia.org/wiki/Linear_congruential_generator)

This is one of the oldest and most well-known PRNG algorithms. Introduced by W. E. Thomson and A. Rotenberg in 1958, it generates a sequence of numbers using a piecewise linear equation. The generator is defined by the recurrence relation:

$$X_i = (a \cdot X_{i-1} + c) \mod m$$

Where:

* $X$ is the sequence of pseudo-random values.
* $m$ is the modulus, $0 < m$.
* $a$ is the multiplier, $0 < a < m$.
* $c$ is the increment, $0 \leq c < m$.
* $X_0$ is the seed or start value, $0 \leq X_0 < m$.

The Linear Congruential Generator is a simple and efficient way to produce pseudo-random numbers. However, the quality of the output is highly dependent on the choice of parameters $a$, $c$, and $m$. Properly chosen parameters ensure a long period and good statistical properties, making the LCG suitable for many applications, although it is generally not recommended for cryptographic purposes due to its predictability.

```cs
class LinearCongruentialGenerator : IRandomNumberGenerator {

  private const ulong _MULTIPLIER = 6364136223846793005;
  private const ulong _INCREMENT = 1442695040888963407;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() => this._state = _MULTIPLIER * _state + _INCREMENT; // implicit mod 2^64

}
```

> [!TIP]
> The NuGet library may contain an implementation that supports parametrized **multiplier**, **increment** and **modulo**.

### Combined Linear Congruential Generator (CLCG) [^10]

[^10]: [CLCG](https://en.wikipedia.org/wiki/Combined_linear_congruential_generator)

This is an extension of the [LCG](#linear-congruential-generator-lcg) designed to improve the statistical properties and period length by combining multiple LCGs. This method aims to mitigate the weaknesses inherent in single LCGs, such as short periods and poor distribution, by combining several generators with different parameters.

In a CLCG, multiple LCGs are run in parallel, and their outputs are combined using addition or XOR operations to produce the final random number. The combination of multiple generators with carefully chosen parameters ensures that the resulting sequence has a much longer period and better statistical properties than any individual LCG.

Each $Q^j_i$ is generated by an individual LCG with its own set of parameters:

$$Q^j_i = (a_j \cdot Q^j_{i-1} + c_j) \mod m_j$$

The CLCG can be defined using one of the following formulas, where $k$ LCGs are combined:

$$X_i = \left [ \sum_{j=1}^k {(a_j \cdot Q^j_{i-1} + c_j) \mod m_j} \right ] \mod m = \left [ \sum_{j=1}^k {Q^j_i} \right ] \mod m = [Q^1_i+\cdots+Q^k_i] \mod m$$

$$X_i = \left [ \prod_{j=1}^k {(a_j \cdot Q^j_{i-1} + c_j) \mod m_j} \right ] \mod m = \left [ \prod_{j=1}^k {Q^j_i} \right ] \mod m = [Q^1_i \cdot \cdots \cdot Q^k_i] \mod m$$

Where:

* $X$ is the sequence of pseudo-random values.
* $Q^j_i$ is the sequence of pseudo-random values from the $j$-th LCG.
* $m$ is the modulus of the CLCG.
* $m_j$ is the modulus of the $j$-th LCG.
* $a_j$ is the multiplier of the $j$-th LCG.
* $c_j$ is the increment of the $j$-th LCG.
* $Q^j_0$ is the seed of the $j$-th LCG.

```cs
class CombinedLinearCongruentialGenerator : IRandomNumberGenerator {

  private ulong _state1;
  private ulong _state2;

  private const ulong _A1 = 6364136223846793005;  // Multiplier for LCG1
  private const ulong _C1 = 1442695040888963407;  // Increment  for LCG1
  
  private const ulong _A2 = 3935559000370003845;  // Multiplier for LCG2
  private const ulong _C2 = 2691343689449507681;  // Increment  for LCG2
  
  public void Seed(ulong seed) {
    this._state1 = seed;
    this._state2 = seed ^ 0x5DEECE66D; // Ensure different seeds for the two LCGs
  }

  public ulong Next() // implicit mod 2^64
    => (this._state1 = _A1 * this._state1 + _C1) 
     + (this._state2 = _A2 * this._state2 + _C2)
  ;
    
}
```

> [!TIP]
> The NuGet library may contain an implementation that supports more than two LCGs and more combinatorial operators than just addition.

### Inversive Congruential Generator (ICG) [^11]

[^11]: [ICG](https://www.ams.org/journals/mcom/1991-56-193/S0025-5718-1991-1052092-X/S0025-5718-1991-1052092-X.pdf)

These are a type of nonlinear congruential pseudorandom number generator that use the modular multiplicative inverse to generate the next number in a sequence. These generators offer excellent uniformity properties and longer periods compared to LCGs.

The standard formula for an ICG, modulo a prime number $m$, is:

$$
X_i =
\begin{cases}
  (a \cdot X_{i-1}^{-1} + c) \mod m & \text{if } X_{i-1} \neq 0 \\
  c & \text{if } X_{i-1} = 0
\end{cases}
$$

Where:

* $X_i$ is the current value in the sequence.
* $a$ is the multiplier.
* $c$ is the increment.
* $m$ is the modulus.
* $X_{i-1}^{-1}$ is the modular multiplicative inverse of $X_{i-1}$ modulo $m$.

The maximum period for an ICG is $m$, provided $a$ and $c$ are chosen appropriately, such that the polynomial $f(x) = x^2 - cx - a$ is primitive over the finite field $\mathbb{F}_m$.

```cs
class InversiveCongruentialGenerator : IRandomNumberGenerator {

  private ulong _state;
  private const ulong _A = 6364136223846793005;
  private const ulong _C = 1442695040888963407;
  private const ulong _M = 18446744073709551557;

  public void Seed(ulong seed) => this._state = seed % _M;

  public ulong Next() {
    return this._state = this._state == 0 ? _C : (_A * ModInverse(this._state, _M) + _C) % _M;

    ulong ModInverse(ulong value, ulong modulus) {
      ulong t = 0, newT = 1;
      ulong r = modulus, newR = value;

      while (newR != 0) {
        ulong quotient = r / newR;
        ulong tProduct = quotient * newT;
        ulong rProduct = quotient * newR;
        
        (t, newT) = (newT, tProduct > t ? modulus + t - tProduct : t - tProduct);
        (r, newR) = (newR, rProduct > r ? modulus + r - rProduct : r - rProduct);
      }

      return r > 1 ? 0 : t;
    }
  }

}
```

### Multiply with Carry (MWC) [^12]

[^12]: [MWC](http://www.cs.engr.uky.edu/~klapper/pdf/MWC.pdf)

These are a class of PRNGs that combines multiplication, addition, and a carry mechanism to produce sequences of random numbers, introduced by George Marsaglia in 1994.

The key difference between MWC and [LCG](#linear-congruential-generator-lcg) is the introduction of a carry value in MWC. This carry value is carried over to the next step, which helps in achieving longer periods and better statistical properties compared to traditional LCGs.

The general formula for an MWC generator is as follows:

$$X_i = (a \cdot X_{i-1} + C_{i-1}) \mod m$$

Where:

* $X_{i}$ is the current state.
* $a$ is the multiplier.
* $C_{i-1}$ is the carry from the previous step.
* $m$ is the modulus.

The carry value $C_i$ is updated in each step as follows:

$$C_i = \left\lfloor \frac{X_i}{m} \right\rfloor$$

In this mechanism, the next state $X_i$ depends not only on the current state $X_{i-1}$ and the multiplier $a$, but also on the carry value $C_{i-1}$, which introduces a non-linear component to the generator, distinguishing it from the linear nature of LCGs.

```cs
class MultiplyWithCarry : IRandomNumberGenerator {
  private const ulong A = 6364136223846793005UL;  // Multiplier
  private ulong _state;                           // Current state
  private ulong _carry;                           // Carry value

  public void Seed(ulong seed) {
      _state = seed;
      _carry = ~seed;
  }

  public ulong Next() { // implicit mod 2^64
    UInt128 state = this._state;
    state *= A;
    state += this._carry;

    this._state = (ulong)state;
    this._carry = (ulong)(state >> 64);

    return _state;
  }
}
```

> [!TIP]
> The NuGet library may contain an implementation that supports a parametrized **multiplier** and **modulo**.

### XorShift (XS) [^13]

[^13]: [XS](https://www.jstatsoft.org/index.php/jss/article/view/v008i14/916)

This method was introduced by George Marsaglia in 2003, and is a class of extremely fast and simple RNGs. These generators operate by repeatedly applying the exclusive-or ([xor](https://en.wikipedia.org/wiki/Exclusive_or)) operation combined with bit shifts to produce sequences of random numbers. XS RNGs can generate sequences of integers with periods $2^k - 1$ for values of $k$ like $32, 64, 96, 128, 160$ and $192$.

The algorithm uses a binary vector space model where each step involves applying a linear transformation over the binary vectors. The primary operations involve xor'ing a computer word with a shifted (=multiplied) version of itself, either left or right. For instance, the operations

$S_i \leftarrow S_i \oplus ( S_i \ll a )$ shifts $S_i$ left by $a$ bits and xor's the result with $S_i$ whereas

$S_i \leftarrow S_i \oplus ( S_i \gg b )$ shifts $S_i$ right by $b$ bits and xor's the result with $S_i$.

The full XorShift step combining one left- and one right-shift is therefore:

$$
\begin{aligned}
S_i &\leftarrow S_{i-1} \oplus (S_{i-1} \ll a) \\
S_i &\leftarrow S_i \oplus (S_i \gg b) \\
X_i &= S_i
\end{aligned}
$$

with $a = 7$ and $b = 9$ in this 64-bit implementation. Many shift-pair combinations exist that give full-period $2^{64}-1$; Marsaglia tabulated them in his original 2003 paper.

```cs
class XorShift : IRandomNumberGenerator {
  
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed == 0 ? 1 : seed;

  public ulong Next() {
    ulong s = this._state;
    s ^= s << 7;
    s ^= s >> 9;
    return this._state = s;
  }

}
```

### XorShift+ (XS+) [^14]

[^14]: [XS+](https://arxiv.org/pdf/1404.0390)

This method, introduced by Sebastiano Vigna, is an extension of XS generators. Instead of using a multiplication operation, XS+ returns the sum of two consecutive outputs from the [XS](#xorshift-xs) generator. This approach helps to eliminate linear artifacts typically associated with linear operations in
$\mathbb{Z}/2^{32}\mathbb{Z}$. The XS+ generators have been adopted in various JavaScript engines, including those in Chrome, Firefox, Safari, and Microsoft Edge. They are faster and have better statistical properties than some earlier RNGs, passing rigorous tests like BigCrush from the TestU01 suite.

It is also designed to avoid weaknesses in lower bits that were observed in some other RNGs, ensuring a uniform distribution of random numbers.

The state is two 64-bit words $(S^x, S^y)$. Each step:

$$
\begin{aligned}
t &= S^x \\
u &= S^y \\
t &\leftarrow t \oplus (t \ll 23) \\
S^x &\leftarrow u \\
S^y &\leftarrow t \oplus u \oplus (t \gg 17) \oplus (u \gg 26) \\
X_i &= S^y + u
\end{aligned}
$$

The shift triple $(23, 17, 26)$ is the parameter set from Vigna's 2014 paper, chosen by exhaustive search to maximise statistical quality.

```cs
class XorShiftPlus : IRandomNumberGenerator {

  private ulong _stateX, _stateY;

  public void Seed(ulong seed) {
    this._stateX = seed == 0 ? 1 : seed;
    this._stateY = ~seed == 0 ? 1 : ~seed;
  }

  public ulong Next() {
    ulong x = this._stateX;
    ulong y = this._stateY;

    x ^= x << 23;
    x ^= x >> 17;
    x ^= y ^ (y >> 26);

    return (this._stateX = y) + (this._stateY = x);
  }

}
```

### XorShift*(XS*) [^15]

[^15]: [XS*](https://rosettacode.org/wiki/Pseudo-random_numbers/Xorshift_star)

These generators are an enhancement over the basic [XS](#xorshift-xs) generators, incorporating an invertible multiplication (modulo the word size) as a non-linear transformation to the output. This technique was suggested by Marsaglia to address linear artifacts inherent in pure XS generators. The multiplication step ensures that the output sequence is equidistributed in the maximum possible dimension, which means it spans all possible values in its range more uniformly than the basic XS. These generators are designed to produce high-quality pseudorandom numbers and are widely used due to their simplicity and efficiency.

State update (shift triple $(a, b, c) = (12, 25, 27)$ in this implementation) then a final multiplication by $M$ to scramble the output:

$$
\begin{aligned}
S_i &\leftarrow S_{i-1} \oplus (S_{i-1} \gg a) \\
S_i &\leftarrow S_i \oplus (S_i \ll b) \\
S_i &\leftarrow S_i \oplus (S_i \gg c) \\
X_i &= S_i \cdot M \pmod{2^{64}}
\end{aligned}
$$

with $M = \mathtt{0x2545F4914F6CDD1D}$, chosen to maximise the avalanche of the bottom bits.

```cs
class XorShiftStar : IRandomNumberGenerator {
  
  private const ulong _MULTIPLICATOR = 0x2545F4914F6CDD1D;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed == 0 ? 1 : seed;

  public ulong Next() {
    ulong s = this._state;
    s ^= s >> 12;
    s ^= s << 25;
    s ^= s >> 27;

    return (this._state = s) * _MULTIPLICATOR;
  }

}
```

### XorWow [^16]

[^16]: [XorWow](https://www.pcg-random.org/downloads/snippets/uncxorwow.c)

This is another variant introduced by Marsaglia, that adds a simple additive counter (a Weyl sequence) to the output of a [XS](#xorshift-xs) generator. This method extends the period and enhances the randomness of the output. The XorWow generator was historically the default RNG in Nvidia's cuRAND library (`CURAND_RNG_PSEUDO_DEFAULT`); modern cuRAND now offers Philox and MRG32k3a alongside it for many simulation workloads.

```cs
class XorWow : IRandomNumberGenerator {
  
  private const uint _WEYL_CONSTANT = 362437;
  private uint _stateX, _stateY, _stateZ, _stateW, _stateV, _weyl;
  
  public void Seed(ulong seed) {
    uint low = (uint)seed;
    uint high = (uint)(seed >> 32);

    uint s0 = low ^ 0xAAD26B49;
    uint s1 = high ^ 0xF7DCEFDD;
    uint t0 = 1099087573 * s0;
    uint t1 = 2591861531 * s1;

    this._weyl = 6615241 + t1 + t0;
    this._stateX = 123456789 + t0;
    this._stateY = 362436069 ^ t0;
    this._stateZ = 521288629 + t1;
    this._stateW = 88675123 ^ t1;
    this._stateV = 5783321 + t0;
  }

  public ulong Next() {
    uint high = Next32();
    uint low = Next32();
    return (ulong)high << 32 | low;

    uint Next32() {
      uint x = this._stateX;
      x ^= x >> 2;
      x ^= x << 1;

      (this._stateX, this._stateY, this._stateZ, this._stateW) 
      = (this._stateY, this._stateZ, this._stateW, this._stateV)
      ;

      uint v = this._stateV;
      v ^= v << 4;
      v ^= x;

      return (this._stateV = v) + (this._weyl += _WEYL_CONSTANT);
    }
  }

}
```

### SplitMix (SM) [^17]

[^17]: [SM](https://gee.cs.oswego.edu/dl/papers/oopsla14.pdf)

This generator was introduced by Guy Steele, Doug Lea, and Christine Flood in their 2014 OOPSLA paper *Fast Splittable Pseudorandom Number Generators*. The 64-bit variant (SplitMix64) was later popularized by Sebastiano Vigna as the recommended seeding helper for the Xoshiro/Xoroshiro family. It combines adding the golden gamma constant ($2^{64}/\phi$ where $\phi = \frac{1 + \sqrt{5}}{2}$) with David Stafford's Mix13 variant of the [MurmurHash](https://en.wikipedia.org/wiki/MurmurHash)3 finalizer. It is primarily used to initialize the states of other more complex generators because of its excellent statistical properties and simplicity.

State update is an additive Weyl sequence; the output is Stafford's Mix13 finaliser applied to the new state:

$$
\begin{aligned}
S_i &\leftarrow S_{i-1} + \gamma \quad (\bmod \; 2^{64}) \\
z &\leftarrow (S_i \oplus (S_i \gg 30)) \cdot \mathtt{0xBF58476D1CE4E5B9} \\
z &\leftarrow (z \oplus (z \gg 27)) \cdot \mathtt{0x94D049BB133111EB} \\
X_i &= z \oplus (z \gg 31)
\end{aligned}
$$

with $\gamma = \mathtt{0x9E3779B97F4A7C15}$.

```cs
class SplitMix64 : IRandomNumberGenerator {

  private const ulong _GOLDEN_GAMMA = 0x9E3779B97F4A7C15;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() => Next(ref this._state);

  public static ulong Next(ref ulong z) {
    z += _GOLDEN_GAMMA;
    z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
    z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
    z ^= z >> 31;
    return z;
  }

}
```

### XoShiRo (XSR) [^18]

[^18]: [XSR](https://prng.di.unimi.it/)

This algorithm is designed for high performance and quality. It uses a combination of XOR operations, bitwise shifts, and bitwise rotations to generate random numbers. XSR256** is used in various software implementations, including the GNU Fortran compiler, Lua 5.4, and the .NET framework from version 6.0 onwards.

The state is four 64-bit words $S = (S_0, S_1, S_2, S_3)$. The state-evolution step is shared by every member of the Xoshiro256 family ($\ast\ast$, $++$, $+$); only the output scrambler differs.

State evolution:

$$
\begin{aligned}
t &= S_1 \ll 17 \\
S_2 &\leftarrow S_2 \oplus S_0,\quad S_3 \leftarrow S_3 \oplus S_1 \\
S_1 &\leftarrow S_1 \oplus S_2,\quad S_0 \leftarrow S_0 \oplus S_3 \\
S_2 &\leftarrow S_2 \oplus t \\
S_3 &\leftarrow S_3 \lll 45
\end{aligned}
$$

Output scrambler for the $\ast\ast$ variant (this section):

$$X_i = (S_1 \cdot 5) \lll 7 \cdot 9$$

```cs
class Xoshiro256SS : IRandomNumberGenerator {
  private ulong _stateW, _stateX, _stateY, _stateZ;

  public void Seed(ulong seed) {
    this._stateW = SplitMix64.Next(ref seed);
    this._stateX = SplitMix64.Next(ref seed);
    this._stateY = SplitMix64.Next(ref seed);
    this._stateZ = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    ulong result = BitOperations.RotateLeft(this._stateX * 5, 7) * 9;

    ulong x = this._stateX << 17;

    this._stateY ^= this._stateW;
    this._stateZ ^= this._stateX;
    this._stateX ^= this._stateY;
    this._stateW ^= this._stateZ;

    this._stateY ^= x;
    this._stateZ = BitOperations.RotateLeft(this._stateZ, 45);

    return result;
  }

}
```

### XoRoShiRo (XRSR) [^19]

[^19]: [XRSR](https://vigna.di.unimi.it/ftp/papers/ScrambledLinear.pdf)

The name stands for XOR/rotate/shift/rotate, which describes the core operations used in these generators. These generators are designed to provide high performance utilizing less memory while maintaining excellent statistical properties.

The state is two 64-bit words $S = (S_0, S_1)$, half the size of the Xoshiro256 family. State evolution is shared between the $++$ and $+$ variants:

$$
\begin{aligned}
S_1 &\leftarrow S_1 \oplus S_0 \\
S_0 &\leftarrow (S_0 \lll 24) \oplus S_1 \oplus (S_1 \ll 16) \\
S_1 &\leftarrow S_1 \lll 37
\end{aligned}
$$

Output scrambler for the $++$ variant (this section):

$$X_i = (S_0 + S_1) \lll 17 \;+\; S_0$$

```cs
class Xoroshiro128PlusPlus : IRandomNumberGenerator {
  private ulong _stateX;
  private ulong _stateY;

  public void Seed(ulong seed) {
    this._stateX = SplitMix64.Next(ref seed);
    this._stateY = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    ulong x = this._stateX;
    ulong y = this._stateY;
    ulong result = BitOperations.RotateLeft(x + y, 17) + x;

    y ^= x;
    this._stateX = BitOperations.RotateLeft(x, 49) ^ y ^ (y << 21);
    this._stateY = BitOperations.RotateLeft(y, 28);

    return result;
  }

}
```

### Keep it simple stupid (KISS) [^20]

[^20]: [KISS](https://eprint.iacr.org/2011/007.pdf)

This generator is a combination of several simple and fast pseudorandom number generators. It was introduced by George Marsaglia to create a generator with a longer period and better statistical properties by combining the outputs of multiple generators. The idea behind KISS is to use the strengths of different RNGs to compensate for each other's weaknesses.

A typical KISS generator combines [LCG](#linear-congruential-generator-lcg), [XS](#xorshift-xs), and a [MWC](#multiply-with-carry-mwc). Each of these generators produces a sequence of pseudorandom numbers independently, and their outputs are combined using a simple bitwise operation to produce the final random number.

However one may choose to extend the principle by combining more than three and utilizing another combination operation:

* **Additive KISS**: Uses addition as the operation.
* **Subtractive KISS**: Uses subtraction.
* **Multiplicative KISS**: Uses multiplication.
* **XOR KISS**: Uses the XOR operation (default).

```cs
class KeepItSimpleStupid:IRandomNumberGenerator {

  private readonly LinearCongruentialGenerator _lcg = new();
  private readonly XorShift _xs = new();
  private readonly MultiplyWithCarry _mwc = new();

  public void Seed(ulong seed) {
    this._lcg.Seed(seed);
    this._xs.Seed(seed);
    this._mwc.Seed(seed);
  }

  public ulong Next() => this._lcg.Next() ^ this._xs.Next() ^ this._mwc.Next();

}
```

> [!TIP]
> The NuGet library may contain an implementation that supports more than two IRandomNumberGenerators and more combinatorial operators than just addition.

### Complementary Multiply with Carry (CMWC) [^21]

[^21]: [CMWC](https://blacklen.wordpress.com/2011/05/15/prng-3-complementary-multiply-with-carry/)

This generator is a refinement of the [MWC](#multiply-with-carry-mwc) method. In a CMWC generator, a sequence of random numbers is produced using a multiplier and a carry value, similar to the MWC method. However, the CMWC method maintains an array of states and updates them in a more sophisticated manner to improve the quality of the generated random numbers.

The CMWC generator is defined by the following parameters:

* $a$: Multiplier
* $m$: Modulus (typically a power of 2 for computational efficiency)
* $r$: The size of the state array
* $C$: Carry value (one scalar, updated after every step)
* $Q$: State array of size $r$ (the indexed Lagged-Fibonacci-style memory)

The generator produces the next random number using the following steps:

1. Select an index $j$ from the state array.
2. Calculate the new value of the state $Q_j$ using the formula:

   $$t = a \cdot Q_j + C$$

3. The new state value is given by the lower bits of $t$:

   $$Q_j = t \mod m$$

4. The carry value is updated using the upper bits of $t$:

   $$C = \left\lfloor \frac{t}{m} \right\rfloor$$

5. The new random number is the complement of the new state value:

   $$X_i = (m - 1) - Q_j$$

This process ensures that the state values are updated in a way that maintains a high-quality sequence of random numbers with a long period.

```cs
class ComplementaryMultiplyWithCarry : IRandomNumberGenerator {
  private static readonly UInt128 A = 6364136223846793005UL;
  private const int R = 4096;
  private readonly ulong[] _state = new ulong[R];
  private ulong _carry;
  private int _index = R - 1;

  public void Seed(ulong seed) {
    for (int i = 0; i < R; ++i)
      this._state[i] = SplitMix64.Next(ref seed);

    this._carry = SplitMix64.Next(ref seed);
  }

  public ulong Next() { // implicit mod 2^64
    this._index = (this._index + 1) % R;
    UInt128 t = A * this._state[this._index] + this._carry;

    this._carry = (ulong)(t >> 64);
    this._state[this._index] = (ulong)t;

    return ulong.MaxValue - this._state[this._index];
  }
}
```

### Lagged Fibonacci Generator (LFG) [^22]

[^22]: [LFG](https://asecuritysite.com/encryption/fab)

This is a type of pseudo-random number generator that extends the Fibonacci sequence concept to generate random numbers. Instead of simply adding the two previous numbers, as in the Fibonacci sequence, the LFG uses a combination of past values with different operations to produce the next value in the sequence. Donald Erwin Knuth described a subtractive LFG in "The Art of Computer Programming, Volume 2 - Seminumerical Algorithms".

The general form of the LFG is:

$$X_i = (X_{i-s} \, \circ \, X_{i-l}) \mod m$$

Where:

* $X_i$ is the current value in the sequence.
* $s$ and $l$ are lags, where $l > s \geq 0$.
* $\circ$ is a binary operation, such as addition, subtraction, multiplication, or bitwise XOR.
* $m$ is the modulus, which defines the range of the output values.
* The initial values, $X_0$ to $X_l$, are the seed values.

Depending on the operation used, LFGs can be categorized into different types:

* **Additive LFG**: Uses addition as the operation.
* **Subtractive LFG**: Uses subtraction.
* **Multiplicative LFG**: Uses multiplication.
* **XOR LFG**: Uses the XOR operation.

The LFG can be highly efficient and capable of generating sequences with very long periods, especially when properly chosen lags and modulus are used.

To further increase period length, one may store more than the last $l$ values in the internal state and utilize an incrementing index.

```cs
class LaggedFibonacciGenerator : IRandomNumberGenerator {

  private const int S = 0;
  private const int L = 21;
  private readonly ulong[] _state = new ulong[56];
  private int _index = 0;

  public void Seed(ulong seed) {
    for (int i = 0; i < this._state.Length; ++i)
      this._state[i] = SplitMix64.Next(ref seed);
  }

  public ulong Next() { // implicit mod 2^64
    ulong[] state = this._state;
    int length = state.Length;
    int index = this._index;

    ulong a = state[(index - S + length) % length];
    ulong b = state[(index - L + length) % length];
    ulong result = a + b;
    state[index] = result;

    this._index = ++index % length;
    return result;
  }

}
```

> [!TIP]
> The NuGet library may contain an implementation that supports parametrized **lags** and more combinatorial operators than just addition.

### Subtract with Borrow (SWB) [^23]

[^23]: [SWB](https://projecteuclid.org/journals/annals-of-applied-probability/volume-1/issue-3/A-New-Class-of-Random-Number-Generators/10.1214/aoap/1177005878.full)

This is a type of PRNG in the family of [LFG](#lagged-fibonacci-generator-lfg), introduced by George Marsaglia and Arif Zaman in 1991. These generators produce sequences of random numbers by using two preceding numbers at specified offsets or "lags" along with a carry value to influence the computation.

The SWC generator is defined by the following parameters:

* $r$: The size of the state array
* $s$: Short lag
* $l$: Long lag
* $C$: Borrow value (single scalar — capitalised here to distinguish from the LCG-style additive constant $c$, matching the [Notation key](#prng-algorithms))
* $m$: Modulus (typically a power of 2 for computational efficiency)
* $Q$: State array of size $r$

The generator produces the next random number using the following steps:

1. Select two indices, $i - s$ and $i - l$, from the state array.
2. Calculate the new value using the formula:

   $$X_i = (X_{i-s} - X_{i-l} - C) \bmod m$$

3. Update the borrow value $C$ based on the result:

   $$C = \begin{cases}
      1 & \text{if } X_{i-s} - X_{i-l} - C < 0 \\
      0 & \text{otherwise}
      \end{cases}$$

4. Update the state array at index $i$ with the new value $X_i$.

```cs
class SubtractWithBorrow : IRandomNumberGenerator {
  private const ulong M = ulong.MaxValue;
  private const int S = 63;
  private const int L = 4093;
  private const int R = 4096;
  private readonly ulong[] _state = new ulong[R];
  private ulong _carry;
  private int _index;

  public void Seed(ulong seed) {
    for (int i = 0; i < R; ++i)
      this._state[i] = SplitMix64.Next(ref seed);

    this._carry = SplitMix64.Next(ref seed);
    this._index = R - 1;
  }

  public ulong Next() {
    this._index = (this._index + 1) % R;
    int j = (this._index + R - S) % R;
    int k = (this._index + R - L) % R;

    Int128 t = (Int128)this._state[j] - this._state[k] - this._carry;
    this._carry = t < 0 ? 1UL : 0UL;
    if (t < 0)
      t += M;

    this._state[this._index] = (ulong)t;

    return this._state[this._index];
  }
}
```

### Linear Feedback Shift Register (LFSR) [^24]

[^24]: [LFSR](https://www.analog.com/en/resources/design-notes/random-number-generation-using-lfsr.html)

This is basically a shift register whose input bit is a linear function of its previous state. The most commonly used linear function of single bits is XOR. LFSRs are widely used in error detection and correction codes (CRC, BCH), spread-spectrum communications, and as *components* of stream ciphers — but on their own they are emphatically **not** cryptographically secure: the Berlekamp-Massey algorithm recovers the entire state from just $2n$ output bits in $O(n^2)$ time. Real-world stream ciphers (e.g. A5/1, E0, Trivium) combine multiple LFSRs with nonlinear filters or combiners to defeat this attack.

An LFSR is defined by its feedback polynomial, which determines how the previous bits of the register affect the new bit shifted into the register. The polynomial is typically represented in the form:

$$P(x) = x^n + c_{n-1}x^{n-1} + \cdots + c_1x + c_0$$

where $c_i$ are coefficients that are either $0$ or $1$. This polynomial is used to compute the feedback bit for the register. The state of the LFSR at step $i$ can be represented as a binary vector $X_i$, and the next state $X_{i+1}$ is computed by shifting all bits of $X_i$ to the right and inserting the new feedback bit at the leftmost position.

The feedback bit $f_i$ is calculated as follows:

$$f_i = X^0_i \oplus X^{p_1}_i \oplus \cdots \oplus X^{p_n}_i$$

where $p_{1..n}$ are the positions of the bits that correspond to the non-zero coefficients of the feedback polynomial.

Therefore:

$$X_i = \langle X^1_{i-1} \cdots X^n_{i-1}, f_i \rangle$$

Consider the tap mask $P(x) = x^{20} + x^{19} + x^{17} + x^{16} + x^{13} + x^{10} + x^7 + x^3 + x$. The taps are the bit positions whose coefficients are 1; XORing the state bits at those positions yields the feedback bit. As a binary literal:

$$\text{POLYNOM} = 0b110110010010001001010$$

(Here the rightmost bit is position 0 — so the set bits sit at positions 1, 3, 7, 10, 13, 16, 17, 19, 20 — matching the 9-term polynomial above.)

```cs
class LinearFeedbackShiftRegister : IRandomNumberGenerator {
  private const ulong POLYNOM = 0b110110010010001001010;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() {
    ulong result = 0UL;
    for (int i = 0; i < 64; ++i)
      result |= (ulong)StepLFSR() << i;

    return result;

    byte StepLFSR() {
      this._state = (ulong)CalculateFeedback() << 63 | (this._state >> 1);
      return (byte)(this._state & 1);

      byte CalculateFeedback() {
        ulong masked = this._state & POLYNOM;
        masked ^= masked >> 32;
        masked ^= masked >> 16;
        masked ^= masked >> 8;
        masked ^= masked >> 4;
        masked ^= masked >> 2;
        masked ^= masked >> 1;
        return (byte)(masked & 1);
      }
    }
  }

}
```

> [!TIP]
> The NuGet library may contain an implementation that supports a parametrized **polynom**.

### Feedback with Carry Shift Register (FCSR) [^25]

[^25]:[FCSR](https://www.researchgate.net/publication/220738954_A_Survey_of_Feedback_with_Carry_Shift_Registers)

This is a type of pseudorandom number generator that extends the concept of [LFSR](#linear-feedback-shift-register-lfsr)s by incorporating a carry value. The carry introduces non-linearity over GF(2), which makes FCSRs better-suited as *components* of stream ciphers (e.g. F-FCSR) than pure LFSRs — though, like LFSRs, an FCSR alone is not cryptographically secure and is categorized here under Deterministic rather than Cryptographic.

The FCSR generator operates by shifting bits through a register and using feedback to update the state of the register. The key difference between FCSR and LFSR is the addition of a carry value, which adds non-linearity to the generator and improves the randomness of the output sequence.

The FCSR generator is defined by the following parameters:

* $n$: The size of the shift register
* $P$: A feedback polynomial with coefficients in $\{0, 1\}$
* $r$: A carry value
* $s$: The state of the shift register, represented as a binary vector of length $n$

The generator produces the next random number using the following steps:

1. Calculate the feedback value using the feedback polynomial:

   $$f = \sum_{i=0}^{n-1} p_i \cdot s_i \mod 2$$

   where $p_i$ are the coefficients of the feedback polynomial $P$ and $s_i$ are the bits of the state vector $s$.

2. Add the carry value $r$ to the feedback value $f$:

   $$c = f + r$$

3. Update the carry value $r$:

   $$r = \left\lfloor \frac{c}{2} \right\rfloor$$

4. Update the state of the shift register by shifting all bits to the right and inserting the new bit (the least significant bit of $c$) at the leftmost position:

   $$s = (c \mod 2) \, || \, s_{0} \, || \, s_{1} \, || \, \ldots \, || \, s_{n-2}$$

5. The new random number is the bit that was shifted out of the register (the rightmost bit of the original state).

```cs
class FeedbackWithCarryShiftRegister : IRandomNumberGenerator {
  private ulong _state;
  private byte _carryBit;
  
  private const ulong POLY = 0b1000_1101__0101_1101__1100_1011__1101_1011__0110_0111__1100_1010__1101_1011__0110_0111;
  
  public void Seed(ulong seed) {
    this._carryBit = (byte)(seed & 1);
    this._state = seed;
  }

  public ulong Next() {
    ulong qword = 0UL;
    for (int i = 0; i < 64; ++i)
      qword |= (ulong)GetNextBit() << i;

    return qword;

    byte GetNextBit() {
      byte feedbackBit = ComputeFeedbackBit();
      byte feedbackCarrySum = feedbackBit + this._carryBit;
      this._carryBit = (byte)(feedbackCarrySum >> 1);

      // get one bit out of state
      byte result = (byte)(this._state & 1);
      this._state >>= 1;

      // and rotate the feedbackCarrySum in
      this._state |= (ulong)(feedbackCarrySum & 1) << 63;

      return result;
    }

    byte ComputeFeedbackBit() {
      ulong result = this._state & FeedbackWithCarryShiftRegister.POLY;
      result ^= result >> 32;
      result ^= result >> 16;
      result ^= result >> 8;
      result ^= result >> 4;
      result ^= result >> 2;
      result ^= result >> 1;
      return (byte)(result & 1);
    }

  }

}
```

> [!TIP]
> The NuGet library may contain an implementation that supports a parametrized **polynom**.

### Additive Congruential Random Number Generator (ACORN) [^26]

[^26]: [ACORN](https://acorn.wikramaratna.org/concept.html)

This generator that uses modular arithmetic and addition to produce a sequence of random numbers. The generator can achieve good statistical properties and is relatively simple to implement.

The ACORN generator is defined by the following parameters:

* $m$: Modulus (a large integer, typically a power of 2)
* $k$: Order of the generator
* $Q^j_0$: Initial seed for $j = 1, \ldots, k$, where $0 \leq Q^j_0 < m$
* $Q^j_i$: State for $j = 1, \ldots, k$, where $0 \leq Q^j_i < m$

The sequence is generated using the following steps:

For $i \geq 1$ and $j = 1, \ldots, k$:

$$Q^j_i = \left(Q^j_{i-1} + Q^{j-1}_{i-1} \right) \mod m$$

$$X_i = Q^k_i$$

```cs
class AdditiveCongruentialRandomNumberGenerator : IRandomNumberGenerator {
  private const int K = 12;
  private readonly ulong[] _state = new ulong[K + 1];

  public void Seed(ulong seed) {
    for (int m = 0; m <= K; ++m)
      this._state[m] = SplitMix64.Next(ref seed);
  }

  public ulong Next() { // implicit mod 2^64
    for (int m = 1; m <= K; ++m)
      this._state[m] += this._state[m - 1];

    return this._state[K];
  }

}
```

> [!TIP]
> The NuGet library may contain an implementation that supports parametrized **order** and **modulo**.

### Permuted Congruential Generator (PCG) [^27]

[^27]: [PCG](https://www.pcg-random.org/pdf/hmc-cs-2014-0905.pdf)

This is a pseudorandom number generation algorithm developed in 2014 by Dr. Melissa E. O'Neill. PCG applies an output permutation function to improve the statistical properties of a modulo- $2^n$ [LCG](#linear-congruential-generator-lcg).

The key features of PCG are:

* **Large Modulus and State**: The LCG modulus and state are usually twice the size of the desired output.
* **Power-of-2 Modulus**: Uses a power-of-2 modulus for efficient implementation.
* **Output Permutation**: The state is not output directly; instead, it is permuted to improve randomness.

The permutation step typically involves a combination of bitwise rotations, shifts, and xors, ensuring that the generated numbers have good statistical properties.

The shared core is a plain 128-bit LCG that advances the hidden state $S$ each step. Every PCG variant shares this state recurrence and differs only in the output permutation $P$:

$$
\begin{aligned}
S_i &\leftarrow a \cdot S_{i-1} + c \quad (\bmod \; 2^{128}) \\
X_i &= P(S_i)
\end{aligned}
$$

with $a = \mathtt{110282366920938463463374607431768211483}$ and $c = \mathtt{1442695040888963407}$ in this implementation.

The RXS-M-XS output permutation used in this base variant takes the top $5 + \lfloor S/2^{122} \rfloor$ bits of $S$ as a rotation count and applies an XOR-shift, multiply, XOR-shift sequence to return a 64-bit value.

The PCG family includes several different output transformations, each with specific characteristics:

* **XSH-RR**: An xorshift followed by a random rotation.
* **XSH-RS**: An xorshift followed by a random shift.
* **XSL-RR**: A simplified xorshift followed by a random rotation, optimized for 128-bit states.
* **RXS-M-XS**: An xorshift by a random amount, followed by a multiplication and another xorshift. This is the slowest and strongest output transformation when producing half-size output.
* **XSL-RR-RR**: A combination of xorshift and double random rotations, producing 128-bit output from 128-bit state.

```cs
class PermutedCongruentialGenerator : IRandomNumberGenerator {
  private UInt128 _state;
  
  private static readonly UInt128 MULTIPLIER = UInt128.Parse("110282366920938463463374607431768211471", NumberStyles.Integer, CultureInfo.InvariantCulture);
  private static readonly UInt128 INCREMENT = 1442695040888963407UL;

  public void Seed(ulong seed) => this._state = (UInt128)seed << 64 | ~seed;

  public ulong Next() {
    this._state = this._state * MULTIPLIER + INCREMENT;

    return Permute(this._state);

    // Apply RXS-M-XS permutation
    static ulong Permute(UInt128 state) {
      int count = (int)(state >> 122);
      state ^= state >> (5 + count);
      state *= 12605985483714917081UL;
      state ^= state >> 43;

      return (ulong)state;
    }
  }

}
```

### MIXMAX [^28]

[^28]: [MIXMAX](https://arxiv.org/pdf/1403.5355)

This generator is based on the properties of [Kolmogorov](https://link.springer.com/article/10.1134/S1063779620040644)-[Anosov C-systems](https://arxiv.org/pdf/1507.06348), which are a class of chaotic dynamical systems known for their excellent mixing properties. This generator utilizes an integer-valued unimodular matrix of size $NxN$ and arithmetic defined on a Galois field $GF[p]$ with a large prime modulus $p$. The primary idea is to leverage the dynamics of linear automorphisms on the unit hypercube in $\mathbb{R}^N$, which can be expressed as:

$$u_i(t + 1) = \sum_{j=1}^{N} A_{ij} u_j(t) \mod 1$$

Here, $u$ represents the vector in the unit interval $[0, 1)$, and $A$ is the defining matrix whose entries are integers, $A_{ij} \in \mathbb{Z}$. The conditions for the matrix $A$ to ensure the desired properties are:

* $\det A = 1$
* The eigenvalues $\lambda_k$ of $A$ must not lie on the unit circle, i.e., $|\lambda_k| \neq 1$ for all $k = 1, 2, ..., N$.

These conditions ensure that the map defined by $A$ is volume-preserving and exhibits exponential divergence of nearby trajectories, creating a highly chaotic and mixing behavior necessary for generating high-quality pseudo-random numbers.

The specific form of the MIXMAX matrix used in the generator is:

$$ A =
\begin{pmatrix}
1 & 1 & 1 & 1 & \cdots & 1 \\
1 & 2 & 1 & 1 & \cdots & 1 \\
1 & 3 + s & 2 & 1 & \cdots & 1 \\
1 & 4 & 3 & 2 & \cdots & 1 \\
\vdots & \vdots & \vdots & \vdots & \ddots & \vdots \\
1 & N & N-1 & N-2 & \cdots & 2 \\
\end{pmatrix}
$$

The matrix is recursively defined, and the only variable entry is $A_{32} = 3 + s$, where $s$ is a small integer chosen to avoid eigenvalues lying on the unit circle.

In practice, the MIXMAX RNG is implemented using modular arithmetic with a large prime modulus $p$, ensuring that the state vectors remain within a finite field $GF[p]$. The period of the generator is determined by the characteristic polynomial of the matrix $A$, and the maximal period is attained when this polynomial is primitive in the extended Galois field $GF[p^N]$.

```cs
class Mixmax : IRandomNumberGenerator {
  
  private const int _matrixSize = 256;
  private const long _magicNumber = -3;
  private ulong[] _state;
  private readonly ulong[,] _matrix;

  public Mixmax() {
    this._state = new ulong[_matrixSize];
    this._matrix = new ulong[_matrixSize, _matrixSize];
    this.InitializeMatrix();
  }

  private void InitializeMatrix() {
    for (int row = 0; row < _matrixSize; ++row) {
      this._matrix[row, 0] = 1;
      for (int column = 1; column < _matrixSize; ++column)
        if (column > row)
          this._matrix[row, column] = 1;
        else
          this._matrix[row, column] = (ulong)(row - column + 2);
    }

    this._matrix[2, 1]+= unchecked((ulong)_magicNumber);
  }

  public void Seed(ulong seed) {
    for (int i = 0; i < _matrixSize; ++i)
      this._state[i] = SplitMix64.Next(ref seed);
  }

  public ulong Next() { // implicit mod 2^64

    ulong result = 0;
    for (int i = 0; i < _matrixSize; ++i)
      result += this._state[i];

    ulong[] newState = new ulong[_matrixSize];
    for (int i = 0; i < _matrixSize; ++i) {
      newState[i] = 0;
      for (int j = 0; j < _matrixSize; ++j)
        newState[i] += this._matrix[i, j] * this._state[j];
    }

    this._state = newState;
    return result;
  }
}
```

### Mersenne Twister (MT) [^29]

[^29]: [MT](https://www.sciencedirect.com/topics/computer-science/mersenne-twister)

This was developed by Makoto Matsumoto and Takuji Nishimura in 1997. It is known for its long period, high order of equidistribution, and efficient implementation. The most widely used version of the Mersenne Twister is MT19937, which has a period of $2^{19937}-1$.

The MT generates sequences of random numbers using a large state array and uses a tempering transformation to produce the final output.

The state is an array $S$ of $n = 624$ 32-bit words. The **twist** step computes the next word from three older ones, using upper-/lower-bit masks $U$ and $L$ and the matrix-$A$ constant $a$:

$$
S_{i+n} = S_{i+m} \oplus \bigl((S_i \wedge U) \;\vert\; (S_{i+1} \wedge L)\bigr) \gg 1
\;\oplus\; \begin{cases} a & \text{if } S_{i+1} \text{ odd} \\ 0 & \text{otherwise} \end{cases}
$$

with $m = 397$, $U = \mathtt{0x80000000}$, $L = \mathtt{0x7FFFFFFF}$, $a = \mathtt{0x9908B0DF}$. Each raw state word is then passed through a **tempering** transform that produces the output $X_i$:

$$
\begin{aligned}
y &\leftarrow S_i \\
y &\leftarrow y \oplus (y \gg 11) \\
y &\leftarrow y \oplus ((y \ll 7) \wedge \mathtt{0x9D2C5680}) \\
y &\leftarrow y \oplus ((y \ll 15) \wedge \mathtt{0xEFC60000}) \\
y &\leftarrow y \oplus (y \gg 18) \\
X_i &\leftarrow y
\end{aligned}
$$

The shift counts $\{11, 7, 15, 18\}$ and the bit-masks come from an exhaustive search by Matsumoto and Nishimura — they are what make MT19937 1-dimensionally equidistributed across 32-bit words.

```cs
class MersenneTwister : IRandomNumberGenerator {
  private const int N = 624;
  private const int PERIOD = 397;
  private const uint MATRIX_A = 0x9908B0DF;
  private const uint UPPER_MASK = 0x80000000;
  private const uint LOWER_MASK = 0x7FFFFFFF;
  private const uint _TEMPERING_MASK_B = 0x9D2C5680;
  private const uint _TEMPERING_MASK_C = 0xEFC60000;

  private readonly uint[] _state = new uint[N];
  private int _index = N + 1;
  private static readonly uint[] _MAG01 = [0, MATRIX_A];

  public void Seed(ulong seed) {
    seed ^= seed >> 32;
    this._state[0] = (uint)seed;
    for (this._index = 1; this._index < N; ++this._index)
      this._state[this._index] = 1812433253 * (this._state[this._index - 1] ^ (this._state[this._index - 1] >> 30)) + (uint)this._index;
  }

  public ulong Next() {
    return (ulong)Next32() << 32 | Next32();
    
    uint Next32() {
      if (this._index >= N) {
        int i;

        for (i = 0; i < N - PERIOD; ++i) {
          uint y = (this._state[i] & UPPER_MASK) | (this._state[i + 1] & LOWER_MASK);
          this._state[i] = this._state[i + PERIOD] ^ (y >> 1) ^ _MAG01[y & 1];
        }

        for (; i < N - 1; ++i) {
          uint y = (this._state[i] & UPPER_MASK) | (this._state[i + 1] & LOWER_MASK);
          this._state[i] = this._state[i + (PERIOD - N)] ^ (y >> 1) ^ _MAG01[y & 1];
        }

        {
          uint y = (this._state[N - 1] & UPPER_MASK) | (this._state[0] & LOWER_MASK);
          this._state[N - 1] = this._state[PERIOD - 1] ^ (y >> 1) ^ _MAG01[y & 1];
        }

        this._index = 0;
      }

      uint x = this._state[this._index++];

      x ^= x >> 11;
      x ^= (x << 7) & _TEMPERING_MASK_B;
      x ^= (x << 15) & _TEMPERING_MASK_C;
      x ^= x >> 18;

      return x;
    }
  }
  
}
```

### Well Equidistributed Long-Period Linear (WELL) [^30]

[^30]: [WELL](https://www.iro.umontreal.ca/~lecuyer/myftp/papers/lfsr04.pdf)

The family of this generators was developed to improve upon [MT](#mersenne-twister-mt) by offering better equidistribution properties and faster recovery from bad states. Introduced by François Panneton and Pierre L'Ecuyer, WELL generators aim to provide high-quality random numbers for computational statistics and simulation.

Characteristics

* **Equidistribution**: WELL generators offer better equidistribution in high dimensions compared to the Mersenne Twister, ensuring a more uniform distribution of generated numbers.
* **Bit-Mixing**: WELL generators perform more bit transformations, leading to better statistical properties and reducing the impact of dependencies among successive output values.
* **Performance**: They maintain competitive performance in terms of speed while improving the quality of the random numbers.
* **Period Length**: WELL generators have extremely long periods, similar to MT, making them suitable for applications requiring extensive random data.
Algorithm

The WELL generators are based on linear recurrences modulo 2, specifically designed to improve the equidistribution and bit-mixing properties of the generated sequences. The state transition function uses a series of matrix transformations to update the state and produce the next random number.

The state is an array $S$ of $r = 32$ 32-bit words and an index pointer $i$. Each step computes four intermediate words $z_0 \ldots z_3$ from rotated views of the state, then writes the new state and emits the output. Letting $T_k^+$ and $T_k^-$ denote the GF(2) transformations $T_k^+(v) = v \oplus (v \gg k)$ and $T_k^-(v) = v \oplus (v \ll \lvert k\rvert)$ (so for $k < 0$ the shift goes left), one WELL step is:

$$
\begin{aligned}
z_0 &= S_{i + r - 1} \\
z_1 &= S_i \;\oplus\; T_{T_1}^+(S_{i + m_1}) \\
z_2 &= T_{T_2}^-(S_{i + m_2}) \;\oplus\; T_{T_3}^-(S_{i + m_3}) \\
z_3 &= z_1 \oplus z_2 \\
S_i &\leftarrow z_3 \\
S_{i + r - 1} &\leftarrow T_{T_4}^-(z_0) \oplus T_{T_5}^-(z_1) \oplus T_{T_6}^-(z_2) \\
i &\leftarrow (i + r - 1) \bmod r \\
X &= S_i
\end{aligned}
$$

with parameters $(m_1, m_2, m_3) = (3, 24, 10)$ and shift counts $(T_1, T_2, T_3, T_4, T_5, T_6) = (8, -19, -14, -11, -7, -13)$. These specific values were chosen to maximise the equidistribution of the resulting linear feedback over GF(2).

```cs
class WellEquidistributedLongperiodLinear : IRandomNumberGenerator {
  private const int R = 32;
  private const int M1 = 3;
  private const int M2 = 24;
  private const int M3 = 10;

  private uint index;
  private readonly uint[] _state = new uint[R];
  
  private static uint MAT0POS(int t, uint v) => v ^ (v >> t);
  private static uint MAT0NEG(int t, uint v) => v ^ (v << -t);
  private uint V0 => this._state[this.index];
  private uint VM1 => this._state[(this.index + M1) % R];
  private uint VM2 => this._state[(this.index + M2) % R];
  private uint VM3 => this._state[(this.index + M3) % R];
  private uint VRm1 => this._state[(this.index + R - 1) % R]; 
  private uint newV0 => this._state[(this.index + R - 1) % R] = value;
  private uint newV1 => this._state[this.index] = value;
  
  public void Seed(ulong seed) {
    this.index = 0;
    for (int i = 0; i < this._state.Length; ++i)
      this._state[i] = (uint)SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    return (ulong)Next32() << 32 | Next32();

    uint Next32() {
      const int T1 = 8;
      const int T2 = -19;
      const int T3 = -14;
      const int T4 = -11;
      const int T5 = -7;
      const int T6 = -13;
      
      uint z0 = this.VRm1;
      uint z1 = this.V0 ^ MAT0POS(T1, this.VM1);
      uint z2 = MAT0NEG(T2, this.VM2) ^ MAT0NEG(T3, this.VM3);
      uint z3 = z1 ^ z2;

      this.newV1 = z3;
      this.newV0 = MAT0NEG(T4, z0) ^ MAT0NEG(T5, z1) ^ MAT0NEG(T6, z2);
      this.index = (this.index + R - 1) % R;
      return this._state[this.index];
    }
  }
}
```

### Small Fast Counting 64 (SFC64) [^53]

[^53]: [SFC64](https://pracrand.sourceforge.net/RNG_engines.txt)

This was designed by Chris Doty-Humphrey for the PractRand test suite, and adopted by NumPy as an alternative to [Philox](#philox-phx-44). It is a counter-augmented chaotic generator: three state words evolve under shifts and rotations while a separate counter guarantees no short cycles regardless of the chaotic part.

Characteristics

* **Counter Insurance**: An explicit counter mixed into the output ensures the period is at least 2^64, even if the chaotic part falls into a short cycle.
* **256-bit State**: Three 64-bit state words plus a 64-bit counter.
* **Very Fast**: Only shifts, adds, XORs, and one rotation per output.
* **Not cryptographically secure**: state recovery is feasible given enough output, so do not use sfc64 for security-sensitive applications.

State words $(S_a, S_b, S_c)$, monotonic counter $i$:

$$
\begin{aligned}
X_i &= S_a + S_b + i \\
S_a &\leftarrow S_b \oplus (S_b \gg 11) \\
S_b &\leftarrow S_c + (S_c \ll 3) \\
S_c &\leftarrow (S_c \lll 24) + X_i
\end{aligned}
$$

with shift parameters $(11, 3, 24)$ tuned by Doty-Humphrey for the PractRand test suite.

```cs
class Sfc64 : IRandomNumberGenerator {
  private ulong _a, _b, _c, _counter;

  public void Seed(ulong seed) {
    this._a = this._b = this._c = seed;
    this._counter = 1;
    for (var i = 0; i < 12; ++i) this.Next();
  }

  public ulong Next() {
    var output = this._a + this._b + this._counter++;
    this._a = this._b ^ (this._b >> 11);
    this._b = this._c + (this._c << 3);
    this._c = BitOperations.RotateLeft(this._c, 24) + output;
    return output;
  }
}
```

### MRG32k3a (MRG) [^54]

[^54]: [MRG32k3a](https://www.iro.umontreal.ca/~lecuyer/myftp/papers/streams00.pdf)

This was designed by Pierre L'Ecuyer (1999). A combined Multiple Recursive Generator using two third-order linear recurrences modulo two distinct primes near 2^32. The two streams are subtracted to produce the output. Used as the default in MATLAB, R, SAS, and L'Ecuyer's widely-cited streams library.

Characteristics

* **Two Independent Streams**: Combining two parallel recurrences provides excellent equidistribution and an effective period near 2^191.
* **Modulus Near 2^32**: Each stream produces ~32-bit output; two consecutive outputs are concatenated for 64-bit results.
* **Provably Good**: The chosen multipliers are the result of an exhaustive parameter search optimizing spectral test scores.

The multipliers `(0, 1403580, -810728)` and `(527612, 0, -1370589)` are the result of an exhaustive parameter search by L'Ecuyer that maximises the spectral-test score across dimensions 2..6. The two prime moduli ($m_1 = 2^{32}-209$, $m_2 = 2^{32}-22853$) are coprime, so the periods of the two recurrences combine multiplicatively to roughly $m_1^3 \cdot m_2^3 / 2 \approx 2^{191}$.

Maintain two state vectors `(x10, x11, x12)` and `(x20, x21, x22)`. Each step computes one new 32-bit output; two consecutive outputs are concatenated for a 64-bit result.

```cs
class Mrg32k3a : IRandomNumberGenerator {
  private const long M1 = 4294967087;     // 2^32 - 209
  private const long M2 = 4294944443;     // 2^32 - 22853
  private long _x10, _x11, _x12, _x20, _x21, _x22;

  public void Seed(ulong seed) {
    this._x10 = (long)(SplitMix64.Next(ref seed) % (ulong)M1);
    this._x11 = (long)(SplitMix64.Next(ref seed) % (ulong)M1);
    this._x12 = (long)(SplitMix64.Next(ref seed) % (ulong)M1);
    this._x20 = (long)(SplitMix64.Next(ref seed) % (ulong)M2);
    this._x21 = (long)(SplitMix64.Next(ref seed) % (ulong)M2);
    this._x22 = (long)(SplitMix64.Next(ref seed) % (ulong)M2);
    // Avoid degenerate all-zero state for either recurrence.
    if (this._x10 == 0 && this._x11 == 0 && this._x12 == 0) this._x10 = 1;
    if (this._x20 == 0 && this._x21 == 0 && this._x22 == 0) this._x20 = 1;
  }

  public ulong Next() => ((ulong)NextInt32() << 32) | NextInt32();

  private ulong NextInt32() {
    var p1 = (1403580 * this._x11 - 810728 * this._x10) % M1;
    if (p1 < 0) p1 += M1;
    this._x10 = this._x11; this._x11 = this._x12; this._x12 = p1;

    var p2 = (527612 * this._x22 - 1370589 * this._x20) % M2;
    if (p2 < 0) p2 += M2;
    this._x20 = this._x21; this._x21 = this._x22; this._x22 = p2;

    var z = p1 - p2;
    if (z <= 0) z += M1;
    return (ulong)z;
  }
}
```

### Trivium (TRV) [^55]

[^55]: [TRV](https://www.ecrypt.eu.org/stream/p3ciphers/trivium/trivium_p3.pdf)

This is a hardware-oriented lightweight stream cipher (de Cannière & Preneel, 2005), part of the eSTREAM portfolio. Used as a CSPRNG by emitting one keystream bit per inner step and packing 64 bits per `Next()`.

Characteristics

* **288-bit State**: Three nonlinear feedback shift registers of lengths 93, 84, and 111 bits.
* **Bit-Oriented**: Designed for compact hardware; outputs one bit per step.
* **Cryptographic Strength**: An eSTREAM finalist with no known practical attacks below brute-force.

Algorithm

Initialize by loading an 80-bit key into the first 80 state bits (1-indexed positions 1..80, i.e. 0-indexed 0..79), an 80-bit IV into 1-indexed positions 94..173, setting state bits 286, 287, 288 to 1, then running $4 \cdot 288 = 1152$ warm-up cycles without output. Each step computes three feedback bits with AND-XOR mixing across the three registers, XORs them to produce the output bit, and shifts the registers.

```cs
class Trivium : IRandomNumberGenerator {
  private const int STATE_SIZE = 288;
  private readonly bool[] _state = new bool[STATE_SIZE];

  public void Seed(ulong seed) {
    Array.Clear(this._state);
    // Derive 80-bit key and 80-bit IV from the 64-bit seed via SplitMix64.
    var key = SplitMix64.Next(ref seed);
    var keyHi = SplitMix64.Next(ref seed);
    var iv = SplitMix64.Next(ref seed);
    var ivHi = SplitMix64.Next(ref seed);
    for (var i = 0; i < 64; ++i) this._state[i] = ((key >> i) & 1) != 0;
    for (var i = 0; i < 16; ++i) this._state[64 + i] = ((keyHi >> i) & 1) != 0;
    for (var i = 0; i < 64; ++i) this._state[93 + i] = ((iv >> i) & 1) != 0;
    for (var i = 0; i < 16; ++i) this._state[93 + 64 + i] = ((ivHi >> i) & 1) != 0;
    this._state[285] = this._state[286] = this._state[287] = true;
    for (var i = 0; i < 4 * STATE_SIZE; ++i) StepBit();
  }

  public ulong Next() {
    var result = 0UL;
    for (var i = 0; i < 64; ++i)
      if (StepBit()) result |= 1UL << i;
    return result;
  }

  // Indices below are 0-indexed translations of the spec's 1-indexed positions.
  private bool StepBit() {
    var s = this._state;
    var t1 = s[65] ^ s[92];
    var t2 = s[161] ^ s[176];
    var t3 = s[242] ^ s[287];
    var output = t1 ^ t2 ^ t3;
    t1 ^= (s[90] & s[91]) ^ s[170];
    t2 ^= (s[174] & s[175]) ^ s[263];
    t3 ^= (s[285] & s[286]) ^ s[68];
    for (var i = 92; i > 0; --i) s[i] = s[i - 1]; s[0] = t3;
    for (var i = 176; i > 93; --i) s[i] = s[i - 1]; s[93] = t1;
    for (var i = 287; i > 177; --i) s[i] = s[i - 1]; s[177] = t2;
    return output;
  }
}
```

### Xoroshiro128+ (XRSR+) [^78]

[^78]: [XRSR+](https://prng.di.unimi.it/)

This is the smaller cousin of [Xoshiro256+](#xoshiro256-xsr-58) by Blackman and Vigna. The state-evolution function is identical to [Xoroshiro128++](#xoroshiro-xrsr-19) (see that section for the recurrence on $(S_0, S_1)$); only the output scrambler changes:

$$X_i = S_0 + S_1$$

— no rotation, no multiplication. Like Xoshiro256+, the three low-order bits are LFSR-linear, so this variant is intended for generating IEEE-754 doubles where those bits never reach the mantissa.

```cs
class Xoroshiro128Plus : IRandomNumberGenerator {
  private ulong _s0, _s1;

  public ulong Next() {
    var s0 = _s0;
    var s1 = _s1;
    var result = s0 + s1;          // only change vs Xoroshiro128++: + instead of rotl(s0+s1,17)+s0
    s1 ^= s0;
    _s0 = BitOperations.RotateLeft(s0, 24) ^ s1 ^ (s1 << 16);
    _s1 = BitOperations.RotateLeft(s1, 37);
    return result;
  }
}
```

### Rule 30 cellular automaton (R30) [^79]

[^79]: [R30](https://www.wolframscience.com/nks/notes-7-9--rule-30-and-other-1d-cellular-automata/)

Stephen Wolfram famously used **Rule 30**, an elementary 1-D cellular automaton, as the random-number generator in early versions of *Mathematica*. Letting $S_{i,t}$ denote the value of cell $i$ at step $t$, the local update is

$$
S_{i,\,t+1} = S_{i-1,\,t} \;\oplus\; \bigl(S_{i,\,t} \;\vee\; S_{i+1,\,t}\bigr)
$$

— a 3-bit lookup table whose 8 output bits, read as a binary number, equal 30 (hence the name). Despite the deterministic, fully local update, the centre column of the evolving pattern looks random and passes many classical statistical tests; in this implementation the output bit $X_i$ is exactly the centre cell of the current row.

```mermaid
flowchart LR
  L["Left cell"] --> X1((XOR))
  C["Center cell"] --> O1((OR))
  R["Right cell"] --> O1
  O1 --> X1
  X1 --> N["New center value"]
```

The implementation in this repository keeps a 256-bit cyclic cell array and runs 64 synchronous Rule-30 updates per `Next()` call, accumulating the centre cell's value into a 64-bit output. Note that Rule 30 by itself **fails modern statistical tests** (BigCrush exposes serial-correlation problems immediately), so it is included here as a historical curiosity and a paradigmatically different generator design rather than as a recommendation.

```cs
class Rule30 : IRandomNumberGenerator {
  public ulong Next() {
    var result = 0UL;
    for (var step = 0; step < 64; ++step) {
      for (var i = 0; i < WIDTH; ++i) {
        var left   = _cells[(i + WIDTH - 1) % WIDTH];
        var center = _cells[i];
        var right  = _cells[(i + 1) % WIDTH];
        _next[i] = left ^ (center | right);
      }
      if (_cells[WIDTH / 2]) result |= 1UL << step;
      // Promote _next to _cells for the next step (implementation also clears _next).
    }
    return result;
  }
}
```

### PCG XSH-RR (PCG-XSH) [^80]

[^80]: [PCG-XSH-RR](https://www.pcg-random.org/pdf/hmc-cs-2014-0905.pdf)

The third PCG variant in the project, completing the family alongside [RXS-M-XS](#permuted-congruential-generator-pcg-27) and [XSL-RR](#permuted-congruential-xsl-rr-pcg-xsl-59). All three share the same 128-bit LCG core (see the [PCG](#permuted-congruential-generator-pcg-27) section for the state recurrence on $S$); only the output transform differs.

For PCG XSH-RR, letting $r = \lfloor S / 2^{122} \rfloor$ be the top six bits used as a rotation count:

$$
X_i = \bigl((S \gg 64) \oplus (S \gg 35)\bigr)_{\text{lo 64}} \;\ggg\; r
$$

— a high-bit XOR-shift mixes the high half of the state into the low half, then the result is rotated right by $r$.

```cs
class PermutedCongruentialXshRr : IRandomNumberGenerator {
  public ulong Next() {
    var state = _state * MULTIPLIER + INCREMENT;
    _state = state;
    var xored = (ulong)((state >> 64) ^ (state >> 35));   // XSH: mix high bits down
    var rot = (int)(state >> 122);                         // top 6 bits choose rotation
    return BitOperations.RotateRight(xored, rot);
  }
}
```

### WyRand (WY) [^52]

[^52]: [WY](https://github.com/wangyi-fudan/wyhash)

This is from the wyhash library by Wang Yi. It is an extremely fast counter-based generator that uses 128-bit multiply mixing. Despite its simplicity (just an increment, XOR, and one widening multiplication), it is widely used in hash tables (Zig, Go, Rust standard libraries) and general-purpose applications, and passes the standard NIST/Crush batteries; some users report PractRand failures at extremely large sample volumes, so wyrand is best treated as a fast general-purpose generator rather than one stress-tested to PB-scale.

Characteristics

* **Extremely Fast**: One addition and one widening multiply per output — among the fastest PRNGs that pass BigCrush.
* **Counter-Based**: The internal counter has period exactly $2^{64}$. Like every 64-bit generator, the *output stream* is subject to birthday collisions at roughly $2^{32}$ samples.
* **WyMix**: The core mixing function multiplies two 64-bit values into a 128-bit product, then XORs the halves — simple but highly effective diffusion.
* **Minimal State**: Only 64 bits of state (the counter).

Algorithm

Each step increments the state $S$ by a Weyl constant $\gamma$, then mixes the new state with a secret constant $K$ using the WyMix function (a 64×64→128 multiplication folded back to 64 bits by XOR):

$$
\begin{aligned}
S_i &\leftarrow S_{i-1} + \gamma \quad (\bmod \; 2^{64}) \\
P &\leftarrow S_i \cdot (S_i \oplus K) \\
X_i &= (P \gg 64) \oplus P_{\text{lo 64}}
\end{aligned}
$$

with $\gamma = \mathtt{0xA0761D6478BD642F}$ and $K = \mathtt{0xE7037ED1A0B428DB}$ — the wyhash constants by Wang Yi.

```cs
class WyRand : IRandomNumberGenerator {
  private const ulong INCREMENT = 0xA0761D6478BD642F;
  private const ulong SECRET = 0xE7037ED1A0B428DB;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() {
    this._state += INCREMENT;
    return WyMix(this._state, this._state ^ SECRET);
  }

  static ulong WyMix(ulong a, ulong b) {
    UInt128 full = (UInt128)a * b;
    return (ulong)(full >> 64) ^ (ulong)full;
  }
}
```

#### Counter-based vs sequential generators

Most PRNGs in this document — LCG, XorShift, Mersenne Twister, Romu, JSF, Lehmer128 and so on — are **sequential**: each output depends on the previous internal state, so generating output number $n$ requires walking through all $n-1$ predecessors (or jumping ahead via a custom function). The next several algorithms — Philox, Threefry, Squares, sfc64 — are **counter-based**: their output is a fixed bijection $f(\text{key}, i)$ of a key and an index $i$, so any output can be produced directly without computing the ones before it.

```mermaid
flowchart LR
  subgraph "Sequential PRNG (e.g. XorShift)"
    direction LR
    S0["state_0"] -->|"Next()"| S1["state_1"] -->|"Next()"| S2["state_2"] -->|"Next()"| S3["…"]
    S1 --> O1["out_1"]
    S2 --> O2["out_2"]
    S3 --> O3["out_3"]
  end
  subgraph "Counter-based PRNG (e.g. Philox)"
    direction LR
    K[key]
    K --> F1["f(key, 1)"] --> P1["out_1"]
    K --> F2["f(key, 2)"] --> P2["out_2"]
    K --> F3["f(key, 3)"] --> P3["out_3"]
    K --> Fn["f(key, n) -- computed directly"] --> Pn["out_n"]
  end
```

The practical consequences:

* **GPUs and parallel workers** can each compute their own slice $\{f(\text{key}, i_{\text{start}}), \ldots, f(\text{key}, i_{\text{end}})\}$ without synchronisation, since there is no shared mutable state. This is why Philox is the default RNG in NumPy, TensorFlow and JAX.
* **Jump-ahead is free**: skipping $10^{12}$ outputs costs the same as generating one, by just setting the counter to $i + 10^{12}$. With sequential generators, jumping ahead requires algorithm-specific math (Mersenne Twister and Xoshiro/Xoroshiro both ship dedicated jump functions; many older generators have none).
* **Reproducibility across machines** is trivial: $f$ is a pure function, so two implementations of the same algorithm produce byte-identical streams from the same key and counter.

The trade-off is that the round function $f$ has to be strong enough on its own — there's no accumulated mixing across calls to hide weaknesses — so counter-based generators typically apply 5–20 rounds of multiplication, rotation and XOR to each counter value.

### Philox (PHX) [^44]

[^44]: [PHX](https://www.thesalmons.org/john/random123/papers/random123sc11.pdf)

This is a counter-based RNG from the Random123 library (Salmon et al., 2011). Unlike traditional stateful PRNGs, counter-based generators compute output as a function of a counter and a key, making them trivially parallelizable. Philox is the default RNG in NumPy and is widely used in TensorFlow, JAX, and PyTorch for GPU-accelerated random number generation.

Characteristics

* **Counter-Based**: Output is a pure function of (counter, key) — no sequential state dependency, enabling massive parallelism.
* **Bijective Rounds**: Uses a Feistel network with widening multiplications as the round function, providing excellent diffusion.
* **Statistical Quality**: Passes BigCrush with as few as 7 rounds; 10 rounds is the standard conservative choice.
* **Performance**: Extremely fast on GPUs due to the use of hardware multiply instructions and absence of data-dependent branches.

Algorithm

Philox2x64-10 operates on a pair of 64-bit values $(L, R)$ — initially $L = i$ (the counter) and $R = 0$ — using a Feistel-network round function with widening multiplications. Let $K_r$ denote the round-$r$ key, $a$ the round multiplier, and $\gamma$ the Weyl key-bump:

$$
\begin{aligned}
P_r &= L \cdot a \quad \text{(128-bit product)} \\
L &\leftarrow (P_r \gg 64) \oplus K_r \oplus R \\
R &\leftarrow (P_r)_{\text{lo 64}} \\
K_{r+1} &\leftarrow K_r + \gamma
\end{aligned}
$$

with $a = \mathtt{0xD2B74407B1CE6E93}$, $\gamma = \mathtt{0x9E3779B97F4A7C15}$ (the golden-ratio Weyl constant). After 10 rounds the pair $(L, R)$ is the output (this implementation returns $L$ first and $R$ on the next call).

```cs
class Philox : IRandomNumberGenerator {
  private const ulong MULTIPLIER = 0xD2B74407B1CE6E93;
  private const ulong ROUND_KEY_BUMP = 0x9E3779B97F4A7C15;

  private ulong _counter;
  private ulong _key;
  private ulong _buffered;
  private bool _hasBuffered;

  public void Seed(ulong seed) {
    this._key = seed;
    this._counter = 0;
    this._hasBuffered = false;
  }

  public ulong Next() {
    if (this._hasBuffered) {
      this._hasBuffered = false;
      return this._buffered;
    }

    var lo = this._counter++;
    var hi = 0UL;
    var roundKey = this._key;

    for (var i = 0; i < 10; ++i) {
      var product = Math.BigMul(lo, MULTIPLIER); // 128-bit multiply
      var newLo = (ulong)(product >> 64) ^ roundKey ^ hi;
      hi = (ulong)product;
      lo = newLo;
      roundKey += ROUND_KEY_BUMP;
    }

    // Each counter step produces a 128-bit (lo, hi) pair; buffer hi for the next call.
    this._buffered = hi;
    this._hasBuffered = true;
    return lo;
  }
}
```

### Threefry (TF) [^45]

[^45]: [TF](https://www.thesalmons.org/john/random123/papers/random123sc11.pdf)

This is another counter-based RNG from the Random123 library, based on the Threefish block cipher from the Skein hash function. It uses only simple operations (addition, rotation, XOR) with no multiplications, making it especially suitable for platforms where multiplications are expensive.

Characteristics

* **ARX Design**: Uses only Addition, Rotation, and XOR — no multiplications needed.
* **Threefish Heritage**: Derived from the Threefish block cipher with reduced rounds for RNG use.
* **Key Injection**: Subkeys are injected every 4 rounds using the Skein key schedule.
* **Statistical Quality**: Passes BigCrush with 13+ rounds; 20 rounds is the standard choice.

Algorithm

Threefry2x64-20 operates on two 64-bit values $(L, R)$ with 20 rounds of rotation and XOR mixing, plus a key injection every 4 rounds. Let $\theta_r$ be the rotation amount at round $r$ — drawn cyclically from $(16, 42, 12, 31, 16, 32, 24, 21)$. One Threefry round is:

$$
\begin{aligned}
L &\leftarrow L + R \\
R &\leftarrow (R \lll \theta_r) \oplus L
\end{aligned}
$$

Every 4 rounds, two subkeys $(k_r^L, k_r^R)$ derived from the key schedule are added back in:

$$L \leftarrow L + k_r^L, \quad R \leftarrow R + k_r^R + r/4$$

The key schedule extends the 2-word key $(K_0, K_1)$ to 3 words using the Skein parity constant $K_2 = K_0 \oplus K_1 \oplus \mathtt{0x1BD11BDAA9FC1A22}$. The rotation amounts come from the Threefish-256 round schedule, chosen via exhaustive search to maximise diffusion per round.

```cs
class Threefry : IRandomNumberGenerator {
  private const ulong SKEIN_PARITY = 0x1BD11BDAA9FC1A22;
  private static readonly int[] ROTATIONS = [16, 42, 12, 31, 16, 32, 24, 21];

  private ulong _key0, _key1;
  private ulong _counter;

  public void Seed(ulong seed) {
    this._key0 = seed;
    this._key1 = SplitMix64.Next(ref seed);
    this._counter = 0;
  }

  public ulong Next() {
    var x0 = this._counter++;
    var x1 = 0UL;
    var ks0 = this._key0;
    var ks1 = this._key1;
    var ks2 = SKEIN_PARITY ^ ks0 ^ ks1;

    x0 += ks0;
    x1 += ks1;

    for (var round = 0; round < 20; ++round) {
      x0 += x1;
      x1 = BitOperations.RotateLeft(x1, ROTATIONS[round % 8]) ^ x0;

      if ((round + 1) % 4 == 0) { // key injection
        var inject = (round + 1) / 4;
        x0 += (inject % 3) switch { 0 => ks0, 1 => ks1, _ => ks2 };
        x1 += ((inject % 3) switch { 0 => ks1, 1 => ks2, _ => ks0 }) + (ulong)inject;
      }
    }

    return x0; // x1 available as second output
  }
}
```

### Squares (SQ) [^46]

[^46]: [SQ](https://arxiv.org/abs/2004.06278)

This was developed by Bernard Widynski (2022), the same author as [MSWS](#middle-square-weyl-sequence-msws-6). It is a modern counter-based generator that revisits the middle-square concept with a Weyl sequence key, achieving excellent statistical quality with minimal state. The 64-bit variant uses 5 rounds of squaring and word-swapping.

Characteristics

* **Counter-Based**: Like Philox and Threefry, output is a pure function of (counter, key).
* **Middle-Square Heritage**: Each round squares the state and takes the "middle" via a 32-bit rotation, connecting it to von Neumann's original idea.
* **Compact**: Only two 64-bit state variables (counter + key).
* **Fast**: Uses only multiplication, addition, XOR, and rotation — no lookup tables or complex permutations.

Algorithm

Let $i$ be the counter (incremented per call) and $K$ be the Weyl key (fixed per-stream). Define $y = i \cdot K$, $z = y + K$, and let $\rho(v) = (v \gg 32) \;\vert\; (v \ll 32)$ denote a 32-bit halfword swap. Then one Squares step is:

$$
\begin{aligned}
x &= y \\
x &\leftarrow \rho(x^2 + y) \quad \text{(round 1)} \\
x &\leftarrow \rho(x^2 + z) \quad \text{(round 2)} \\
x &\leftarrow \rho(x^2 + y) \quad \text{(round 3)} \\
t &= x^2 + z \\
x &\leftarrow \rho(t)         \quad \text{(round 4)} \\
X_i &= t \oplus \bigl((x^2 + y) \gg 32\bigr) \quad \text{(round 5)}
\end{aligned}
$$

Following Widynski's "4 + 1" formulation, the 64-bit output is the XOR of $t$ (the round-4 value *before* its 32-bit rotation) with the upper 32 bits of the round-5 product $x^2 + y$.

```cs
class Squares : IRandomNumberGenerator {
  private ulong _counter, _key;

  public void Seed(ulong seed) {
    this._key = SplitMix64.Next(ref seed) | 1;
    this._counter = 0;
  }

  public ulong Next() {
    var y = this._counter * this._key;
    var x = y;
    var z = y + this._key;
    this._counter++;

    x = x * x + y; x = (x >> 32) | (x << 32); // round 1
    x = x * x + z; x = (x >> 32) | (x << 32); // round 2
    x = x * x + y; x = (x >> 32) | (x << 32); // round 3
    var t = x = x * x + z; x = (x >> 32) | (x << 32); // round 4
    return t ^ ((x * x + y) >> 32);              // round 5 (64-bit output)
  }
}
```

### Jenkins Small Fast (JSF) [^47]

[^47]: [JSF](https://burtleburtle.net/bob/rand/smallprng.html)

This was designed by Bob Jenkins, known for his hash functions and the [ISAAC](#isaac-64-isaac-50) CSPRNG. JSF64 is a compact, fast, non-cryptographic PRNG with 256 bits of state and no bad seeds. It combines subtraction, addition, XOR, and rotation in a simple four-variable feedback loop.

Characteristics

* **Compact State**: Only 256 bits (4 × 64-bit words), yet provides excellent statistical quality.
* **No Bad Seeds**: The warm-up loop during seeding ensures all initial states lead to good sequences.
* **Simple Design**: Each step uses only subtraction, addition, XOR, and rotation — easy to implement and verify.
* **Fast**: Competitive with XorShift variants while having much better statistical properties.

Algorithm

JSF64 maintains four 64-bit state variables $(S_a, S_b, S_c, S_d)$. Each step computes a new value from subtracting a rotated value, then cascades XOR and rotation through the remaining variables. The output is the new value of $S_d$:

$$
\begin{aligned}
e &= S_a - (S_b \lll 7) \\
S_a &\leftarrow S_b \oplus (S_c \lll 13) \\
S_b &\leftarrow S_c + (S_d \lll 37) \\
S_c &\leftarrow S_d + e \\
S_d &\leftarrow e + S_a \\
X_i &= S_d
\end{aligned}
$$

The rotation triple $(7, 13, 37)$ is the recommended JSF64 set from Jenkins's original page.

```cs
class JenkinsSmallFast : IRandomNumberGenerator {
  private ulong _a, _b, _c, _d;

  public void Seed(ulong seed) {
    this._a = 0xF1EA5EED;
    this._b = this._c = this._d = seed;
    for (var i = 0; i < 20; ++i) this.Next(); // warm-up
  }

  public ulong Next() {
    var e = this._a - BitOperations.RotateLeft(this._b, 7);
    this._a = this._b ^ BitOperations.RotateLeft(this._c, 13);
    this._b = this._c + BitOperations.RotateLeft(this._d, 37);
    this._c = this._d + e;
    this._d = e + this._a;
    return this._d;
  }
}
```

### RomuTrio (ROMU) [^48]

[^48]: [ROMU](https://www.romu-random.org/)

This was designed by Mark Overton (2020). Romu generators use rotation-based multiplicative mixing to achieve excellent speed and statistical quality. RomuTrio, the flagship variant, uses 192 bits of state and is among the fastest high-quality PRNGs available.

Characteristics

* **Rotation-Multiplicative**: Combines rotation and multiplication for strong non-linear mixing.
* **Very Fast**: One of the fastest generators that still passes all standard test suites.
* **192-bit State**: Three 64-bit words provide a large enough state space to avoid short cycles.
* **Simple Update**: Each step requires only one multiplication and two rotations.

Algorithm

RomuTrio maintains three 64-bit state variables $(S_x, S_y, S_z)$. With multiplier $a = \mathtt{15241094284759029579}$ and snapshotted previous values $x' = S_x$, $y' = S_y$, $z' = S_z$:

$$
\begin{aligned}
X_i &= x' \\
S_x &\leftarrow a \cdot z' \\
S_y &\leftarrow (y' - x') \lll 12 \\
S_z &\leftarrow (z' - y') \lll 44
\end{aligned}
$$

The all-three-snapshotted form is essential — it ensures each new state value depends on the *old* values of the others (as in Overton's specification).

```cs
class RomuTrio : IRandomNumberGenerator {
  private const ulong MULTIPLIER = 15241094284759029579;
  private ulong _x, _y, _z;

  public void Seed(ulong seed) {
    this._x = SplitMix64.Next(ref seed);
    this._y = SplitMix64.Next(ref seed);
    this._z = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    // Snapshot all three state words BEFORE writing back, so that
    // each new value depends on the OLD other values (per Overton's spec).
    var xp = this._x;
    var yp = this._y;
    var zp = this._z;

    this._x = MULTIPLIER * zp;
    this._y = BitOperations.RotateLeft(yp - xp, 12);
    this._z = BitOperations.RotateLeft(zp - yp, 44);
    return xp;
  }
}
```

### Lehmer 128-bit (L128) [^49]

[^49]: [L128](https://lemire.me/blog/2019/03/19/the-fastest-conventional-random-number-generator-that-can-pass-big-crush/)

This is a 128-bit multiplicative congruential generator (MCG), a modern evolution of the classical [MLCG](#multiplicative-linear-congruential-generator-mlcg-7). By using a 128-bit state multiplied by a 64-bit constant and returning the upper 64 bits, it achieves excellent statistical quality while being extremely fast. Recommended by Steele and Vigna as a simple, high-quality baseline generator.

Characteristics

* **Compact State**: 128 bits — comparable to other modern BigCrush-passing generators (JSF, sfc64, RomuTrio all use roughly 128–256 bits).
* **Extremely Fast**: A single 128-bit multiplication per output. On x86-64 with BMI2 (`mulx`) this compiles to roughly two machine instructions; on platforms without a 64×64→128 multiply it requires emulation and is correspondingly slower.
* **Upper Bits Output**: Returning the upper 64 bits of the 128-bit state provides better statistical properties than lower bits.
* **No Addition**: Pure multiplicative (no additive constant), yet the 128-bit state avoids the low-bit weaknesses of classical 64-bit MCGs.

Algorithm

The state $S$ is a 128-bit unsigned integer; each step multiplies it by the 64-bit constant $a = \mathtt{0xDA942042E4DD58B5}$ and emits the high 64 bits as output:

$$
\begin{aligned}
S_i &\leftarrow a \cdot S_{i-1} \quad (\bmod \; 2^{128}) \\
X_i &= S_i \gg 64
\end{aligned}
$$

— a pure multiplicative congruential generator at 128-bit precision.

```cs
class Lehmer128 : IRandomNumberGenerator {
  private const ulong MULTIPLIER = 0xDA942042E4DD58B5;
  private UInt128 _state;

  public void Seed(ulong seed) {
    this._state = ((UInt128)SplitMix64.Next(ref seed) << 64)
                | SplitMix64.Next(ref seed) | 1;
  }

  public ulong Next() {
    this._state *= MULTIPLIER;
    return (ulong)(this._state >> 64);
  }
}
```

### LXM (LXM) [^50]

[^50]: [LXM](https://openjdk.org/jeps/356)

This is a family of generators introduced in Java 17 (JEP 356) that combine a Linear congruential generator, a Xor-based generator, and a Mixing function. The L64X128MixRandom variant combines a 64-bit LCG with a 128-bit Xoroshiro128 subgenerator (Vigna's plain "+" variant — see the [XoRoShiRo](#xoroshiro-xrsr-19) section; the ++/** scrambled output transforms used in Xoroshiro128++ are *not* part of LXM, which delegates output mixing to the Lea64 finalizer instead).

Characteristics

* **Hybrid Design**: Combines the long-period guarantee of an LCG with the bit-mixing quality of Xoroshiro.
* **Splittable**: The LCG addend can be varied per stream, enabling safe parallel use without coordination.
* **Lea64 Mixing**: The output mixer ensures that weaknesses in either subgenerator are masked.
* **Java Standard**: The default `RandomGenerator` family in Java 17+, chosen after extensive evaluation.

Algorithm

The hidden state has three parts: an LCG state $S^L$, a per-stream LCG addend $c$ (the "splittable" element), and a 128-bit Xoroshiro substate $(S_0, S_1)$. Let $M = \mathtt{0xD1342543DE82EF95}$ be the LCG multiplier and let $\mu(z)$ denote the Lea64 finaliser. One LXM step is:

$$
\begin{aligned}
\text{old} &\leftarrow (S^L, S_0) \\
S^L &\leftarrow S^L \cdot M + c \quad (\bmod \; 2^{64}) \\
(S_0, S_1) &\leftarrow \text{xoroshiro128}(S_0, S_1) \quad \text{(state evolution from the X-subgenerator)} \\
X_i &= \mu(\text{old}.S^L + \text{old}.S_0)
\end{aligned}
$$

The Lea64 finaliser (named after Doug Lea, who proposed it for JEP 356) is two rounds of multiply-XOR-shift:

$$
\mu(z) = \bigl( ((z \oplus (z \gg 32)) \cdot \mathtt{0xDABA0B6EB09322E3}) \oplus (\cdots \gg 32) \bigr) \cdot \mathtt{0xDABA0B6EB09322E3} \oplus (\cdots \gg 32)
$$

The same multiplier constant appears in both rounds — unusual but specified by JEP 356.

```cs
class Lxm : IRandomNumberGenerator {
  private const ulong LCG_MULTIPLIER = 0xD1342543DE82EF95;
  private ulong _lcgState, _lcgAddend;
  private ulong _x0, _x1;

  public void Seed(ulong seed) {
    this._lcgAddend = SplitMix64.Next(ref seed) | 1;
    this._lcgState = SplitMix64.Next(ref seed);
    this._x0 = SplitMix64.Next(ref seed);
    this._x1 = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    var s = this._lcgState;
    var q0 = this._x0;
    var q1 = this._x1;

    this._lcgState = s * LCG_MULTIPLIER + this._lcgAddend;

    q1 ^= q0;
    this._x0 = BitOperations.RotateLeft(q0, 24) ^ q1 ^ (q1 << 16);
    this._x1 = BitOperations.RotateLeft(q1, 37);

    return MixLea64(s + q0);

    static ulong MixLea64(ulong z) {
      z = (z ^ (z >> 32)) * 0xDABA0B6EB09322E3;
      z = (z ^ (z >> 32)) * 0xDABA0B6EB09322E3;
      return z ^ (z >> 32);
    }
  }
}
```

### Xoshiro256+ (XSR+) [^58]

[^58]: [XSR+](https://prng.di.unimi.it/)

This is a sibling of [Xoshiro256**](#xoshiro-xsr-18) by David Blackman and Sebastiano Vigna. The state-evolution function is identical (see the [Xoshiro256**](#xoshiro-xsr-18) section for the full state recurrence) — only the output scrambler changes:

$$X_i = S_0 + S_3$$

Because of that simplification the *lowest three bits* of the output are LFSR-linear (just XORs of state bits), which would fail strict integer-bit tests; the design specifically targets generating IEEE-754 doubles, where the bottom three bits never make it into the mantissa.

Xoshiro256+ is the default generator in `System.Random` from .NET 6 onwards.

```cs
class Xoshiro256Plus : IRandomNumberGenerator {
  private ulong _w, _x, _y, _z;

  public ulong Next() {
    var result = this._w + this._z;  // only change vs Xoshiro256**: + instead of (x*5).rotl(7)*9
    var x = this._x << 17;
    this._y ^= this._w; this._z ^= this._x; this._x ^= this._y; this._w ^= this._z;
    this._y ^= x;
    this._z = BitOperations.RotateLeft(this._z, 45);
    return result;
  }
}
```

### Permuted Congruential XSL-RR (PCG-XSL) [^59]

[^59]: [PCG-XSL-RR](https://www.pcg-random.org/pdf/hmc-cs-2014-0905.pdf)

This is the canonical 128-bit → 64-bit output variant of Melissa O'Neill's PCG family, complementing the [RXS-M-XS](#permuted-congruential-generator-pcg-27) variant already in the project. Both share the same 128-bit LCG core (see the [PCG](#permuted-congruential-generator-pcg-27) section for the state recurrence on $S$); only the output permutation differs.

For PCG XSL-RR, letting $r = \lfloor S / 2^{122} \rfloor$ (top six bits):

$$
X_i = \bigl((S \gg 64) \oplus S\bigr)_{\text{lo 64}} \;\ggg\; r
$$

— fold the high 64 bits of the state into the low 64 via XOR, then rotate the result by $r$.

```cs
class PermutedCongruentialXslRr : IRandomNumberGenerator {
  private UInt128 _state;

  public ulong Next() {
    var state = this._state * MULTIPLIER + INCREMENT;
    this._state = state;
    var rotation = (int)(state >> 122);                          // top 6 bits choose the rotation
    var xored = (ulong)(state >> 64) ^ (ulong)state;             // fold high into low
    return BitOperations.RotateRight(xored, rotation);
  }
}
```

The pattern — *use one part of the LCG state to choose how to mix the rest* — is the core PCG insight: it gives a non-linear output transform "for free" (no extra state, one extra rotate) while the underlying LCG core remains amenable to mathematical analysis.

## CSRNG Algorithms

### Blum-Micali (BM) [^31]

[^31]: [BM](https://pages.cs.wisc.edu/~cs812-1/blum.micali82.pdf)

This was developed by Manuel Blum and Silvio Micali in 1982. It is one of the earliest CSPRNGs and is based on the hardness of the discrete logarithm problem. The BM generator is designed to produce a sequence of bits that are computationally indistinguishable from a truly random sequence, assuming the underlying mathematical problem remains intractable.

**Key Concepts:**

1. **Underlying Hard Problem:**
   * The security of the BM generator is based on the discrete logarithm problem, which is considered to be computationally hard. Specifically, it is assumed that given a number $g^x$ (where $g$ is a generator of a cyclic group and $x$ is an integer), it is computationally infeasible to determine $x$ from $g^x$ without additional information.

2. **Mathematical Setup:**
   * The BM generator operates within a finite cyclic group $G$ of prime order $p$. A generator $g$ of this group is selected, and a secret seed $s_0$ is chosen such that $1 < s_0 < p-1$.
   * The generator then iteratively computes values based on the following recurrence relation:

     $$ s_{i+1} = g^{s_i} \mod p $$

   * For each $s_i$, a bit $b_i$ is generated based on whether $s_i$ is greater than or less than $\frac{p-1}{2}$.

3. **Bit Generation:**
   * In each iteration, the BM generator produces a single bit of output. The bit \( b_i \) is determined as follows:

   $$
     b_i =
     \begin{cases}
       1 & \text{if } s_i \leq \frac{p-1}{2} \\
       0 & \text{if } s_i > \frac{p-1}{2}
     \end{cases}
   $$

   * The resulting sequence of bits $X_n = \langle b_0, b_1, \ldots \rangle$ forms the output of the BM generator.

4. **Computational Security:**
   * The security of the BM generator relies on the difficulty of predicting any bit of the output sequence given knowledge of the previous bits. This security property is derived from the hardness of the discrete logarithm problem.

```cs
class BlumMicali(ulong p, ulong g) :IRandomNumberGenerator {

  private ulong _state;

  public BlumMicali() : this(6364136223846793005UL, 2147483647) { }

  public void Seed(ulong seed) {
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(seed, 1UL);
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(seed, p - 1);
    this._state = seed;
  }

  public ulong Next() {
    ulong result = 0UL;
    for (int i = 0; i < 64; ++i)
      result = (result << 1) | (NextBit() ? 1UL : 0);

    return result;

    bool NextBit() 
      => (this._state = (ulong)BigInteger.ModPow(g, this._state, p)) <= (p - 1) / 2
      ;

  }
}
```

> [!TIP]
> The NuGet library may contain an implementation that supports parametrized **p** and **q**.

### Self-shrinking Generator (SSG) [^32]

[^32]: [SSG](https://link.springer.com/book/10.1007/BFb0053418)

This is a type of CSRNG that operates based on the principles of [LFSR](#linear-feedback-shift-register-lfsr)s. Introduced by Meier and Staffelbach in 1994, the SSG is particularly known for its simplicity and the inherent cryptographic properties derived from its LFSR-based design. This generator shrinks the output of an LFSR by selecting bits in a specific manner, thus providing a more secure and less predictable output sequence. It works by using the output of an LFSR in pairs of bits. Depending on the values of these pairs, it either includes or excludes certain bits from the final output sequence. Here's how the SSG operates in detail:

* **LFSR Step**: The LFSR is stepped to produce a new bit.
* **Pairing Bits**: The generator looks at pairs of bits $(x, y)$ produced by consecutive steps of the LFSR.
* **Output Rule**: If the first bit of the pair is `1`, the second bit is used as part of the output. If the first bit is `0`, the second bit is discarded.

This process effectively "shrinks" the sequence of bits by removing some based on the predefined rule, hence the name "Self-Shrinking Generator".

```cs
class SelfShrinkingGenerator : IRandomNumberGenerator {
  private const ulong POLYNOM = 0b110110010010001001010;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() {
    ulong result = 0UL;
    int resultBits = 0;

    do {
      byte (x, y) = (StepLFSR(), StepLFSR());
      if (x == 0)
        continue;

      result |= ((ulong)y << resultBits);
      ++resultBits;
    } while (resultBits < 64);

    return result;
    
    byte StepLFSR() {
      this._state = (ulong)CalculateFeedback() << 63 | (this._state >> 1);
      return (byte)(this._state & 1);

      byte CalculateFeedback() {
        ulong masked = this._state & POLYNOM;
        masked ^= masked >> 32;
        masked ^= masked >> 16;
        masked ^= masked >> 8;
        masked ^= masked >> 4;
        masked ^= masked >> 2;
        masked ^= masked >> 1;
        return (byte)(masked & 1);
      }
    }
  }

}
```

> [!TIP]
> The NuGet library may contain an implementation that supports a parametrized **polynom**.

### Blum Blum Shub (BBS) [^33] [^34]

[^33]: [BBS](https://www.cs.miami.edu/home/burt/learning/Csc609.062/docs/bbs.pdf)
[^34]: [BBS-Paper](https://people.tamu.edu/~rojas/bbs.pdf)

This generator is a cryptographically secure pseudorandom number generator based on the difficulty of factoring large composite numbers. It was proposed by Lenore Blum, Manuel Blum, and Michael Shub in 1986. The generator is particularly known for its security properties, making it suitable for cryptographic applications.

The BBS is defined by the following parameters:

* Two large prime numbers, $p$ and $q$, where both $p$ and $q$ are congruent to 3 modulo 4.
* The modulus $m = p \cdot q$ .
* An initial seed $s$ such that $s$ is relatively prime to $m$ (i.e., $\gcd(s, m) = 1$).

The generator produces the next state using the recurrence relation:
$$X_i = X_{i-1}^2 \mod m$$

To generate pseudorandom bits, the least significant bit (LSB) of each $X_i$ is used. For more bits, several of the least significant bits can be extracted.

```cs
class BlumBlumShub : IRandomNumberGenerator {
  private UInt128 _state;
  private readonly UInt128 _modulus;

  public BlumBlumShub() : this(18446744073709551559, 30064771079) { }

  public BlumBlumShub(ulong p, ulong q) {
    if (p % 4 != 3 || q % 4 != 3)
      throw new ArgumentException("Both p and q must be congruent to 3 modulo 4.");

    this._modulus = (UInt128)p * q;
  }

  public void Seed(ulong seed) {
    this._state = seed % this._modulus;
    
    // Ensure seed is relatively prime to modulus
    while (BigInteger.GreatestCommonDivisor(this._state, this._modulus) != 1)
      this._state = (this._state + 1) % this._modulus;
  }

  public ulong Next() {
    ulong result = 0;
    for (int i = 0; i < 64; i += 8) { // extract 8 bits at a time
      this._state = this._state * this._state % this._modulus;
      result |= (ulong)(this._state & 0xff) << i;
    }
    return result;
  }
}
```

> [!TIP]
> The NuGet library may contain an implementation that supports parametrized **p**, **q** and **bitcount per iteration**.

### ChaCha20 (CC20) [^35]

[^35]: [CC20](https://www.chronox.de/chacha20_drng/)

This is a stream cipher designed by Daniel J. Bernstein, known for its simplicity, speed, and security. Although originally intended for encryption, ChaCha20 can also be effectively used as a CSPRNG. Its strong diffusion properties and resistance to cryptanalytic attacks make it an excellent choice for generating high-quality random numbers.

**Key Characteristics of ChaCha20 for RNG:**

* **Cryptographic Security**: ChaCha20 generates random numbers that are resistant to a wide range of attacks, making it suitable for security-sensitive applications.
* **High Performance**: ChaCha20 is optimized for performance, providing fast random number generation on a wide variety of hardware, from small embedded systems to powerful servers.
* **Simplicity**: The design of ChaCha20 is straightforward, ensuring ease of implementation and reducing the risk of errors.

**How ChaCha20 Works as an RNG:**

ChaCha20 operates on 512-bit blocks, divided into 16 words of 32 bits each. When used as an RNG, the basic process remains the same as in its encryption use, but instead of XORing the keystream with plaintext, the keystream itself is used directly as the source of random numbers.

Here’s how you can implement ChaCha20 as an RNG:

1. **Initialization**: ChaCha20 initializes a 512-bit state from a 256-bit key, a 32-bit counter, and a 96-bit nonce. These values seed the RNG, ensuring that the output sequence is pseudorandom and difficult to predict.

2. **Keystream Generation**: ChaCha20 generates a keystream by processing the state through its quarter-round functions over multiple rounds. This keystream can then be directly used as random numbers.

3. **Output**: The output of the ChaCha20 block function is a series of 16 × 32-bit words (a 512-bit block), which can be used as random numbers. The implementation in this repository takes only the first two 32-bit words per block, concatenated into one 64-bit result. That trades efficiency (the other 14 words are recomputed on the next call) for simplicity and resistance to side-channel state leakage; a production CSPRNG would buffer the full block instead.

The core operation is the **quarter-round** — four 32-bit words $(a, b, c, d)$ are mixed via the ARX (Add-Rotate-XOR) pattern:

$$
\begin{aligned}
a &\leftarrow a + b, &\quad d &\leftarrow d \oplus a, &\quad d &\leftarrow d \lll 16 \\
c &\leftarrow c + d, &\quad b &\leftarrow b \oplus c, &\quad b &\leftarrow b \lll 12 \\
a &\leftarrow a + b, &\quad d &\leftarrow d \oplus a, &\quad d &\leftarrow d \lll 8 \\
c &\leftarrow c + d, &\quad b &\leftarrow b \oplus c, &\quad b &\leftarrow b \lll 7
\end{aligned}
$$

Each round of the cipher applies this to four columns, then four diagonals; ChaCha20 does 10 such double-rounds. The diagram below shows the dataflow of one quarter-round.

```mermaid
flowchart LR
  A0[a] --> AB1((+))
  B0[b] --> AB1
  AB1 --> A1[a += b]
  A1 --> XD1((XOR))
  D0[d] --> XD1
  XD1 --> R1["d = ROTL d 16"]
  R1 --> CD1((+))
  C0[c] --> CD1
  CD1 --> C1[c += d]
  C1 --> XB1((XOR))
  B0 --> XB1
  XB1 --> R2["b = ROTL b 12"]
  R2 --> note["... repeat with rotations 8 and 7"]
```

```cs
class ChaCha20 : IRandomNumberGenerator {
  
  // The number of rounds used in the ChaCha20 algorithm, as specified by RFC 7539 for enhanced security.
  private const int ROUNDS = 20;

  // State array to hold the internal state of the ChaCha20 algorithm.
  // It's initialized with 16 32-bit words: 4 constant words, 8 key words, 1 counter, and 3 nonce words.
  private readonly uint[] state=new uint[16];
  
  // Constants for positioning the counter and nonce within the state array
  // COUNTER is used to track the block number being processed.
  private const int COUNTER = 12;
  private const int NONCE_0 = COUNTER + 1;
  private const int NONCE_1 = NONCE_0 + 1;
  private const int NONCE_2 = NONCE_1 + 1;
  private const int NONCE_3 = NONCE_2 + 1;

  public void Seed(ulong seed) {

    // Set the first 4 words to ChaCha20-specific constants (from "expand 32-byte k").
    this.state[0] = 0x61707865;
    this.state[1] = 0x3320646e;
    this.state[2] = 0x79622d32;
    this.state[3] = 0x6b206574;

    // Derive the key and nonce from the seed to fill the remaining 12 state elements.
    // This design choice ensures that the nonce is unique per seed, preventing nonce reuse across different sessions.
    for (int i = 4; i < this.state.Length; ++i) {
      ulong current = SplitMix64.Next(ref seed);
      this.state[i] = (uint)((current >> 32) ^ current);
    }

    // Initialize the block counter to 0, as required by the ChaCha20 algorithm.
    this.state[COUNTER] = 0;
  }

  public ulong Next() {
    uint[] result = new uint[this.state.Length];
    ChaCha20Block(ref result);
    return ((ulong)result[0] << 32) | result[1];

    void ChaCha20Block(ref uint[] output) {

      // Copy the current state into the output buffer before applying the ChaCha20 rounds.
      for (int i = 0; i < this.state.Length; ++i)
        output[i] = this.state[i];

      for (int i = 0; i < ROUNDS; i += 2) {

        // Column rounds
        QuarterRound(ref output[0], ref output[4], ref output[8], ref output[12]);
        QuarterRound(ref output[1], ref output[5], ref output[9], ref output[13]);
        QuarterRound(ref output[2], ref output[6], ref output[10], ref output[14]);
        QuarterRound(ref output[3], ref output[7], ref output[11], ref output[15]);

        // Diagonal rounds
        QuarterRound(ref output[0], ref output[5], ref output[10], ref output[15]);
        QuarterRound(ref output[1], ref output[6], ref output[11], ref output[12]);
        QuarterRound(ref output[2], ref output[7], ref output[8], ref output[13]);
        QuarterRound(ref output[3], ref output[4], ref output[9], ref output[14]);
      }

      for (int i = 0; i < this.state.Length; ++i)
        output[i] += this.state[i];

      // Increment the block counter.
      if (++this.state[COUNTER] != 0)
        return;

      // Handle counter overflow by incrementing the nonce, ensuring continuous unique state and extending the counter space.
      // This deviates from RFC 7539 where the nonce is fixed and counter wraps around.
      // Implication: Enables generation of a larger stream.
      if (++this.state[NONCE_0] == 0)
        if (++this.state[NONCE_1] == 0)
          if (++this.state[NONCE_2] == 0)
            ++this.state[NONCE_3];

      return;

      static void QuarterRound(ref uint a, ref uint b, ref uint c, ref uint d) {
        a += b;
        d ^= a;
        d = BitOperations.RotateLeft(d, 16);
        c += d;
        b ^= c;
        b = BitOperations.RotateLeft(b, 12);
        a += b;
        d ^= a;
        d = BitOperations.RotateLeft(d, 8);
        c += d;
        b ^= c;
        b = BitOperations.RotateLeft(b, 7);
      }
    }
  }
}
```

> [!TIP]
> The NuGet library may contain an implementation that supports parametrized **constants** and **round count**.

### Salsa20 (SAL20) [^81]

[^81]: [SAL20](https://cr.yp.to/snuffle/salsafamily-20071225.pdf)

The direct predecessor of [ChaCha20](#chacha20-cc20-35), also by Daniel J. Bernstein. Both ciphers operate on a 512-bit state (sixteen 32-bit words) and produce keystream blocks via repeated *quarter-round* mixing of four words, but the wiring differs: Salsa20 alternates four **column rounds** and four **row rounds**, while ChaCha20 uses **column + diagonal** rounds. The quarter-round itself differs as well — Salsa20 mixes via additions in a 4-cycle pattern:

$$
\begin{aligned}
b &\leftarrow b \oplus \bigl((a + d) \lll 7\bigr) \\
c &\leftarrow c \oplus \bigl((b + a) \lll 9\bigr) \\
d &\leftarrow d \oplus \bigl((c + b) \lll 13\bigr) \\
a &\leftarrow a \oplus \bigl((d + c) \lll 18\bigr)
\end{aligned}
$$

whereas ChaCha20 uses pair-of-pairs additions.

ChaCha20 won out for new deployments because its diagonal-round wiring gives slightly better diffusion per round; Salsa20 remains relevant because it underlies the widely-deployed XSalsa20 / NaCl variants.

```cs
class Salsa20 : IRandomNumberGenerator {
  public ulong Next() {
    var working = (uint[])_state.Clone();
    for (var r = 0; r < _rounds; r += 2) {
      // Column rounds (four parallel quarter-rounds on each column)
      QuarterRound(ref working[0], ref working[4], ref working[8], ref working[12]);
      QuarterRound(ref working[5], ref working[9], ref working[13], ref working[1]);
      QuarterRound(ref working[10], ref working[14], ref working[2], ref working[6]);
      QuarterRound(ref working[15], ref working[3], ref working[7], ref working[11]);
      // Row rounds (four parallel quarter-rounds on each row)
      QuarterRound(ref working[0], ref working[1], ref working[2], ref working[3]);
      QuarterRound(ref working[5], ref working[6], ref working[7], ref working[4]);
      QuarterRound(ref working[10], ref working[11], ref working[8], ref working[9]);
      QuarterRound(ref working[15], ref working[12], ref working[13], ref working[14]);
    }
    for (var i = 0; i < 16; ++i) working[i] += _state[i];
    IncrementCounter();
    return ((ulong)working[0] << 32) | working[1];

    static void QuarterRound(ref uint a, ref uint b, ref uint c, ref uint d) {
      b ^= RotateLeft(a + d, 7);
      c ^= RotateLeft(b + a, 9);
      d ^= RotateLeft(c + b, 13);
      a ^= RotateLeft(d + c, 18);
    }
  }
}
```

### ISAAC-64 (ISAAC) [^51]

[^51]: [ISAAC](https://burtleburtle.net/bob/rand/isaacafa.html)

This was developed by Bob Jenkins (1996), also the author of [JSF](#jenkins-small-fast-jsf-47). ISAAC (Indirection, Shift, Accumulate, Add, and Count) uses array indirection as its primary source of non-linearity. ISAAC-64 is the 64-bit variant; the 32-bit variant historically appeared in some BSD kernels (e.g. NetBSD's `/dev/urandom` and an earlier version of `arc4random`), though modern OpenBSD and FreeBSD have moved to ChaCha20-based generators. ISAAC has been called "cryptographically secure" by its author, but later analysis (Aumasson, 2006) found small biases in the output distribution; treat its claim of security as historical rather than current best practice.

Characteristics

* **Indirection-Based**: Uses table lookups indexed by the current state, creating a highly non-linear mapping that resists cryptanalysis.
* **Large State**: 256 × 64-bit internal array plus three auxiliary variables — the large state space makes state recovery infeasible.
* **Batch Generation**: Produces 256 random values per generation cycle, amortizing the cost of the mixing pass.
* **No Multiplication**: Uses only shifts, additions, XOR, and array indexing — efficient on all platforms.

Algorithm

ISAAC-64 maintains a 256-element array and three accumulators (aa, bb, cc). Each generation pass iterates through the array, applying a cycle of four different shift patterns to the accumulator, combined with indirect table lookups. The indirection `mem[(x >> 3) & 255]` introduces non-linearity that is the core of ISAAC's security.

```cs
class Isaac : IRandomNumberGenerator {
  private const int SIZE = 256;
  private const int MASK = SIZE - 1;

  private readonly ulong[] _mem = new ulong[SIZE];
  private readonly ulong[] _results = new ulong[SIZE];
  private ulong _aa, _bb, _cc;
  private int _index;

  public void Seed(ulong seed) {
    for (var i = 0; i < SIZE; ++i)
      this._results[i] = SplitMix64.Next(ref seed);
    Initialize();
  }

  public ulong Next() {
    if (this._index >= SIZE) {
      Generate();
      this._index = 0;
    }
    return this._results[this._index++];
  }

  private void Generate() {
    ++this._cc;
    this._bb += this._cc;
    for (var i = 0; i < SIZE; ++i) {
      var x = this._mem[i];
      this._aa = (i % 4) switch {
        0 => ~(this._aa ^ (this._aa << 21)),
        1 => this._aa ^ (this._aa >> 5),
        2 => this._aa ^ (this._aa << 12),
        _ => this._aa ^ (this._aa >> 33),
      } + this._mem[(i + SIZE / 2) & MASK];

      var y = this._mem[(x >> 3) & MASK] + this._aa + this._bb;
      this._mem[i] = y;
      this._bb = this._mem[(y >> 11) & MASK] + x;
      this._results[i] = this._bb;
    }
  }
}
```

### AES-CTR DRBG [^60]

[^60]: [NIST SP 800-90A Rev. 1](https://nvlpubs.nist.gov/nistpubs/specialpublications/nist.sp.800-90ar1.pdf)

This is one of the four DRBGs ("Deterministic Random Bit Generators") specified by NIST in SP 800-90A — the standard construction used when the platform already has a strong block cipher available. The internal state is a 256-bit key $K$ and a 128-bit counter $V$. Each Generate step increments $V$, encrypts $V$ under $K$ via AES-256, and returns 64 bits of the ciphertext block. Immediately after, an internal *Update* step replaces $(K, V)$ with the encryption of two further counter values, so an attacker who later learns $(K, V)$ cannot recover prior outputs (this property is called *backtracking resistance*).

```cs
class AesCtrDrbg : IRandomNumberGenerator {
  private readonly Aes _aes = Aes.Create();           // AES-256, ECB, no padding
  private readonly byte[] _key = new byte[32];        // K
  private readonly byte[] _v = new byte[16];          // V (counter)

  public ulong Next() {
    IncrementBigEndian(this._v);
    var block = new byte[16];
    using (var enc = this._aes.CreateEncryptor())
      enc.TransformBlock(this._v, 0, 16, block, 0);
    var result = BitConverter.ToUInt64(block, 0);
    UpdateInternalState();                            // backtracking resistance
    return result;
  }
}
```

The full SP 800-90A specification also covers reseeding from a fresh entropy source, derivation functions for non-uniform seed material, and "personalization strings" — all omitted here for brevity. For production, use a vetted library (e.g. .NET's `RandomNumberGenerator.Fill`, which itself wraps a vetted DRBG).

### HMAC DRBG [^60]

This is another NIST SP 800-90A construction, this time built around HMAC-SHA-256 rather than AES. It is the standard choice when a hash function is available but a block cipher is not (common in embedded TLS stacks). State consists of two 256-bit values $K$ (the HMAC key) and $V$ (the chaining value); each step computes $V \leftarrow \text{HMAC}(K, V)$, returns 64 bits of $V$, then performs a key-rotation Update.

```cs
class HmacDrbg : IRandomNumberGenerator {
  private readonly byte[] _key = new byte[32];  // K (HMAC key)
  private readonly byte[] _v = new byte[32];    // V (chaining value)

  public ulong Next() {
    var newV = HMACSHA256.HashData(this._key, this._v);
    Array.Copy(newV, this._v, 32);
    var result = BitConverter.ToUInt64(this._v, 0);
    Update(null);                               // rotate K and V for backtracking resistance
    return result;
  }

  private void Update(byte[]? providedData) {
    // K = HMAC(K, V || 0x00 || providedData);  V = HMAC(K, V);
    // if providedData != empty:  K = HMAC(K, V || 0x01 || providedData);  V = HMAC(K, V);
    // (see SP 800-90A §10.1.2 for full pseudocode)
  }
}
```

Why two NIST DRBGs? Different platforms expose different primitives. AES-CTR DRBG is the fastest of the SP 800-90A constructions on x86-64 with AES-NI; HMAC DRBG is the most portable (any cryptographic hash function works); the original spec also defined Hash-DRBG (covered below) and Dual-EC DRBG, the latter of which was withdrawn after [the NSA backdoor was discovered](https://en.wikipedia.org/wiki/Dual_EC_DRBG#NSA_backdoor).

### Hash DRBG (SHA-256) [^60]

The third construction defined in NIST SP 800-90A: maintain a 440-bit secret state $V$ and a 440-bit constant $C$. Each Generate call produces output bytes by iteratively hashing $V$, then updates the state via $V \leftarrow V + H(\text{0x03} \mathbin\Vert V) + C + \text{reseed\_counter}$ (mod $2^{440}$) so that an attacker who later learns $V$ cannot recover prior outputs. The 440-bit width is the SP 800-90A "seedlen" for the SHA-256 instantiation.

```cs
class HashDrbg : IRandomNumberGenerator, IDisposable {
  private const int SEED_BYTES = 55; // 440 bits
  private readonly byte[] _v = new byte[SEED_BYTES];
  private readonly byte[] _c = new byte[SEED_BYTES];
  private ulong _reseedCounter;

  public ulong Next() {
    // Hashgen: output = SHA256(V)
    var block = SHA256.HashData(_v);
    var result = BitConverter.ToUInt64(block, 0);

    // V update: V = V + H(0x03 || V) + C + reseed_counter   (mod 2^440)
    var h = SHA256.HashData(Concat(new byte[] { 0x03 }, _v));
    AddInto(_v, h);
    AddInto(_v, _c);
    AddInto(_v, BitConverter.GetBytes(_reseedCounter));
    ++_reseedCounter;
    return result;
  }
}
```

Hash DRBG completes the SP 800-90A trio: pick the construction that matches your available primitive — AES (CTR DRBG), HMAC (HMAC DRBG), or any cryptographic hash function (Hash DRBG).

### Yarrow (YAR) [^36]

[^36]: [YAR](https://www.schneier.com/wp-content/uploads/2016/02/paper-yarrow.pdf)

This was developed by Bruce Schneier, John Kelsey, and Niels Ferguson. It was designed with a strong emphasis on security, efficiency, and robustness against various types of attacks. The Yarrow algorithm is particularly notable for its use in environments requiring high levels of security, such as cryptographic key generation, secure communications, and digital signatures even for environments where entropy collection may be sparse.

**Key Components of Yarrow:**

Yarrow is built around three main principles:

1. **Entropy Accumulation:**
   * Yarrow collects entropy from multiple, independent sources to ensure that the randomness it produces is unpredictable. Entropy sources might include hardware events, user inputs, and system timings.
   * Yarrow uses a technique called "entropy pool management," where entropy is collected and stored in one or more pools. These pools serve as reservoirs for the randomness that Yarrow extracts to produce secure random numbers.

2. **Reseed Mechanism:**
   * One of the core features of Yarrow is its reseeding mechanism, which allows the PRNG to periodically mix new entropy into its internal state. This process enhances security by making it difficult for an attacker to predict future outputs, even if they have knowledge of past states.
   * Yarrow defines two types of reseeding: fast reseeding and slow reseeding. Fast reseeding happens frequently and uses less entropy, while slow reseeding happens less frequently but uses more entropy, ensuring that the generator’s state is refreshed thoroughly over time.

3. **Generator Function:**
   * Yarrow uses a cryptographic algorithm, typically a block cipher like AES, to generate random outputs from its internal state. The generator function takes the internal state, processes it through the cipher, and produces pseudorandom output bits.
   * The output of the generator function is not directly influenced by the entropy inputs; instead, the generator produces a stream of random numbers from the internal state, which is periodically refreshed by the reseeding mechanism.

**Yarrow's Operation:**

The operation of Yarrow can be broken down into three main phases:

1. **Entropy Collection:**
   * Yarrow monitors various sources of entropy, such as hardware interrupts, mouse movements, and keystrokes. Each event contributes a small amount of randomness to one or more entropy pools.
   * The amount of entropy estimated from each event is carefully measured, ensuring that Yarrow does not overestimate the randomness it collects.

2. **Reseeding:**
   * Periodically, Yarrow assesses whether enough entropy has been collected to perform a reseed. This decision is made based on thresholds defined for the fast and slow reseeding processes.
   * During reseeding, entropy from the pools is mixed into the internal state of the PRNG. Fast reseeding adds less entropy and occurs more frequently, while slow reseeding adds more entropy and occurs less often.

3. **Random Number Generation:**
   * Once reseeded, Yarrow uses its generator function to produce pseudorandom numbers. The internal state, influenced by the entropy pools, ensures that the output is unpredictable.
   * The block cipher used in the generator function provides cryptographic strength to the generated numbers, making them suitable for high-security applications.

**Security Considerations:**

Yarrow is designed to be robust against several types of attacks:

* **State Compromise Extension Attacks:**
  Yarrow’s reseeding mechanism ensures that even if an attacker compromises the internal state of the generator, they will not be able to predict future outputs after a reseed has occurred. This resilience is critical for maintaining security over long periods.

* **Backtracking Resistance:**
  The internal state of Yarrow is regularly updated with fresh entropy, making it difficult for an attacker to reconstruct previous outputs, even if they obtain the current state.

* **Predictable Entropy Sources:**
  Yarrow is designed to handle cases where some entropy sources may be less random or potentially biased. By combining multiple sources and carefully managing entropy, Yarrow ensures the randomness of its output remains high.

The implementation in this repository uses AES-256 in counter mode for output and SHA-256 to mix the fast pool into the key on reseed. Since no real entropy source is wired in, output bytes are stirred back into the fast pool as an educational substitute — production use requires real entropy injection from a hardware RNG or OS service.

```cs
class Yarrow : IRandomNumberGenerator, IDisposable {
  public ulong Next() {
    if (FastPoolReady() || OutputCapReached()) Reseed();
    IncrementCounter(_counter);
    var block = AesEncrypt(_counter);
    StirIntoFastPool(block);             // educational: real Yarrow takes entropy here
    return BitConverter.ToUInt64(block);
  }

  private void Reseed() {
    // K = SHA256(K || fastPool); reset fast pool and output counter.
    _key = SHA256.HashData(Concat(_key, _fastPool[.._fastPoolFill]));
    _aes.Key = _key;
  }
}
```

### Fortuna (FORT) [^37] [^38]

[^37]: [FORT](https://www.codeproject.com/Articles/6321/Fortuna-A-Cryptographically-Secure-Pseudo-Random-N)

[^38]: [FORT-Paper](https://www.schneier.com/wp-content/uploads/2015/12/fortuna.pdf)

This was designed by Bruce Schneier, Niels Ferguson, and other cryptographers. It was developed as a successor to the [YAR](#yarrow-yar) and addresses some of its limitations, while also improving security and flexibility. Fortuna is notable for its simplicity, robustness, and the ability to withstand various types of attacks, making it a reliable choice for cryptographic applications.

**Key Components:**

It consists of several key components that work together to ensure the generation of secure random numbers:

1. **Entropy Accumulators:**
   * Fortuna gathers entropy from multiple sources and stores it in a series of 32 entropy pools. Each pool is designed to accumulate entropy independently, ensuring that even if one pool is compromised, others remain secure.
   * Entropy sources can include system events, user inputs, hardware noise, and more. The randomness from these sources is distributed across the pools using a round-robin technique, ensuring an even distribution of entropy.

2. **Reseeding Mechanism:**
   * Fortuna uses a reseeding mechanism to periodically refresh its internal state with new entropy from the pools. The reseeding process is triggered after generating a certain number of outputs or after a specified time interval.
   * Each reseeding operation pulls entropy from one or more of the pools, with the frequency of use varying among them. The first pool is used frequently, while the others are used less often, based on a geometric series. This staggered use ensures that even if an attacker gains knowledge of the generator’s state, they cannot easily predict future outputs.

3. **Generator Function:**
   * Fortuna’s generator function is built around a cryptographic algorithm, typically AES in counter mode (AES-CTR). The internal state of the generator, combined with the counter, is used to produce a stream of random numbers.
   * The use of AES-CTR ensures that the output is cryptographically secure and resistant to attacks. The generator also employs key updates after every reseed, further enhancing security.

4. **Key Management:**
   * Fortuna includes a key management process that updates the generator's key after each reseed. This key update is crucial for maintaining the security of the random number generator, especially in the face of potential state compromises.
   * By regularly changing the key, Fortuna ensures that even if part of the state is leaked or compromised, future outputs remain secure.

**Fortuna’s Operation:**

The operation of Fortuna can be divided into three main phases:

1. **Entropy Collection:**
   * Fortuna collects entropy from various sources and distributes it across the 32 pools. The distribution is managed in a way that ensures each pool receives entropy, but some pools will accumulate more slowly than others.

2. **Reseeding:**
   * Reseeding occurs at regular intervals or after a certain amount of data has been generated. During a reseed, Fortuna combines entropy from one or more of the pools to update its internal state and key.
   * The frequency and pool selection for reseeding are designed to prevent attackers from predicting or controlling the reseeding process, thereby maintaining the security of the PRNG.

3. **Random Number Generation:**
   * Once seeded, Fortuna uses its generator function to produce random numbers. The generator runs AES-CTR to produce a secure stream of bits, which are then used as random numbers for cryptographic purposes.
   * The counter used in AES-CTR mode is incremented with each output, ensuring a fresh and unpredictable sequence of random numbers.

**Security Considerations:**

Fortuna was designed with several key security principles in mind:

* **Resilience Against State Compromise:**
  Fortuna’s multiple entropy pools and staggered reseeding ensure that even if an attacker gains partial knowledge of the internal state, they cannot easily predict future outputs. The periodic key updates further protect the generator from state compromise attacks.

* **Flexible and Scalable:**
  Fortuna is designed to be flexible and scalable, capable of handling different amounts of entropy and operating in a variety of environments. The use of 32 pools allows the system to adapt to different levels of entropy input while maintaining security.

* **Defense Against Entropy Source Attacks:**
  Fortuna’s design assumes that some entropy sources may be compromised or less random. By using multiple independent pools, the generator mitigates the risk of any single source influencing the overall security of the system.

The implementation in this repository uses 32 SHA-256 pools, AES-256 in counter mode for output, and the geometric reseed schedule (pool $i$ contributes when $2^i \mid \text{reseedCount}$). As with Yarrow, the educational version stirs output back into pools rather than collecting real entropy.

```cs
class Fortuna : IRandomNumberGenerator, IDisposable {
  public ulong Next() {
    if (_pool0Bytes >= ResetThreshold) Reseed();
    IncrementCounter(_counter);
    var block = AesEncrypt(_counter);
    _pools[_nextPool].TransformBlock(block);  // stir into round-robin pool
    _nextPool = (_nextPool + 1) % 32;
    return BitConverter.ToUInt64(block);
  }

  private void Reseed() {
    ++_reseedCount;
    var combiner = SHA256.Create();
    combiner.TransformBlock(_key);
    for (int i = 0; i < 32; ++i) {
      if ((_reseedCount & ((1 << i) - 1)) != 0) break;  // 2^i must divide _reseedCount
      combiner.TransformBlock(_pools[i].FinalHash());
      _pools[i] = SHA256.Create();
    }
    _key = combiner.FinalHash();
    _aes.Key = _key;
  }
}
```

### ANSI X9.17 (ANSI) [^39]

[^39]: [ANSI](https://www.researchgate.net/publication/267297736_EFFICIENT_COMBINATION_OF_SEVERAL_TECHNIQUES_IN_THE_DESIGN_AND_IMPLEMENTATION_OF_A_NETWORKS_SECURITY_SYSTEM)

This standard specifies a method for generating cryptographically secure random numbers, particularly for the purpose of key generation. It leverages the [Data Encryption Standard (DES)](https://en.wikipedia.org/wiki/Data_Encryption_Standard), which was the prevailing symmetric key encryption algorithm at the time of writing in 1985. The RNG process involves a combination of a seed value, a date/time component, and a secret key, all of which are processed using DES to produce a CSRN.

**Process:**

1. **Initialization**:
   * The process begins with a secret key $𝐾$, which is used with the DES algorithm.
   * A 64-bit seed value $𝑉_0$ is also required. This seed value is usually derived from a secure source of entropy and is kept secret.

2. **Generation Loop**:
   * Step 1: Concatenate the current date/time (in a 64-bit format) with the seed value $𝑉_𝑛$ to form a 128-bit input.
   * Step 2: Encrypt this 128-bit input using the DES algorithm with key $K$ to produce a 64-bit output.
   * Step 3: The output from Step 2 is XORed with the current date/time to produce the next seed value  $𝑉_{n+1}$.
   * Step 4: Encrypt the new seed value $𝑉_{n+1}$ using the DES algorithm with key $𝐾$ to produce the final 64-bit random number.

3. **Iteration**:
   * The newly generated seed value $𝑉_{n+1} is used in subsequent iterations, ensuring that each random number is dependent on the previous one, the date/time, and the secret key.

**Security Considerations:**

* DES Dependency: The security of the ANSI X9.17 RNG is tightly coupled with the security of DES. As DES has a 56-bit key length, which is now considered insecure against brute-force attacks, the ANSI X9.17 standard is generally considered obsolete for modern cryptographic applications.
* Key Management: The secrecy of the key $𝐾$ is critical. If the key is compromised, the entire sequence of generated random numbers can be predicted, undermining the security of the system.
* Entropy Source: The initial seed $𝑉_0$ and the date/time component must be chosen carefully to ensure that the generator's output remains unpredictable. Any weakness in these components can lead to predictability in the generated numbers.

The implementation in this repository follows the modernised ANSI X9.31 update: AES-128 replaces DES, and the date/time component is substituted with a monotonic counter so the algorithm becomes self-contained for testing.

```cs
class AnsiX931 : IRandomNumberGenerator, IDisposable {
  public ulong Next() {
    var d = CounterAsBlock(_counter++);          // D substitute (was: date/time)
    var i = AesEncrypt(d);                       // I = E(K, D)
    var r = AesEncrypt(Xor(i, _v));              // R = E(K, I XOR V) — this is the output
    _v = AesEncrypt(Xor(r, i));                  // V = E(K, R XOR I) — state update
    return BitConverter.ToUInt64(r);
  }
}
```

## Drinking Bit-Soup

### When you need less bits than the RNG provides

In cryptographic applications and other scenarios where precise control over the number of random bits is required, it's common to encounter situations where you need fewer bits than what the RNG provides. For example, an RNG might output 64 bits at a time, but your application only requires 16 bits, or you need numbers in a specific range (e.g. 1 to 6). Simply discarding the extra bits might seem straightforward, but this approach can lead to inefficiencies or, worse, security vulnerabilities if not handled correctly.

Here are several methods to extract exactly the number of bits you need, along with their potential pitfalls and ways to mitigate them:

#### Truncating

```mermaid
graph TD
  V[Random: 64-bit Value]
  
  subgraph Truncation
    H[Upper 32 Bits]
    L[Lower 32 Bits]
  end

  R[Result: 32-bit Value]

  V-->H
  V-->L
  L-->R
```

This involves discarding the higher bits and retaining only the lower bits. This method is simple and efficient when the lower bits are sufficient for your needs. Due to the involved data-types both halves have the same size.

```cs
uint Truncate32(IRandomNumberGenerator instance) => (uint)instance.Next();
```

> [!CAUTION]
> If the RNG's output is not uniformly distributed across all bits, this can exacerbate non-uniformities, especially if the lower bits are less random.

#### Shifting

```mermaid
graph TD
  V[Random: 64-bit Value]
  
  subgraph Shifting
    H[Upper 8 Bits]
    L[Lower 24 Bits]
  end

  R[Result: 8-Bit Value]

  V-->H
  V-->L
  H-->R
```

This involves discarding the lower bits by right-shifting the RNG output, effectively keeping only the higher bits. Arbitrary shift values are possible making the number of output bits very flexible.

```cs
byte Shift8(IRandomNumberGenerator instance) => (byte)(instance.Next() >> 56);
```

> [!CAUTION]
> Similar to truncating, if the RNG has non-uniformity issues in its higher bits, shifting can result in biased outputs.

#### Masking

```mermaid
graph TD
  V[Random: 8-bit Value]
  
  subgraph Bit-Mask
    B7[7]
    B6[6]
    B5[5]
    B4[4]
    B3[3]
    B2[2]
    B1[1]
    B0[0]
  end

  R[Result: 4-bit Value]

  V-->B7
  V-->B6
  V-->B5
  V-->B4
  V-->B3
  V-->B2
  V-->B1
  V-->B0
  B6-->R
  B3-->R
  B1-->R
  B0-->R
```

This allows you to take specific bits from the RNG output by applying a bitmask. This method is useful when you need a certain range of bits from the output. Depending on the implementation it might be possible to extract non-contiguous bits and combine them together into the final output.

```cs
ushort Mask16(IRandomNumberGenerator instance) => (ushort)((instance.Next() & 0x000000FFFF000000) >> 24);
```

> [!CAUTION]
> Masking can also suffer from the same non-uniformity issues as truncating and shifting if the RNG is biased in the selected bit range.

#### Sponging

```mermaid
block-beta
  columns 3
  V["Random: 8-bit Value"]:3
    space:3

    V4Upper["Upper 4 Bits"]
    space
    V4Lower["Lower 4 Bits"]
    
    space:3

    space
    V2["Intermediate 4 Bits"]
    space
    
    space:3

    V2Upper["Upper 2 Bits"]
    space
    V2Lower["Lower 2 Bits"]
    
    space:3

  R["Result: 2-bit Value"]:3

  V4Upper--"⊕"-->V4Lower
  V2Upper--"⊕"-->V2Lower
  V--->V4Upper
  V--->V4Lower
  V4Lower--->V2
  V2--->V2Upper
  V2--->V2Lower
  V2Lower--->R
```

This technique involves repeatedly XORing the RNG output with itself after progressively smaller right shifts. This ensures that the final extracted bit(s) are influenced by all bits in the RNG output, increasing entropy and security. Due to the construction of the sponge the output bit count is a power of 2.

```cs
bool Sponge1(IRandomNumberGenerator instance) {
  ulong result = instance.Next();
  result ^= result >> 32; // HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL -> XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
  result ^= result >> 16; // 00000000000000000000000000000000HHHHHHHHHHHHHHHHLLLLLLLLLLLLLLLL -> XXXXXXXXXXXXXXXX
  result ^= result >> 8;  // 000000000000000000000000000000000000000000000000HHHHHHHHLLLLLLLL -> XXXXXXXX
  result ^= result >> 4;  // 00000000000000000000000000000000000000000000000000000000HHHHLLLL -> XXXX
  result ^= result >> 2;  // 000000000000000000000000000000000000000000000000000000000000HHLL -> XX
  result ^= result >> 1;  // 00000000000000000000000000000000000000000000000000000000000000HL -> X
  return (result & 1) != 0;
}
```

> [!TIP]
> This method helps distribute any biases uniformly across all bits, making it more resilient to non-uniformity issues.

#### Construction

```mermaid
graph TB
  V0[Random: 64-bit Value]
  V1[Random: 64-bit Value]
  
  subgraph Construction

    B0[3 Bits]
    B1[3 Bits]
    
  end

  R[Result: 6-bit Value]

V0-->B0
V1-->B1
B0 & B1-->R
```

In this method, you repeatedly call the RNG to generate the exact number of bits you need. This approach can be useful when you need a non-standard number of bits (e.g., 24 bits) and want to ensure each bit is generated with uniform randomness.

```cs
byte Construct6(IRandomNumberGenerator instance) {
  int result = 0;
  for (int i = 0; i < 2 ; ++i) {
    int s = instance.Next();
    int x = (s & (1 << 62)) >> 62; // Take bit 62
    int y = (s & (1 <<  7)) >>  7; // Take bit 7
    int z = (s & (1 << 31)) >> 31; // Take bit 31
    result = x | y << 1 | z << 2 | result << 3;
  }

  return result;
}
```

> [!CAUTION]
> If the RNG is biased for certain bits, this approach can accumulate those biases across multiple calls, leading to a non-uniform final output.

#### Slicing

```mermaid
graph TB
  V[Random: 64-bit Value]
  
  subgraph Slicing

    B0[16 Bits]
    B1[16 Bits]
    B2[16 Bits]
    B3[16 Bits]
    
  end

  R0[Result: 16-bit Value]
  R1[Result: 16-bit Value]
  R2[Result: 16-bit Value]
  R3[Result: 16-bit Value]

V-->B0 & B1 & B2 & B3
B0-->R0
B1-->R1
B2-->R2
B3-->R3
```

This involves splitting the RNG output into smaller parts and using those parts as needed. This can be useful when you need multiple smaller random values from a single RNG output.

```cs
(ushort, ushort, ushort, ushort) Slice16x4(IRandomNumberGenerator instance) {
  ulong result = instance.Next();
  ushort part1 = (ushort)(result & 0xFFFF);
  ushort part2 = (ushort)((result >> 16) & 0xFFFF);
  ushort part3 = (ushort)((result >> 32) & 0xFFFF);
  ushort part4 = (ushort)((result >> 48) & 0xFFFF);
  return (part1, part2, part3, part4);
}
```

> [!CAUTION]
> If the RNG has non-uniform distribution across different bit ranges, slicing can result in biased sub-values.

#### Modulo Operation

```mermaid
flowchart TD
  subgraph Random["Random: 64-bit Values"]
    direction LR
    R1("9")
    R3("62")
    R4("15")
    R2("21")
    R5("100")
    R6("1502")
    R7("999")
    R8("...")
  end

  subgraph Modulo["Modulo  3"]
    direction LR
    A1("0")
    A3("2")
    A4("0")
    A2("1")
    A5("1")
    A6("2")
    A7("0")
    A8("...")
  end

  R1 --> A1
  R2 --> A2
  R3 --> A3
  R4 --> A4
  R5 --> A5
  R6 --> A6
  R7 --> A7
  R8 --> A8
  
```

This operation is commonly used to reduce a large random number to a smaller range. For example, reducing a 64-bit RNG output to a value between 0 and 19.

```cs
byte NextD20(IRandomNumberGenerator instance) => (byte)(1 + instance.Next() % 20);
```

> [!CAUTION]
> [Modulo Bias](https://research.kudelskisecurity.com/2020/07/28/the-definitive-guide-to-modulo-bias-and-how-to-avoid-it/) occurs when the RNG output is not perfectly divisible by the target range. This bias can make certain values more likely than others.

```mermaid
xychart-beta
x-axis [0, 1, 2, 3, 4, 5, 6, 7, 8]
y-axis 0 --> 1100
bar [1001, 998, 1013, 802, 775, 799, 801, 805, 787]
```

#### Rejection Sampling

```mermaid
flowchart TD
    subgraph Random["Random: 64-bit Values"]
    direction LR
    R1("0")
    R3("17")
    R4("2")
    R2("100")
    R5("19337")
    R6("1")
    R7("0")
    R8("...")
  end

  subgraph Rejection
    direction LR
    A1("0")
    A4("2")
    A6("1")
    A7("0")
    A8("...")
  end
  
  R1-->A1
  R4-->A4
  R6-->A6
  R7-->A7
  R8-.->A8

  style R1 fill:#32CD32
  style R4 fill:#32CD32
  style R6 fill:#32CD32
  style R7 fill:#32CD32
  style R2 fill:#FA8072
  style R3 fill:#FA8072
  style R5 fill:#FA8072
```

This is a technique to avoid modulo bias by discarding values that would introduce bias. This method involves generating random numbers until one falls within the desired range without bias.

```cs
byte NextD6(IRandomNumberGenerator instance) {
  ulong result;
  do {
    result = instance.Next();
  } while (result >= 6);
  return (byte)(1 + result);
}
```

> [!CAUTION]
> While this method eliminates modulo bias, it can be slow and inefficient, especially if the range is small compared to the RNG output, leading to frequent rejections.

#### Modulo-Rejection Combination

A combination of both [modulo operation](#modulo-operation) and [rejection sampling](#rejection-sampling) can be used to strike a balance between efficiency and eliminating bias.

```cs
byte NextD12(IRandomNumberGenerator instance) {
  ulong result;
  ulong maxValidRange = ulong.MaxValue - (ulong.MaxValue % 12);
  do {
    result = instance.Next();
  } while (result >= maxValidRange);
  return (byte)(1 + (result % 12));
}
```

> [!TIP]
> This method effectively reduces bias while improving efficiency over pure rejection sampling, especially when working with large RNG outputs.

#### Scaling

This method involves normalizing the RNG's output to a floating-point value between $[0.0, 1.0]$, then scaling it to the desired range.

```cs
byte NextD4(IRandomNumberGenerator instance) {
  // Divide by (ulong.MaxValue + 1.0) instead of ulong.MaxValue so the quotient is strictly in [0.0, 1.0),
  // never exactly 1.0; otherwise (1 + 1.0 * 4) would produce 5, which is out of range.
  double u = instance.Next() / (ulong.MaxValue + 1.0);
  return (byte)(1 + u * 4);
}
```

> [!CAUTION]
> The method relies on floating-point arithmetic, which may introduce inaccuracies due to the finite precision of `double`. Note also that dividing by `ulong.MaxValue` (rather than `ulong.MaxValue + 1.0`) makes the upper bound *inclusive*, producing one out-of-range value at the very largest input.

#### Reals

This method constructs floating-point numbers between $[0.0, 1.0)$ according to the [IEEE 754 standard](https://en.wikipedia.org/wiki/IEEE_754), focusing on generating the mantissa randomly while keeping the exponent and sign fixed.

```cs
float NextSingle() {
  uint mantissa = (uint)(rng.Next() >> (64 - 23)); // Extract 23 bits for the mantissa
  uint floatBits = (127 << 23) | mantissa;         // 127 is the biased exponent for 2^0 in single-precision
  return BitConverter.Int32BitsToSingle((int)floatBits) - 1.0f;
}

double NextDouble() {
  ulong mantissa = rng.Next() >> (64 - 52);         // Extract 52 bits for the mantissa
  ulong doubleBits = (1023UL << 52) | mantissa;     // 1023 is the biased exponent for 2^0 in double-precision
  return BitConverter.Int64BitsToDouble((long)doubleBits) - 1.0d;
}
```

> [!CAUTION]
> This approach uses the shifting method to generate the mantissa, which can inherit flaws if the underlying RNG has non-uniformity issues in certain bits. The distribution of the resulting floating-point values might be slightly biased, particularly if the RNG doesn't produce truly uniform random bits across its entire range.

### Arbitrary RNG Part I

Given the methods, we can now create a more generic class to deal with that:

```cs
partial class ArbitraryNumberGenerator(IRandomNumberGenerator rng) : IRandomNumberGenerator {

  public uint Truncate32() => (uint)rng.Next();
  public ushort Truncate16() => (ushort)rng.Next();
  public byte Truncate8() => (byte)rng.Next();
  public bool Truncate1() => (rng.Next() & 1) == 1;

  public ulong Truncate(byte bitCount) {
    ArgumentOutOfRangeException.ThrowIfZero(bitCount);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, (byte)63);
    
    return rng.Next() & ((1UL << bitCount) - 1);
  }

  public uint Shift32() => (uint)(rng.Next() >> 32);
  public ushort Shift16() => (ushort)(rng.Next() >> 48);
  public byte Shift8() => (byte)(rng.Next() >> 56);
  public bool Shift1() => (rng.Next() >> 63) == 1;

  public ulong Shift(byte bitCount) {
    ArgumentOutOfRangeException.ThrowIfZero(bitCount);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, (byte)63);
    
    return rng.Next() >> (64 - bitCount);
  }


  public uint Mask32(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 32UL, nameof(mask));
    
    return (uint)_ParallelBitExtract(rng.Next(), mask);
  }

  public ushort Mask16(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 16UL, nameof(mask));
    
    return (ushort)_ParallelBitExtract(rng.Next(), mask);
  }

  public byte Mask8(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 8UL, nameof(mask));
    
    return (byte)_ParallelBitExtract(rng.Next(), mask);
  }

  public bool Mask1(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 1UL, nameof(mask));
    
    return _ParallelBitExtract(rng.Next(), mask) != 0;
  }

  public ulong Mask(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 63UL, nameof(mask));
    
    return _ParallelBitExtract(rng.Next(), mask);
  }

  public uint Sponge32() {
    ulong result = rng.Next();
    result ^= result >> 32;
    return (uint)result;
  }

  public ushort Sponge16() {
    ulong result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    return (ushort)result;
  }

  public byte Sponge8() {
    ulong result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    result ^= result >> 8;
    return (byte)result;
  }

  public byte Sponge4() {
    ulong result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    result ^= result >> 8;
    result ^= result >> 4;
    return (byte)(result & 0xF);
  }

  public byte Sponge2() {
    ulong result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    result ^= result >> 8;
    result ^= result >> 4;
    result ^= result >> 2;
    return (byte)(result & 0x3);
  }

  public bool Sponge1() {
    ulong result = rng.Next();
    result ^= result >> 32; 
    result ^= result >> 16; 
    result ^= result >> 8;  
    result ^= result >> 4;  
    result ^= result >> 2;  
    result ^= result >> 1;  
    return result != 0;
  }

  public ulong Construct(byte bitsTotal, ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(bitsTotal);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitsTotal, 64);
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    byte bitsPerRound = (byte)BitOperations.PopCount(mask);
    ArgumentOutOfRangeException.ThrowIfNotEqual(bitsTotal % bitsPerRound,0, nameof(mask));
    
    ulong result = 0UL;
    do {
      ulong random = rng.Next();
      ulong roundBits = _ParallelBitExtract(random, mask);
      result <<= bitsPerRound;
      result |= roundBits;
      bitsTotal -= bitsPerRound;
    } while (bitsTotal > 0);

    return result;
  }

  public (uint,uint) Slice32x2() {
    SliceUnion random = new SliceUnion(rng.Next());
    return (
      random.R32_0,
      random.R32_1
    );
  }

  public (ushort, ushort, ushort, ushort) Slice16x4() {
    SliceUnion random = new SliceUnion(rng.Next());
    return (
      random.R16_0,
      random.R16_1,
      random.R16_2,
      random.R16_3
    );
  }

  public (byte, byte, byte, byte, byte, byte, byte, byte) Slice8x8() {
    SliceUnion random = new SliceUnion(rng.Next());
    return (
      random.R8_0,
      random.R8_1,
      random.R8_2,
      random.R8_3,
      random.R8_4,
      random.R8_5,
      random.R8_6,
      random.R8_7
    );
  }

  public ulong Modulo(ulong mod) => rng.Next() % mod;
  
  public ulong RejectionSampling(ulong mod) {
    ulong result;
    do 
      result = rng.Next(); 
    while (result >= mod);

    return result;
  }

  public ulong ModuloRejectionSampling(ulong mod) {
    ulong maxValidRange = ulong.MaxValue - ulong.MaxValue % mod;
    ulong result;
    do 
      result = rng.Next();
    while (result >= maxValidRange);

    return result % mod;
  }

  public double Scale(double scale) => rng.Next() * scale / ulong.MaxValue;

  public float NextSingle() {
    uint mantissa = (uint)(rng.Next() >> (64 - 23));
    uint floatBits = (127 << 23) | mantissa;
    return BitConverter.Int32BitsToSingle((int)floatBits) - 1.0f;
  }

  public double NextDouble() {
    ulong mantissa = rng.Next() >> (64 - 52);
    ulong doubleBits = (1023UL << 52) | mantissa;
    return BitConverter.Int64BitsToDouble((long)doubleBits) - 1.0d;
  }
  
}
```

### When you need more bits than the RNG provides

In some scenarios, the output of an RNG may be too limited in size, and you might need more bits than it provides in a single call. This situation is common in cryptographic applications, simulations, or when working with RNGs that produce relatively small outputs (e.g., 32-bit RNGs) but require larger random values (e.g., 128-bit or 256-bit numbers). Here are a few methods to handle such situations:

#### Concatenation

This method involves generating multiple smaller random numbers from the RNG and then concatenating them to form a larger random number.

```cs
UInt128 Concat128() => (UInt128)rng.Next() << 64 | rng.Next();
```

> [!CAUTION]
> Ensure that each call to the RNG is independent and that the RNG's internal state changes adequately between calls. If the RNG has any weaknesses or patterns in its output, concatenation can amplify these flaws.

#### SpreadBits

This is a technique where the bits of a small RNG output are "spread" or "stretched" over a larger bit space. This is typically done using bitwise operations that distribute the original bits across the desired output size.

```cs
UInt128 SpreadBits128(UInt128 mask) {
  int bitCount = 
    BitOperations.PopCount((ulong)mask) 
    + BitOperations.PopCount((ulong)(mask >> 64))
    ;

  ulong random = rng.Next();
  UInt128 result = UInt128.Zero;
  for (int i = 0; i < bitCount; ++i) {
    UInt128 bit = random & 1;
    random >>= 1;

    int upperZero= BitOperations.TrailingZeroCount((ulong)(mask >> 64));
    int nextPosition = BitOperations.TrailingZeroCount((ulong)mask);
    if (nextPosition == 64)
      nextPosition += upperZero;

    result |= bit << nextPosition;
    mask &= ~(UInt128.One << nextPosition);
  }
  
  return result;
}
```

> [!CAUTION]
> This leaves a lot of bits at zero so maintaining randomness is not guaranteed.

#### SplitMix

This method is used to expand a smaller RNG output into a larger one by applying a mixing function that scrambles the bits and produces additional randomness.

```cs
Vector256<ulong> SplitMix256() {
  ulong random = rng.Next();
  return Vector256.Create(
    random, 
    SplitMix64.Next(ref random), 
    SplitMix64.Next(ref random), 
    SplitMix64.Next(ref random)
  );
}
```

> [!CAUTION]
> The quality of the output heavily depends on the mixing function used. A poor choice can lead to weak or biased random values. However, when done correctly, SplitMix can produce high-quality random numbers.

#### Feistel-Network

The [Feistel network](https://en.wikipedia.org/wiki/Feistel_cipher) technique enhances the randomness of bits generated by a basic RNG by applying a structure commonly used in cryptographic algorithms like [DES (Data Encryption Standard)](https://en.wikipedia.org/wiki/Data_Encryption_Standard). A Feistel network splits the data into two halves, applies a round function with a key, and then swaps the halves, repeating this process to achieve strong diffusion. This makes it an excellent method to improve the distribution and randomness of the output bits, ensuring they are more secure and less predictable.

One round of a 64-bit Feistel network looks like this — the right half passes through a key-dependent round function `F`, the result is XORed into the left half, then the halves swap. Repeating the round many times yields strong diffusion even when `F` is relatively simple:

```mermaid
flowchart LR
  L0[Left 32-bit] --> X1((XOR))
  R0[Right 32-bit] --> F[F: round function + key]
  F --> X1
  X1 --> R1[Right' = old Left XOR F]
  R0 -.-> L1[Left' = old Right]
```

```cs
IEnumerable<byte> FeistelGenerator() {
  ulong state = rng.Next();
  ulong key = rng.Next();
  ulong counter = rng.Next();

  int counterIndex = 0;
  for (;;) {
    int roundBits = (int)(counter >> counterIndex) & 0b1111;
    counterIndex += 4;
    if (counterIndex >= 64) {
      (counter, state) = (state, counter);
      counterIndex = 0;
    }

    ++roundBits; // at least one round each time
    for (int i = 0; i < roundBits; ++i) {
      DoFeistelRound(ref state, key);
      DoFeistelRound(ref state, key);
      (state, key) = (key, state);
    }

    SliceUnion result = new SliceUnion(state);
    yield return result.R8_0;
    yield return result.R8_1;
    yield return result.R8_2;
    yield return result.R8_3;
    yield return result.R8_4;
    yield return result.R8_5;
    yield return result.R8_6;
    yield return result.R8_7;
  }
  
  void DoFeistelRound(ref ulong plainText, ulong roundKey) {
    uint left = (uint)plainText;
    uint right = (uint)(plainText>>32);
    left ^= RoundFunction(right, roundKey);
    (left, right) = (right, left);
    plainText = left | (ulong)right << 32;
  }

  uint RoundFunction(uint right,ulong roundKey) {
    uint result=BitOperations.RotateLeft(right, 3);
    result ^= (uint)roundKey;
    result = BitOperations.RotateRight(result, 17);
    result ^= (uint)(roundKey >> 32);
    return result;
  }
}
```

> [!CAUTION]
> The randomness quality of the output still heavily depends on the initial entropy provided by the RNG and the round function. If the RNG is weak or biased, the Feistel network alone may not sufficiently mitigate these issues.

#### Hash-Function

This technique uses a [cryptographic hash function](https://en.wikipedia.org/wiki/Cryptographic_hash_function) to enhance and expand the randomness provided by an RNG. By seeding the hash function with an initial RNG output and then iteratively rehashing the result with a counter, you can generate a stream of random bits with strong diffusion properties. This method leverages the [avalanche effect](https://en.wikipedia.org/wiki/Avalanche_effect) of hash functions, where a small change in input (like the counter) drastically changes the output, making it a powerful tool for random number generation.

```cs
IEnumerable<byte> HashGenerator() {
  HashAlgorithm instance = SHA512.Create();
  int entropyBitsNeeded = instance.HashSize;
  int entropyBytesNeeded = entropyBitsNeeded >> 3;
  
  // Generate the initial salt using RNG
  byte[] salt = this.ConcatGenerator().Take(entropyBytesNeeded).ToArray();
  
  // Initialize the counter
  byte[] counter = new byte[entropyBytesNeeded];

  for (;;) {
    
    // Combine the salt and counter using XOR
    byte[] plainData = salt.Zip(counter, (s, c) => (byte)(s ^ c)).ToArray();
    
    // Generate the hash
    byte[] hash = instance.ComputeHash(plainData);
    
    // Yield each byte of the hash as part of the random stream
    foreach (byte entry in hash)
      yield return entry;

    // Increment the counter
    for (int i = 0; i < counter.Length; ++i)
      if (++counter[i] != 0)
        break;
  }
}
```

> [!CAUTION]
> The initial seed from the RNG is critical to the quality of the randomness produced. If the RNG has low entropy or is biased, the resulting hash stream might not be as random as expected. Always ensure that the underlying RNG is secure and provides sufficient entropy.

#### Cipher

This method leverages a block cipher in Counter (CTR) mode to generate a stream of random bytes. CTR mode is a popular choice for such applications because it turns a block cipher into a stream cipher, allowing it to generate a continuous stream of pseudorandom bits, which can be used as random numbers. Other mode may also be possible depending on the concrete cipher used.

```cs
IEnumerable<byte> CipherGenerator() {
  using SymmetricAlgorithm instance = Aes.Create();
  instance.Mode = CipherMode.ECB; // CTR mode is simulated with ECB
  instance.Padding = PaddingMode.None;

  // Generate a random key and initialization vector (IV)
  byte[] key = this.ConcatGenerator().Take(instance.KeySize >> 3).ToArray();

  int blockSizeInBytes = instance.BlockSize >> 3;
  byte[] iv = this.ConcatGenerator().Take(blockSizeInBytes).ToArray();
  
  instance.Key = key;
  instance.IV = iv;

  // Initialize the counter
  byte[] counter = new byte[blockSizeInBytes];

  byte[] cipherText = new byte[blockSizeInBytes];
  using ICryptoTransform encryptor = instance.CreateEncryptor();
  for (;;) {
    
    // Encrypt the counter block
    encryptor.TransformBlock(counter, 0, blockSizeInBytes, cipherText, 0);

    // Yield each byte from the encrypted block as random output
    foreach (byte value in cipherText)
      yield return value;

    // Increment the counter
    for (int i = 0; i < counter.Length; ++i)
      if (++counter[i] != 0)
        break;
  }
}
```

> [!CAUTION]
> Ensure that the key and IV generated by the RNG are of high entropy and secure. A weak key or IV can compromise the security of the entire system. While this method is robust and secure, the overhead of encryption may make it slower than simpler RNG methods.

### Arbitrary RNG Part II

Given those new tricks, we can now enhance our more generic class to provide that:

```cs
partial class ArbitraryNumberGenerator {
  
  public UInt128 Concat128() => (UInt128)rng.Next() << 64 | rng.Next();
  
  public Vector256<ulong> Concat256() => Vector256.Create(
    rng.Next(), rng.Next(), rng.Next(), rng.Next()
  );
  
  public Vector512<ulong> Concat512() => Vector512.Create(
    rng.Next(), rng.Next(), rng.Next(), rng.Next(),
    rng.Next(), rng.Next(), rng.Next(), rng.Next()
  );

  public IEnumerable<byte> ConcatGenerator() {
    for (;;) {
      SliceUnion random = new SliceUnion(rng.Next());
      yield return random.R8_0;
      yield return random.R8_1;
      yield return random.R8_2;
      yield return random.R8_3;
      yield return random.R8_4;
      yield return random.R8_5;
      yield return random.R8_6;
      yield return random.R8_7;
    }
  }

  public unsafe byte[] ConcatGenerator(int count) {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

    byte[] result = new byte[count];

    fixed (byte* pointer = &result[0]) {
      byte* index = pointer;
      ulong random = rng.Next();

      // full rounds
      while (count >= 8) {
        *(ulong*)index = random;
        random = rng.Next();
        index += 8;
        count -= 8;
      }

      // remaining bytes
      switch (count) {
        case 0: break;
        case 1:
          *index = (byte)random;
          break;
        case 2:
          *(ushort*)index = (ushort)random;
          break;
        case 3:
          *(ushort*)index = (ushort)random;
          index[2] = (byte)(random >> 16);
          break;
        case 4:
          *(uint*)index = (uint)random;
          break;
        case 5:
          *(uint*)index = (uint)random;
          index[4] = (byte)(random >> 32);
          break;
        case 6:
          *(uint*)index = (uint)random;
          ((ushort*)index)[2] = (ushort)(random >> 32);
          break;
        case 7:
          *(uint*)index = (uint)random;
          ((ushort*)index)[2] = (ushort)(random >> 32);
          index[6] = (byte)(random >> 48);
          break;
      }
    }

    return result;
  }

  public UInt128 SplitMix128() {
    ulong random = rng.Next();
    return (UInt128)random << 64 | SplitMix64.Next(ref random);
  }

  public Vector256<ulong> SplitMix256() {
    ulong random = rng.Next();
    return Vector256.Create(random, SplitMix64.Next(ref random), SplitMix64.Next(ref random), SplitMix64.Next(ref random));
  }

  public Vector512<ulong> SplitMix512() {
    ulong random = rng.Next();
    return Vector512.Create(
      random, SplitMix64.Next(ref random), SplitMix64.Next(ref random), SplitMix64.Next(ref random),
      SplitMix64.Next(ref random), SplitMix64.Next(ref random), SplitMix64.Next(ref random), SplitMix64.Next(ref random)
    );
  }

  public UInt128 SpreadBits128(UInt128 mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    byte bitCount = _PopCount(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, 64);

    ulong random = rng.Next();
    UIn128 result = UInt128.Zero;
    for (int i = 0; i < bitCount; ++i) {
      UInt128 bit = random & 1;
      random >>= 1;

      int upperZero= BitOperations.TrailingZeroCount((ulong)(mask >> 64));
      int nextPosition = BitOperations.TrailingZeroCount((ulong)mask);
      if (nextPosition == 64)
        nextPosition += upperZero;

      result |= bit << nextPosition;
      mask &= ~(UInt128.One << nextPosition);
    }
    
    return result;
  }

  public Vector256<ulong> SpreadBits256(Vector256<ulong> mask) {
    ushort bitCount = _PopCount(mask);
    ArgumentOutOfRangeException.ThrowIfZero(bitCount, nameof(mask));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, 64);

    ulong random = rng.Next();
    Vector256<ulong> result = Vector256<ulong>.Zero;
    for (int i = 0; i < bitCount; ++i) {
      ulong bit = random & 1;
      random >>= 1;

      ulong value0 = mask.GetElement(0);
      ulong value1 = mask.GetElement(1);
      ulong value2 = mask.GetElement(2);
      ulong value3 = mask.GetElement(3);

      int zeroes0 = BitOperations.TrailingZeroCount(value0);
      int zeroes1 = BitOperations.TrailingZeroCount(value1);
      int zeroes2 = BitOperations.TrailingZeroCount(value2);
      int zeroes3 = BitOperations.TrailingZeroCount(value3);

      int nextPosition = zeroes0;
      if (zeroes0 == 64) {
        nextPosition += zeroes1;
        if (zeroes1 == 64) {
          nextPosition += zeroes2;
          if (zeroes2 == 64)
            nextPosition += zeroes3;
        }
      }

      int elementIndex = nextPosition >> 6;
      int intraElementIndex = nextPosition & 63;
      ulong element = result.GetElement(elementIndex);
      element |= bit << intraElementIndex;
      result = result.WithElement(elementIndex, element);

      mask = mask.WithElement(elementIndex, mask.GetElement(elementIndex) & ~(1UL << intraElementIndex));
    }

    return result;
  }

  public Vector512<ulong> SpreadBits512(Vector512<ulong> mask) {
    ushort bitCount = _PopCount(mask);
    ArgumentOutOfRangeException.ThrowIfZero(bitCount, nameof(mask));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, 64);

    ulong random = rng.Next();
    Vector512<ulong> result = Vector512<ulong>.Zero;
    for (int i = 0; i < bitCount; ++i) {
      ulong bit = random & 1;
      random >>= 1;

      int nextPosition = 0;
      for (int j = 0; j < 8; ++j) {
        int currentZeroes = BitOperations.TrailingZeroCount(mask.GetElement(j));
        nextPosition += currentZeroes;
        if (currentZeroes != 64)
          break;
      }

      int elementIndex = nextPosition >> 6;
      int intraElementIndex = nextPosition & 63;
      ulong element = result.GetElement(elementIndex);
      element |= bit << intraElementIndex;
      result = result.WithElement(elementIndex, element);

      mask = mask.WithElement(elementIndex, mask.GetElement(elementIndex) & ~(1UL << intraElementIndex));
    }

    return result;
  }

  public IEnumerable<byte> FeistelGenerator() {
    ulong state = rng.Next();
    ulong key = rng.Next();
    ulong counter = rng.Next();

    int counterIndex = 0;
    for (;;) {
      int roundBits = (int)(counter >> counterIndex) & 0b1111;
      counterIndex += 4;
      if (counterIndex >= 64) {
        (counter, state) = (state, counter);
        counterIndex = 0;
      }

      ++roundBits;
      for (int i = 0; i < roundBits; ++i) {
        DoFeistelRound(ref state, key);
        DoFeistelRound(ref state, key);
        (state, key) = (key, state);
      }

      SliceUnion result = new SliceUnion(state);
      yield return result.R8_0;
      yield return result.R8_1;
      yield return result.R8_2;
      yield return result.R8_3;
      yield return result.R8_4;
      yield return result.R8_5;
      yield return result.R8_6;
      yield return result.R8_7;
    }
    
    void DoFeistelRound(ref ulong plainText, ulong roundKey) {
      uint left = (uint)plainText;
      uint right = (uint)(plainText>>32);
      left ^= RoundFunction(right, roundKey);
      (left, right) = (right, left);
      plainText = left | (ulong)right << 32;
    }

    uint RoundFunction(uint right,ulong roundKey) {
      uint result=BitOperations.RotateLeft(right, 3);
      result ^= (uint)roundKey;
      result = BitOperations.RotateRight(result, 17);
      result ^= (uint)(roundKey >> 32);
      return result;
    }
  }

  public IEnumerable<byte> HashGenerator<THash>() where THash : HashAlgorithm, new() {
    using THash instance=new THash();
      return this.HashGenerator(instance);
  }

  public IEnumerable<byte> HashGenerator(HashAlgorithm instance) {
    ArgumentNullException.ThrowIfNull(instance);

    int entropyBitsNeeded = instance.HashSize;
    int entropyBytesNeeded = entropyBitsNeeded >> 3;
    
    // Generate the initial salt using RNG
    byte[] salt = this.ConcatGenerator(entropyBytesNeeded);
    
    // Initialize the counter
    byte[] counter = new byte[entropyBytesNeeded];

    for (;;) {

      // Combine the salt and counter using XOR
      byte[] plainData = salt.Zip(counter, (s, c) => (byte)(s ^ c)).ToArray();

      // Generate the hash
      byte[] hash = instance.ComputeHash(plainData);
      
      // Yield each byte of the hash as part of the random stream
      foreach (byte entry in hash)
        yield return entry;

      _Increment(counter);
    }
  }

  public IEnumerable<byte> CipherGenerator<TCipher>() where TCipher : SymmetricAlgorithm, new() {
    using TCipher instance = new TCipher();
    return this.CipherGenerator(instance);
  }

  public IEnumerable<byte> CipherGenerator(SymmetricAlgorithm instance) {
    instance.Mode = CipherMode.ECB; // CTR mode is simulated with ECB
    instance.Padding = PaddingMode.None;

    // Generate a random key and initialization vector (IV)
    byte[] key = this.ConcatGenerator(instance.KeySize >> 3);

    int blockSizeInBytes = instance.BlockSize >> 3;
    byte[] iv = this.ConcatGenerator(blockSizeInBytes);
    
    instance.Key = key;
    instance.IV = iv;

    // Initialize the counter
    byte[] counter = new byte[blockSizeInBytes];

    byte[] cipherText = new byte[blockSizeInBytes];
    using ICryptoTransform encryptor = instance.CreateEncryptor();
    for (;;) {
      
      // Encrypt the counter block
      encryptor.TransformBlock(counter, 0, blockSizeInBytes, cipherText, 0);

      // Yield each byte from the encrypted block as random output
      foreach (byte value in cipherText)
        yield return value;

      // Increment the counter
      _Increment(counter);
    }
  }

}
```

## Quasi-Random Sequences

Quasi-random (or *low-discrepancy*) sequences are a separate concept from the PRNGs above. They are not designed to look random — in fact they fail every statistical-randomness test by design — but to fill the unit interval (or hypercube) as **evenly** as possible. This makes them an excellent fit for Monte Carlo integration, where the goal is uniform coverage rather than unpredictability: a quasi-Monte Carlo estimator converges as $O(\log^d(n) / n)$ versus $O(1/\sqrt{n})$ for ordinary Monte Carlo.

They implement the same `IRandomNumberGenerator` interface here for convenience, but be aware that running statistical tests on them will (correctly) flag them as biased.

### Halton (HLT) [^56]

[^56]: [Halton](https://link.springer.com/article/10.1007/BF01386213)

This sequence was introduced by John H. Halton in 1960. The 1-dimensional base-$b$ Halton sequence is the [van der Corput sequence](https://en.wikipedia.org/wiki/Van_der_Corput_sequence) in base $b$. If $i$ has the base-$b$ digit expansion $i = \sum_{k=0}^{K} d_k b^k$, then the $i$-th Halton point is

$$
\varphi_b(i) = \sum_{k=0}^{K} d_k \, b^{-(k+1)} \in [0, 1)
$$

— writing $i$ in base $b$, reversing the digits and placing them after the radix point. For base 2 this reduces to bit-reversal of the counter, so the implementation in this repository emits $X_i = \mathrm{BitReverse}(i)$ as a 64-bit integer.

```cs
class Halton : IRandomNumberGenerator {
  private ulong _counter;

  public void Seed(ulong seed) => this._counter = seed;

  // For base 2 only: bit-reverse the counter to produce the van der Corput sequence.
  public ulong Next() => BitReverse(++this._counter);
}
```

### Sobol' (SOB) [^57]

[^57]: [Sobol'](https://www.sciencedirect.com/science/article/pii/0041555367901449)

Introduced by Ilya M. Sobol' in 1967, Sobol' sequences improve on Halton's even coverage in higher dimensions and are the most widely-used quasi-random sequence in finance and computational physics. The construction uses precomputed *direction numbers* $V_0, V_1, \ldots, V_{w-1}$ (one per bit position) plus the Gray-code trick: the $i$-th and $(i+1)$-th points differ in exactly one bit, identified by the trailing-zero count of $i+1$. Letting $S$ be the running state and $\mathrm{ctz}(\cdot)$ the count-trailing-zeroes operation:

$$
\begin{aligned}
c &= \mathrm{ctz}(i + 1) \\
S &\leftarrow S \oplus V_c \\
X_i &= S
\end{aligned}
$$

A single XOR per call — the next point is produced from the previous one with no multiplication or modular arithmetic. In one dimension the direction numbers are simply the powers of two ($V_k = 2^{w-1-k}$ for a $w$-bit output), which is equivalent to the base-2 Halton sequence enumerated in Gray-code order.

```cs
class Sobol : IRandomNumberGenerator {
  private readonly ulong[] _direction = new ulong[64];
  private ulong _state, _counter;

  public Sobol() {
    // 1-D direction numbers are just bit-position masks.
    for (var i = 0; i < 64; ++i) this._direction[i] = 1UL << (63 - i);
  }

  public void Seed(ulong seed) {
    this._state = 0; this._counter = 0;
    for (var i = 0UL; i < seed; ++i) this.Next();
  }

  public ulong Next() {
    var c = BitOperations.TrailingZeroCount(++this._counter);
    this._state ^= this._direction[c];
    return this._state;
  }
}
```

Multi-dimensional Sobol' (used in practice) requires per-dimension direction-number tables from Joe-Kuo or similar — out of scope here.

## NURNG-Algorithms

The previous sections covered *uniform* random numbers — every output equally likely across a fixed range. Many applications need numbers drawn from a non-uniform distribution: Gaussian noise for a signal-processing simulation, Poisson-distributed arrival times for a queueing model, Pareto-distributed wait times for a heavy-tailed network analysis. The NURNG (Non-Uniform Random Number Generator) algorithms below build on a uniform RNG to produce samples from these and other distributions.

**Distribution catalog at a glance:**

| Distribution                                      | Type       | Domain                     | Parameters                 | Generator method           |
| ------------------------------------------------- | ---------- | -------------------------- | -------------------------- | -------------------------- |
| [Exponential](#inverse-transform-sampling-its-40) | Continuous | $[0, \infty)$              | rate $\lambda$             | Inverse-CDF                |
| [Standard Normal](#box-muller-method-bm-41)       | Continuous | $(-\infty, \infty)$        | μ=0, σ=1                   | Box-Muller polar transform |
| [Standard Normal](#marsaglia-polar-method-mp-42)  | Continuous | $(-\infty, \infty)$        | μ=0, σ=1                   | Marsaglia rejection        |
| [Standard Normal](#ziggurat-zig-43)               | Continuous | $(-\infty, \infty)$        | μ=0, σ=1                   | Ziggurat layered rejection |
| [Poisson](#poisson-psn-61)                        | Discrete   | $\{0, 1, 2, \ldots\}$      | rate $\lambda$             | Knuth / Atkinson PA        |
| [Gamma](#gamma-gam-62)                            | Continuous | $[0, \infty)$              | shape $k$, scale $\theta$  | Marsaglia-Tsang squeeze    |
| [Beta](#beta-bet-63)                              | Continuous | $(0, 1)$                   | shape $\alpha$, $\beta$    | Ratio of two Gammas        |
| [Bernoulli](#bernoulli-brn-64)                    | Discrete   | $\{0, 1\}$                 | probability $p$            | Direct                     |
| [Binomial](#binomial-bin-65)                      | Discrete   | $\{0, 1, \ldots, n\}$      | trials $n$, prob $p$       | Sum of Bernoullis          |
| [Geometric](#geometric-geo-66)                    | Discrete   | $\{0, 1, 2, \ldots\}$      | probability $p$            | Inverse-CDF                |
| [Chi-Squared](#chi-squared-chi-67)                | Continuous | $[0, \infty)$              | df $k$                     | Gamma($k/2$, 2)            |
| [Cauchy](#cauchy-ccy-68)                          | Continuous | $(-\infty, \infty)$        | location, scale            | Inverse-CDF (tan)          |
| [Log-normal](#log-normal-lnm-69)                  | Continuous | $(0, \infty)$              | μ, σ                       | exp of normal              |
| [Weibull](#weibull-wbl-70)                        | Continuous | $[0, \infty)$              | shape $k$, scale $\lambda$ | Inverse-CDF                |
| [Triangular](#triangular-tri-71)                  | Continuous | $[\text{min}, \text{max}]$ | min, mode, max             | Inverse-CDF (two branches) |
| [Pareto](#pareto-par-72)                          | Continuous | $[\text{scale}, \infty)$   | shape, scale               | Inverse-CDF (power)        |

For the upcoming algorithms we'll utilize a different interface than above to generate floating-point 64-bit values with specific distribution properties:

```c#
interface IDoubleRandomNumberGenerator {

  double Next() {
    var (result, _) = this.NextPair();
    return result;
  }

  (double, double) NextPair() => (this.Next(), this.Next());

}
```

### Inverse Transform Sampling (ITS) [^40]

[^40]: [ITS](https://brilliant.org/wiki/inverse-transform-sampling/)

Here’s how it works:

1. **Generate a Uniform Random Number**:
   Start by generating a random number, $U$, uniformly distributed between 0 and 1 using a simple RNG. This $U$ will be the foundation of your transformation.

2. **Compute the Inverse CDF**:
   The next step is to compute the inverse of the cumulative distribution function (CDF) of your target distribution. The CDF, denoted as $f(x)$, represents the probability that a random variable $X$ takes a value less than or equal to $x$. The inverse CDF, $f^{-1}(u)$, will give you the value $x$ such that the probability of $X$ being less than or equal to $x$ is exactly $u$. Mathematically, this can be written as:

   $$X = f^{-1}(U)$$

   Here, $x$ is the random sample drawn from the desired distribution.

3. **Output the Result**:
   The value $x$ obtained from the inverse CDF is your random sample that follows the target probability distribution.

Let’s consider an example where you need to sample from an exponential distribution, which is commonly used in survival analysis, queueing theory, and many other fields. The exponential distribution with rate $\lambda$ has the CDF:

$$u = f(x) = 1 - e^{-\lambda x}$$

To apply inverse transform sampling, you first solve for $x$ in terms of $u$:

$$x = -\frac{\ln(1 - u)}{\lambda}$$

This equation allows you to convert a uniform random number $u$ into a sample $x$ from the exponential distribution.

```cs
double Exponential(double lambda, IRandomNumberGenerator rng) {
  double u = rng.NextDouble();
  return -Math.Log(1 - u) / lambda;
}
```

![Exponential distribution: tallest bar at 0, decaying smoothly to near-zero by x=6](Images/dist_exponential.png)
*200,000 samples with λ=1. Look for the strictly-decreasing shape with no peak away from zero — the exponential's "memoryless" property is what makes the curve a single monotone decay.*

### Box-Muller Method (BM) [^41]

[^41]: [BM](https://www.researchgate.net/publication/264324131_Box-Muller_transformation)

This is another popular algorithm used to generate pairs of independent, normally distributed random variables (also known as Gaussian variables) from uniformly distributed random numbers. The method is named after George E. P. Box and Mervin E. Muller, who introduced it in 1958. It is straightforward to implement and produces two normally distributed values per iteration, making it efficient for simulations and other applications requiring Gaussian distributions.

The BM operates in two main steps:

* **Generate Two Uniform Random Numbers**: The method begins by generating two independent random numbers, $x$ and $y$, from a uniform distribution over the interval $(0, 1]$.

* **Transform to Gaussian Distribution**: The uniform random numbers are then transformed into a pair of independent, normally distributed random variables $z_0$ and $z_1$ using the following formulas:

  $$r = \sqrt{-2 \ln(x)}$$

  $$\theta = 2 \pi y$$

  $$z_0 = r \cdot \cos(\theta)$$

  $$z_1 = r \cdot \sin(\theta)$$

  These transformations are derived from the properties of the normal distribution and trigonometric identities.

  Intuitively, this is a polar-coordinate construction: $x$ chooses a *radius* via $r = \sqrt{-2 \ln x}$ (so that the radius distribution matches the Rayleigh distribution that drops out of two independent Gaussians), and $y$ chooses an *angle* $\theta$ uniformly around the circle. Projecting that single point onto the $X$ and $Y$ axes via cosine and sine yields two independent Gaussian samples in one shot.

  ![Standard Normal: symmetric bell curve centered at 0, tails fading to negligible counts by ±3](Images/dist_normal.png)
  *200,000 samples binned across [-4, 4]. Look for the symmetric bell shape peaking at 0 and the rapid fall-off past about ±3 — these are the visual fingerprints of a clean Gaussian.*

  ```mermaid
  flowchart LR
    U1["x ~ Uniform(0,1]"] --> Radius["r = √(-2 ln x)"]
    U2["y ~ Uniform(0,1]"] --> Angle["θ = 2πy (random angle 0..2π)"]
    Radius --> Project
    Angle --> Project
    Project["Point (r, θ) in polar coords"]
    Project --> Z0["z0 = r·cos θ  (one Gaussian sample)"]
    Project --> Z1["z1 = r·sin θ  (a second, independent Gaussian sample)"]
  ```


```cs
(double, double) Next() {
  double x = 2 * generator.NextDouble() - 1;
  double y = 2 * generator.NextDouble() - 1;
 
  double r = Math.Sqrt(-2.0 * Math.Log(x));
  double theta = 2.0 * Math.PI * y;

  double z0 = r * Math.Cos(theta);
  double z1 = r * Math.Sin(theta);

  return (z0, z1);
}
```

### Marsaglia Polar Method (MP) [^42]

[^42]: [MP](https://www.jstor.org/stable/2027592)

This is a widely used algorithm for generating pairs of Gaussian variables from a uniform RNG. This method is particularly efficient because it generates two normally distributed values simultaneously, making it faster than some other methods like the [BM](#box-muller-method-bm) transform.

The MP relies on the fact that a pair of independent, uniformly distributed variables can be transformed into a pair of independent, normally distributed variables. The method involves the following steps:

* **Generate Two Uniform Random Numbers**: Two random variables $x$ and $y$ are drawn from a uniform distribution between $-1$ and $+1$. These are mapped from an underlying uniform RNG that typically provides values in the range $[0, 1)$.

* **Calculate their Sum of Squares**: The sum of squares $s = x^2 + y^2$ is computed. This value represents the squared distance of the point $(x, y)$ from the origin in a 2D plane.

* **Accept or Reject the Pair**: If $s$ is outside the interval $(0, 1)$, the pair is rejected, and the process repeats. This ensures the points lie within the unit circle, which is essential for the next step to correctly produce normally distributed outputs.

* **Transform to Gaussian Distribution**: Once a valid pair is found (i.e., when $s$ is within the unit circle), the method uses the formula:

   $$\text{multiplier} = \sqrt{\frac{-2 \cdot \ln(s)}{s}}$$

  to transform the uniform random variables $x$ and $y$ into independent, normally distributed random variables.

* **Return Two Normally Distributed Variables**: The transformed values $x \times \text{multiplier}$ and $y \times \text{multiplier}$ are the resulting Gaussian variables.

```cs
(double, double) Next() {
  while (true) {
    double x = 2 * generator.NextDouble() - 1;
    double y = 2 * generator.NextDouble() - 1;
    double s = x * x + y * y;

    if (s is <= 0 or >= 1)
      continue;

    double multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
    return (x * multiplier, y * multiplier);
  }
}
```

### Ziggurat (ZIG) [^43]

[^43]: [ZIG](https://www.jstatsoft.org/article/view/v005i08)

This is an efficient algorithm for generating random numbers from a variety of probability distributions, most notably the normal (Gaussian) distribution. It is particularly well-suited for high-performance applications where speed is critical, such as simulations and cryptographic systems.

The ZIG generates random numbers by partitioning the target distribution into multiple layers, resembling a ziggurat (a terraced structure from ancient Mesopotamia). Each layer is either a rectangle or a tail region, and the method efficiently samples from these regions.

```mermaid
flowchart TB
  subgraph "Half of the bell curve, sliced into rectangular layers"
    direction TB
    L7["Layer 7  ████"]
    L6["Layer 6  ███████"]
    L5["Layer 5  ████████"]
    L4["Layer 4  █████████"]
    L3["Layer 3  ██████████"]
    L2["Layer 2  ███████████"]
    L1["Layer 1  ████████████"]
    L0["Layer 0  ████████████ + ~~~ infinite tail ~~~"]
    L7 --- L6 --- L5 --- L4 --- L3 --- L2 --- L1 --- L0
  end
  Pick["Pick a layer uniformly at random"] --> Sample
  Sample["Pick (x, y) uniformly inside that rectangle"] --> Check{"Is (x, y) below f(x)?"}
  Check -->|yes ~99%| Accept["Return x"]
  Check -->|no, in 'wedge'| Compute["Compute f(x); accept if y < f(x)"]
  L0 -.->|"layer 0 only"| Tail["Use exponential-rejection fallback for the tail"]
```

The key steps in the Ziggurat Method are:

* **Precompute Layers**: The distribution is divided into a series of layers, each represented by a rectangle. These layers cover the bulk of the distribution, with the *bottom* layer (the one extending out to infinity, since it is not bounded above by the next layer) handling the distribution's tails via a separate fallback sampler.

* **Uniform Sampling**: A random rectangle is selected uniformly from the precomputed layers. Within this rectangle, a random point is chosen uniformly.

* **Acceptance or Rejection**: If the point falls within the desired distribution, it is accepted as a valid sample. If not, the algorithm resorts to a fallback method, such as the Box-Muller transform or a direct sampling from the tail region, to generate the sample.

* **Efficiency**: The method is efficient because most samples fall within the rectangles, and the expensive fallback step is needed only for a small fraction of cases.

```cs
class Ziggurat(ArbitraryNumberGenerator generator) {

  private const int NUM_LAYERS = 128;
  private const double R = 3.442619855899;
  private const double V = 9.91256303526217e-3;
  private const double R_INVERSE = 1 / R;

  private static readonly double[] layerWidths = new double[NUM_LAYERS];
  private static readonly double[] layerHeights = new double[NUM_LAYERS];

  static Ziggurat() {

    // Precompute the widths and heights of the layers
    double f = Math.Exp(-0.5 * R * R);
    layerWidths[0] = V / f;
    layerWidths[1] = R;

    double lastLayerWidth = R;
    for (int i = 2; i < NUM_LAYERS; ++i) {
      lastLayerWidth = layerWidths[i] = Math.Sqrt(-2 * Math.Log(V / lastLayerWidth + f));
      f = Math.Exp(-0.5 * lastLayerWidth * lastLayerWidth);
    }

    layerHeights[NUM_LAYERS - 1] = 0;
    for (int i = 0; i < NUM_LAYERS - 1; ++i)
      layerHeights[i] = layerWidths[i + 1] / layerWidths[i];

  }

  public double Next() {
    for (;;) {
      int i = (int)generator.ModuloRejectionSampling(NUM_LAYERS);
      double u = 2 * generator.NextDouble() - 1;

      /* first try the rectangular boxes */
      double layerWidth = layerWidths[i];
      double x = u * layerWidth;
      if (Math.Abs(u) < layerHeights[i])
        return x;

      /* bottom box: sample from the tail */
      if (i == 0)
        return SampleTail(u < 0);

      /* is this a sample from the wedges? */
      double xSqr = x * x;
      double nextLayerWidth = i == NUM_LAYERS - 1 ? 0 : layerWidths[i + 1];

      double f0 = Math.Exp(-0.5 * (layerWidth * layerWidth - xSqr));
      double f1 = Math.Exp(-0.5 * (nextLayerWidth * nextLayerWidth - xSqr));
      if (f1 + generator.NextDouble() * (f0 - f1) < 1.0)
        return x;
    }

    double SampleTail(bool isNegative) {
      for (;;) {
        double x = -Math.Log(generator.NextDouble()) * R_INVERSE;
        double y = -Math.Log(generator.NextDouble());
        if (y + y < x * x)
          continue;

        double result = R + x;
        if (isNegative)
          result = -result;

        return result;
      }
    }
  }

}
```

### Poisson (PSN) [^61]

[^61]: [Poisson](https://en.wikipedia.org/wiki/Poisson_distribution)

This generates integer samples from the **Poisson distribution** — the discrete distribution that models the number of independent events occurring in a fixed interval, given an average rate $\lambda$. Used everywhere queues, arrivals, decays or "rare events" come up: server-request modelling, particle counts, network packet rates.

The implementation uses two branches:

* **Small λ (default branch, λ < 30):** Knuth's algorithm — multiply successive Uniform(0,1) draws until the product drops below $e^{-\lambda}$; the count of draws (minus one) is the sample. Per-sample cost is $O(\lambda)$.
* **Large λ:** Atkinson's PA method — a rejection sampler whose expected cost per output stays bounded as λ grows, by drawing from a Cauchy-like proposal and accepting with the appropriate ratio.

```cs
class Poisson {
  public int Next() => _lambda < 30 ? KnuthSmall() : AtkinsonLarge();

  private int KnuthSmall() {
    var L = Math.Exp(-_lambda);
    var k = 0; var p = 1.0;
    do { ++k; p *= _generator.NextDouble(); }
    while (p > L);
    return k - 1;
  }
}
```

![Poisson(λ=4): bars rise to peak at k=3 or 4, then decay to negligible by k=12](Images/dist_poisson.png)
*200,000 samples with λ=4. Look for the discrete bars peaking near the rate λ and tailing off both ways — mean and variance both equal λ for a Poisson, so the spread should match the peak location.*

### Gamma (GAM) [^62]

[^62]: [Gamma](https://en.wikipedia.org/wiki/Gamma_distribution)

This generates samples from the **Gamma distribution** — a continuous distribution on $(0, \infty)$ parameterised by *shape* $k$ and *scale* $\theta$, with mean $k\theta$. It generalises the exponential distribution (which is Gamma with $k = 1$) and underpins many other distributions, including chi-squared and Erlang.

We use **Marsaglia and Tsang's "squeeze method" (2000)**, which is fast and provides a uniform algorithm for any $k \geq 1$. For $k < 1$ we sample at shape $k+1$ then apply Stuart's correction $G(k) = G(k+1) \cdot U^{1/k}$.

```cs
class Gamma {
  // For shape >= 1: Marsaglia-Tsang squeeze.
  public double Next() {
    double d = _effectiveShape - 1.0 / 3.0;
    double c = 1.0 / Math.Sqrt(9.0 * d);
    for (;;) {
      double x, v;
      do { x = StandardNormal(); v = 1.0 + c * x; } while (v <= 0);
      v = v * v * v;
      double u = _generator.NextDouble();
      if (u < 1.0 - 0.0331 * x * x * x * x) return d * v * _scale;
      if (Math.Log(u) < 0.5 * x * x + d * (1.0 - v + Math.Log(v))) return d * v * _scale;
    }
  }
}
```

![Gamma(2, 2): right-skewed bell with peak around x=2 and a long tail extending past x=10](Images/dist_gamma.png)
*200,000 samples with shape=2, scale=2 (mean=4). Look for the right-skewed bell rising from zero, peaking around x=(shape−1)·scale, and tailing off — Gamma generalises the exponential, so shape=1 would give pure decay and large shapes approach a normal.*

### Beta (BET) [^63]

[^63]: [Beta](https://en.wikipedia.org/wiki/Beta_distribution)

This generates samples from the **Beta distribution** — a continuous distribution on $(0, 1)$ parameterised by two shape values $\alpha$ and $\beta$, with mean $\alpha / (\alpha + \beta)$. Heavily used in Bayesian statistics (as a conjugate prior for binomial likelihoods), A/B testing, and machine learning (especially as a prior on probabilities).

The standard construction is to draw two independent Gamma variates and take their normalised ratio:

$$X \sim \text{Gamma}(\alpha, 1), \quad Y \sim \text{Gamma}(\beta, 1), \quad B = \frac{X}{X + Y} \sim \text{Beta}(\alpha, \beta)$$

```cs
class Beta {
  public double Next() {
    double x = _x.Next();   // Gamma(alpha, 1)
    double y = _y.Next();   // Gamma(beta, 1)
    return x / (x + y);
  }
}
```

![Beta(2, 5): asymmetric peak around 0.2-0.3, falling to zero at both 0 and 1](Images/dist_beta.png)
*200,000 samples with α=2, β=5 (mean ≈ 0.286). Look for the bounded shape on [0, 1] peaking at (α−1)/(α+β−2) — Beta(α, β) is symmetric if α = β, skewed toward 0 when α < β, and toward 1 when α > β.*

### Bernoulli (BRN) [^64]

[^64]: [Bernoulli](https://en.wikipedia.org/wiki/Bernoulli_distribution)

This is the simplest non-uniform distribution: a single trial that returns `true` with probability $p$ and `false` with probability $1-p$. Every other discrete distribution can be built from a sequence of Bernoulli trials, which is why it appears here as a separate class even though one line of arithmetic suffices.

```cs
class Bernoulli {
  public bool Next() => _generator.NextDouble() < _probability;
}
```

### Binomial (BIN) [^65]

[^65]: [Binomial](https://en.wikipedia.org/wiki/Binomial_distribution)

This counts the number of successes in $n$ independent Bernoulli trials. Mean $= np$, variance $= np(1-p)$. Ubiquitous in A/B testing, quality control ("k defective items out of n inspected"), and any "k of n" simulation.

The implementation simulates the $n$ Bernoulli draws directly. For very large $n \cdot \min(p, 1-p)$ a more sophisticated algorithm (BTRS, Devroye's rejection method) would be preferable; that optimisation is left as a future addition.

```cs
class Binomial {
  public int Next() {
    int successes = 0;
    for (int i = 0; i < _trials; ++i)
      if (_generator.NextDouble() < _probability) ++successes;
    return successes;
  }
}
```

![Binomial(20, 0.4): bars peaking around k=8 with binomial-shape spread, falling to near-zero by k=15](Images/dist_binomial.png)
*200,000 samples with n=20 trials, p=0.4. Look for the bars peaking near the expected value np=8; the shape becomes more symmetric and Gaussian-like as n grows.*

### Geometric (GEO) [^66]

[^66]: [Geometric](https://en.wikipedia.org/wiki/Geometric_distribution)

This gives the number of *failures* before the first success in a sequence of Bernoulli$(p)$ trials. Mean $= (1-p)/p$. Common in queueing theory ("how many empty-handed lookups until a cache hit?"), reliability analysis, and any "wait until the first event" simulation.

Generated in constant time via the inverse-CDF method:

$$x = \left\lfloor \frac{\ln U}{\ln(1-p)} \right\rfloor$$

```cs
class Geometric {
  public int Next() {
    double u = _generator.NextDouble();
    return (int)Math.Floor(Math.Log(u) / Math.Log(1.0 - _probability));
  }
}
```

![Geometric(0.3): tallest bar at k=0, each subsequent bar (1-p) times smaller](Images/dist_geometric.png)
*200,000 samples with p=0.3. Look for the strictly decreasing bars — each bar is (1−p) times the previous one, the discrete analogue of exponential decay.*

### Chi-Squared (CHI) [^67]

[^67]: [Chi-Squared](https://en.wikipedia.org/wiki/Chi-squared_distribution)

The chi-squared distribution with $k$ degrees of freedom is the distribution of the sum of $k$ squared independent standard normals. It is the workhorse of hypothesis testing — goodness-of-fit, contingency tables, variance tests. Mathematically it is identical to $\text{Gamma}(k/2, 2)$, so the implementation is a thin wrapper over [Gamma](#gamma-gam-62):

```cs
class ChiSquared {
  public ChiSquared(ArbitraryNumberGenerator gen, double df)
    => _gamma = new Gamma(gen, shape: df / 2.0, scale: 2.0);

  public double Next() => _gamma.Next();
}
```

![Chi-Squared(k=4): right-skewed peak around x=2, long tail extending to x≈14](Images/dist_chisquared.png)
*200,000 samples with df=4. Look for the shape peaking near k−2 (here 2), with mean equal to k (here 4); the shape becomes more symmetric and Gaussian-like as df grows.*

### Cauchy (CCY) [^68]

[^68]: [Cauchy](https://en.wikipedia.org/wiki/Cauchy_distribution)

The Cauchy (also called Lorentz) distribution is a famously badly-behaved heavy-tailed distribution: its mean and variance are *undefined* because the tails decay only as $1/x^2$, so the sample mean does not converge to anything. It is the canonical example of a stable distribution other than the normal, and shows up in physics (resonance peaks) and robust-statistics counterexamples.

Generated via inverse-CDF:

$$x = \text{location} + \text{scale} \cdot \tan\left(\pi \left(U - \tfrac{1}{2}\right)\right)$$

```cs
class Cauchy {
  public double Next() {
    double u = _generator.NextDouble();
    return _location + _scale * Math.Tan(Math.PI * (u - 0.5));
  }
}
```

![Cauchy(0, 1): tall narrow peak at 0 with visibly heavy tails that decay only as 1/x²](Images/dist_cauchy.png)
*200,000 samples with location=0, scale=1. Look for the much taller central peak compared to a Gaussian and the visibly *fatter* tails — these heavy tails are why the Cauchy distribution has no defined mean or variance: extreme outliers happen far more often than the bell-shape suggests.*

### Log-normal (LNM) [^69]

[^69]: [Log-normal](https://en.wikipedia.org/wiki/Log-normal_distribution)

A continuous distribution whose *logarithm* is normally distributed. Arises naturally wherever multiplicative effects compound: stock prices (geometric Brownian motion), particle sizes in milling, body weights in a population, file sizes on a server.

Generated by sampling a standard normal $Z$ and exponentiating:

$$X = e^{\mu + \sigma Z}$$

```cs
class Lognormal {
  public double Next() => Math.Exp(_mu + _sigma * StandardNormal(_generator));
}
```

![Lognormal(0, 0.5): right-skewed shape with peak around x=0.8 and long upper tail](Images/dist_lognormal.png)
*200,000 samples with μ=0, σ=0.5. Look for the strictly positive support (no values below 0) and the asymmetric shape with a long right tail — the median equals exp(μ) but the mean is shifted right by σ²/2.*

### Weibull (WBL) [^70]

[^70]: [Weibull](https://en.wikipedia.org/wiki/Weibull_distribution)

A continuous distribution on $[0, \infty)$ heavily used in reliability engineering and survival analysis. The shape parameter $k$ encodes the *failure-rate trend*:

* $k < 1$: failure rate decreasing with time (the "infant mortality" or burn-in phase).
* $k = 1$: constant failure rate (reduces to exponential).
* $k > 1$: failure rate increasing with time (wear-out).

Generated via inverse-CDF:

$$x = \lambda \cdot \left(-\ln(1 - U)\right)^{1/k}$$

```cs
class Weibull {
  public double Next() {
    double u = _generator.NextDouble();
    return _scale * Math.Pow(-Math.Log(1.0 - u), 1.0 / _shape);
  }
}
```

The three shapes side-by-side show the failure-mode interpretation:

| shape=0.5 (infant mortality)                                                                                                                                                | shape=1 (exponential / constant rate)                                                                                                         | shape=2.5 (wear-out)                                                                                                                      |
| --------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| ![Weibull shape=0.5: bars decay rapidly from a peak at zero — most failures happen immediately, survivors stick around](Images/dist_weibull_0_5.png)                        | ![Weibull shape=1: clean exponential decay — identical to an exponential distribution](Images/dist_weibull_1.png)                             | ![Weibull shape=2.5: bell-like shape peaking around x=1 — items become more likely to fail with age](Images/dist_weibull_2_5.png)         |
| *Failure rate decreases over time. Look for the very tall bar at small x, then sparse populated bars to the right — items that survive the burn-in tend to keep surviving.* | *Failure rate is constant. Look for the strict exponential decay — this is the memoryless regime, identical to the exponential distribution.* | *Failure rate increases over time. Look for the bell-like shape with a definite peak — items become **more** likely to fail as they age.* |

### Triangular (TRI) [^71]

[^71]: [Triangular](https://en.wikipedia.org/wiki/Triangular_distribution)

A continuous distribution on $[\text{min}, \text{max}]$ with peak (mode) at a configurable point inside that range. Popular in PERT analysis and risk modeling where the only available information is an *optimistic*, *most-likely* and *pessimistic* estimate of some quantity.

The inverse-CDF has two branches — rising on $[\text{min}, \text{mode}]$, falling on $[\text{mode}, \text{max}]$ — selected by a single comparison against the threshold $(\text{mode} - \text{min}) / (\text{max} - \text{min})$.

```cs
class Triangular {
  public double Next() {
    double u = _generator.NextDouble();
    double span = _max - _min;
    return u < _threshold
      ? _min + Math.Sqrt(u * span * (_mode - _min))
      : _max - Math.Sqrt((1.0 - u) * span * (_max - _mode));
  }
}
```

![Triangular(min=0, mode=2, max=10): rising bars from 0 to a peak at x=2, then linear decline to 10](Images/dist_triangular.png)
*200,000 samples with min=0, mode=2, max=10. Look for the literal triangular shape: a straight rising line on the left of the mode, a straight falling line on the right. This makes Triangular the obvious choice for "best guess" / PERT analysis where only three numbers are known.*

### Pareto (PAR) [^72]

[^72]: [Pareto](https://en.wikipedia.org/wiki/Pareto_distribution)

The Pareto Type-I distribution is the classic power-law distribution: $P(X > x) = (\text{scale} / x)^{\text{shape}}$ for $x \geq \text{scale}$. It is the source of the "80/20 rule" (a small fraction of inputs account for most of the output) and models income, city sizes, file sizes, network-flow lengths, and natural-language word frequencies (Zipf's law is a discrete cousin).

Generated via inverse-CDF:

$$x = \text{scale} / U^{1/\text{shape}}$$

```cs
class Pareto {
  public double Next() {
    double u = _generator.NextDouble();
    return _scale / Math.Pow(u, 1.0 / _shape);
  }
}
```

![Pareto(shape=2, scale=1): tallest bars near x=1 (the minimum), power-law decay over the visible range](Images/dist_pareto.png)
*200,000 samples with shape=2, scale=1 (note all samples are ≥ scale=1). Look for the steep decay from the left edge — the area under the right portion of the curve is non-negligible even though it looks tiny, which is what gives rise to the "80/20 rule" in real data.*

### Discrete Uniform (DUN) [^73]

[^73]: [DUN](https://en.wikipedia.org/wiki/Discrete_uniform_distribution)

The simplest discrete distribution: each integer in $[\text{min}, \text{max}]$ (inclusive) is equally likely. The canonical "fair die roll" generator. Built on `ModuloRejectionSampling` so that no value is biased even when the range is not a divisor of $2^{64}$.

```cs
class DiscreteUniform {
  public long Next() => _min + (long)_generator.ModuloRejectionSampling(_range);
}
```

### Categorical (CAT) [^74]

[^74]: [Categorical](https://en.wikipedia.org/wiki/Categorical_distribution)

Weighted choice among a finite set of items, each with its own probability — the discrete generalisation of Bernoulli to $n$ outcomes. Used for loot tables in games, mixture-model component selection, weighted sampling for A/B variants, and any "pick one with these probabilities" scenario.

Internally the constructor builds a cumulative-distribution array once, then each sample is an $O(\log n)$ binary search:

```cs
class Categorical<T> {
  public T Next() {
    var u = _generator.NextDouble();
    var idx = Array.BinarySearch(_cumulative, u);
    if (idx < 0) idx = ~idx;
    return _items[Math.Min(idx, _items.Length - 1)];
  }
}
```

### Hypergeometric (HYP) [^75]

[^75]: [Hypergeometric](https://en.wikipedia.org/wiki/Hypergeometric_distribution)

The discrete distribution behind the **urn problem**: how many successes do you observe when you draw $n$ items *without replacement* from a population of $N$ items, $K$ of which are successes? Unlike the binomial (which assumes replacement), the hypergeometric correctly accounts for the depleting population.

Used in survey sampling, quality control ("how many defective items in this batch sample?") and card-game probability calculations. Implementation simulates the draws directly:

```cs
class Hypergeometric {
  public int Next() {
    int remaining = _populationSize, successesLeft = _successCount, observed = 0;
    for (int i = 0; i < _draws; ++i) {
      double p = (double)successesLeft / remaining;
      if (_generator.NextDouble() < p) { --successesLeft; ++observed; }
      --remaining;
    }
    return observed;
  }
}
```

### Student's t (TDF) [^76]

[^76]: [Student's t](https://en.wikipedia.org/wiki/Student%27s_t-distribution)

Symmetric around zero, heavier-tailed than the normal for small df, and approaches the standard normal as $\text{df} \to \infty$. The workhorse distribution for small-sample hypothesis testing (the *t*-test) and for robust regression as a heavy-tailed error model.

Constructed from a standard normal $Z$ and a chi-squared $V$ via:

$$T = \frac{Z}{\sqrt{V / \text{df}}}$$

```cs
class StudentT {
  public double Next() {
    double z = StandardNormal();
    double v = _chiSquared.Next();
    return z / Math.Sqrt(v / _df);
  }
}
```

### Negative Binomial (NBN) [^77]

[^77]: [Negative Binomial](https://en.wikipedia.org/wiki/Negative_binomial_distribution)

Generalises the geometric distribution: instead of counting failures before the *first* success, it counts failures before the *k*-th success. Mean $= k(1-p)/p$. Used in queueing models, overdispersed count data (real-world counts often have variance > mean, which the Poisson cannot model but Negative Binomial can), and "wait for $k$ events" simulations.

```cs
class NegativeBinomial {
  public int Next() {
    int successes = 0, failures = 0;
    while (successes < _successesRequired) {
      if (_generator.NextDouble() < _probability) ++successes;
      else ++failures;
    }
    return failures;
  }
}
```

# Points of Interest

## Comparative Tests

There nearly infinite methods to compare algorithms against each other so I have to decide which I want to include even though the table won't ever be complete.

**One picture from each family — what does randomness actually look like?**

The grid below shows the 256×256 randogram (each pixel encodes the (low-8-bits, low-8-bits) pair from two *consecutive outputs* of the generator) for one representative generator from each major family. The plotting convention matches the one used in Melissa O'Neill's PCG analysis ["Visualizing the heart of some PRNGs"](https://www.pcg-random.org/posts/visualizing-the-heart-of-some-prngs.html): 65,536 samples (so the expected visit count per cell is exactly 1), and intensity *halves* on each occurrence — 0 visits = white, 1 = 50% grey, 2 = 25% grey, etc. A good RNG produces a uniform random-looking speckle pattern; a broken one shows immediately visible regular structure.

Reading left-to-right, top-to-bottom gives a guided tour of the field: from broken historical designs (Middle Square, basic LCG), through classical good PRNGs (Mersenne Twister, Xoshiro), modern counter-based and Romu-style fast generators, cryptographic constructions, and finally the quasi-random and cellular-automaton paradigms.

| Family                   | Generator                     | Randogram                                             | What to look for                                                                                                                                                                                                                                                                                                                                                                                          |
| ------------------------ | ----------------------------- | ----------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Historical broken**    | Middle Square                 | ![](Images/MiddleSquare_randogram.png)                | Nearly blank with a single dot near (0, 0) — the generator collapsed to zero, so every consecutive pair is (0, 0)                                                                                                                                                                                                                                                                                         |
| **Basic LCG**            | Linear Congruential Generator | ![](Images/LinearCongruentialGenerator_randogram.png) | **Marsaglia lattice** — the famous regular dot grid that classical LCGs produce when consecutive outputs are plotted as (x, y). This is exactly the failure mode that O'Neill's PCG paper highlights.                                                                                                                                                                                                     |
| **LCG (weak low bits)**  | Multiplicative LCG            | ![](Images/MultiplicativeLCG_randogram.png)           | Even sparser lattice — MLCG with default parameters concentrates consecutive pairs onto a tiny subset of cells                                                                                                                                                                                                                                                                                            |
| **Mersenne Twister**     | MT19937                       | ![](Images/MersenneTwister_randogram.png)             | Uniform speckle — the classical baseline of "looks random"                                                                                                                                                                                                                                                                                                                                                |
| **XorShift / Xoshiro**   | Xoshiro256**                  | ![](Images/Xoshiro256SS_randogram.png)                | Uniform speckle — modern small-state generator that matches MT                                                                                                                                                                                                                                                                                                                                            |
| **PCG**                  | PCG XSL-RR                    | ![](Images/PermutedCongruentialXslRr_randogram.png)   | Uniform speckle — the output permutation hides the LCG core's lattice, the whole point of the PCG design                                                                                                                                                                                                                                                                                                  |
| **Counter-based**        | Philox 2x64-10                | ![](Images/Philox_randogram.png)                      | Uniform speckle — parallel-friendly bijective design                                                                                                                                                                                                                                                                                                                                                      |
| **Romu family**          | RomuTrio                      | ![](Images/RomuTrio_randogram.png)                    | Uniform speckle — rotation+multiply with a non-linear cycle structure                                                                                                                                                                                                                                                                                                                                     |
| **128-bit MCG**          | Lehmer128                     | ![](Images/Lehmer128_randogram.png)                   | Uniform speckle — pure multiplication on 128-bit state                                                                                                                                                                                                                                                                                                                                                    |
| **Hash-based**           | WyRand                        | ![](Images/WyRand_randogram.png)                      | Uniform speckle — single-multiply mixing from the wyhash family                                                                                                                                                                                                                                                                                                                                           |
| **SIMD-first**           | SHISHUA                       | ![](Images/SHISHUA_randogram.png)                     | Uniform speckle — AVX2 256-bit lanes interleaved                                                                                                                                                                                                                                                                                                                                                          |
| **Stream cipher CSPRNG** | ChaCha20                      | ![](Images/ChaCha20_randogram.png)                    | Uniform speckle — cryptographic-strength diffusion                                                                                                                                                                                                                                                                                                                                                        |
| **NIST DRBG**            | AES-CTR DRBG                  | ![](Images/AesCtrDrbg_randogram.png)                  | Uniform speckle — AES-CTR output, cryptographic                                                                                                                                                                                                                                                                                                                                                           |
| **Older CSPRNG**         | ISAAC-64                      | ![](Images/Isaac_randogram.png)                       | Uniform speckle — known to have minor biases (Aumasson 2006) but visually indistinguishable                                                                                                                                                                                                                                                                                                               |
| **Quasi-random**         | Halton (base 2)               | ![](Images/Halton_randogram.png)                      | Looks blank: base-2 Halton bit-reverses the counter, so for the first ~2⁴⁸ samples the low 16 bits of every output are *structurally* zero, and every consecutive pair plots at (0, 0). The sequence is correct — it covers the *high* bits uniformly — but this particular visualisation can't show it. The bit-index histogram (`Images/Halton_bit_index.png`) is the right tool for inspecting Halton. |
| **Cellular automaton**   | Rule 30                       | ![](Images/Rule30_randogram.png)                      | Uniform speckle — Stephen Wolfram's classic CA generator. Passes the statistical tests in this repository; known to fail BigCrush at extreme sample volumes due to subtle serial correlations.                                                                                                                                                                                                            |

The visual pattern crystallises what the chi-squared and serial-correlation numbers measure: every family with "★★★" or better quality produces speckle indistinguishable from cryptographic-strength noise, while broken or biased generators show *immediately* visible structure (bands, lines, single-point collapses). A useful diagnostic when evaluating a generator: if its randogram doesn't look like uniform TV-static, neither will any application that depends on it.

**One randogram is not enough — different bit-selection methods reveal different failures.**

A single randogram only shows one *projection* of the joint distribution of bits in the RNG's output. The "Consecutive" projection (above) reveals serial correlation, but other projections expose different defects:

| Method              | What goes on (x, y)                                 | What it detects                                                                                             |
| ------------------- | --------------------------------------------------- | ----------------------------------------------------------------------------------------------------------- |
| **Consecutive**     | low 8 bits of output_n, low 8 bits of output_{n+1}  | Serial correlation between successive outputs — the Marsaglia plane test                                    |
| **Adjacent**        | bits 0–7 vs bits 8–15 of the *same* output          | Dependence between adjacent bit positions inside one output                                                 |
| **OppositeHalves**  | bits 0–7 vs bits 32–39 of the same output           | Long-range dependence between low and middle 32-bit halves                                                  |
| **StartAndReverse** | bits 0–7 of value vs bits 0–7 of bit-reversed value | High-bit vs low-bit symmetry — e.g. a generator whose high bits are good but low bits aren't, or vice versa |

The grid below shows the same seven generators under all four methods. Notice how each generator fails (or passes) in a different way depending on which bits we project:

| RNG                     | Consecutive                                           | Adjacent                                                       | OppositeHalves                                                       | StartAndReverse                                                       |
| ----------------------- | ----------------------------------------------------- | -------------------------------------------------------------- | -------------------------------------------------------------------- | --------------------------------------------------------------------- |
| **Linear Congruential** | ![](Images/LinearCongruentialGenerator_randogram.png) | ![](Images/LinearCongruentialGenerator_randogram_adjacent.png) | ![](Images/LinearCongruentialGenerator_randogram_oppositehalves.png) | ![](Images/LinearCongruentialGenerator_randogram_startandreverse.png) |
| **Multiplicative LCG**  | ![](Images/MultiplicativeLCG_randogram.png)           | ![](Images/MultiplicativeLCG_randogram_adjacent.png)           | ![](Images/MultiplicativeLCG_randogram_oppositehalves.png)           | ![](Images/MultiplicativeLCG_randogram_startandreverse.png)           |
| **Middle Square**       | ![](Images/MiddleSquare_randogram.png)                | ![](Images/MiddleSquare_randogram_adjacent.png)                | ![](Images/MiddleSquare_randogram_oppositehalves.png)                | ![](Images/MiddleSquare_randogram_startandreverse.png)                |
| **Mersenne Twister**    | ![](Images/MersenneTwister_randogram.png)             | ![](Images/MersenneTwister_randogram_adjacent.png)             | ![](Images/MersenneTwister_randogram_oppositehalves.png)             | ![](Images/MersenneTwister_randogram_startandreverse.png)             |
| **Xoshiro256\*\***      | ![](Images/Xoshiro256SS_randogram.png)                | ![](Images/Xoshiro256SS_randogram_adjacent.png)                | ![](Images/Xoshiro256SS_randogram_oppositehalves.png)                | ![](Images/Xoshiro256SS_randogram_startandreverse.png)                |
| **PCG XSL-RR**          | ![](Images/PermutedCongruentialXslRr_randogram.png)   | ![](Images/PermutedCongruentialXslRr_randogram_adjacent.png)   | ![](Images/PermutedCongruentialXslRr_randogram_oppositehalves.png)   | ![](Images/PermutedCongruentialXslRr_randogram_startandreverse.png)   |
| **ChaCha20**            | ![](Images/ChaCha20_randogram.png)                    | ![](Images/ChaCha20_randogram_adjacent.png)                    | ![](Images/ChaCha20_randogram_oppositehalves.png)                    | ![](Images/ChaCha20_randogram_startandreverse.png)                    |

Some of the most striking observations:

* **LCG**: the Marsaglia lattice is *only visible* in Consecutive. Adjacent projects to a uniform field — the low and next-low 8 bits of a single LCG output really are independent within one value. **A single-method analysis would wrongly conclude this LCG is fine.**
* **Multiplicative LCG**: produces clean vertical stripes in Adjacent — the bottom bit of any MLCG output cycles between a tiny number of values, which projects onto a few columns. The Consecutive projection shows the lattice; the Adjacent projection shows the low-bit weakness — *two different failure modes from the same generator*.
* **Mersenne Twister, Xoshiro256\*\*, PCG XSL-RR, ChaCha20**: uniform speckle in *every* projection. That's what it means to be a well-behaved generator — no single bit-projection reveals structure.
* **Middle Square**: empty in every method. Collapsed to a fixed point, so all consecutive pairs sit at the same single coordinate.

The takeaway: **always test multiple projections** when evaluating a new generator. A passing Consecutive randogram alone is necessary but not sufficient.

> Detailed per-generator histograms (bit index, bit count, spacing, repetition, Hamming distance, longest run) are available for Xoshiro256SS, ChaCha20, MultiplicativeLCG and MiddleSquare in the **Comparative Tests** section below; the other family representatives are evaluated using their randograms only, since the histogram comparisons told us little new for well-behaved generators.


**Quick-pick table — which generator should I use?**

The table below summarises the practical trade-offs for the algorithms in this repository. Period is given as $\log_2$; "Speed" is a qualitative class (★ slow / ★★ medium / ★★★ fast / ★★★★ very fast); "Quality" reflects published test-suite results (BigCrush in particular) and known weaknesses. Use this table to narrow down a candidate, then read the algorithm's section for details.

| Algorithm                                           | Year |           State | log₂(period) | Speed | Quality | Best for                                             |
| --------------------------------------------------- | ---: | --------------: | -----------: | :---: | :-----: | ---------------------------------------------------- |
| **General-purpose PRNGs**                           |      |                 |              |       |         |                                                      |
| Linear Congruential                                 | 1949 |              64 |           64 | ★★★★  |    ★    | Toy / non-statistical use only                       |
| Mersenne Twister                                    | 1997 |           19937 |        19937 |  ★★   |   ★★    | When you genuinely need huge period                  |
| WELL                                                | 2006 |            1024 |         1024 |  ★★   |   ★★    | Drop-in MT replacement                               |
| Xoshiro256**                                        | 2018 |             256 |          256 | ★★★★  |   ★★★   | **General-purpose default**                          |
| Xoshiro256+                                         | 2018 |             256 |          256 | ★★★★  |   ★★★   | Floating-point output (.NET 6+ default)              |
| Xoroshiro128++                                      | 2018 |             128 |          128 | ★★★★  |   ★★★   | When 256 bits of state is too much                   |
| Lehmer128                                           | 2019 |             128 |          126 | ★★★★  |   ★★★   | Fastest general-purpose (BMI2 platforms)             |
| RomuTrio                                            | 2020 |             192 |          ~64 | ★★★★  |   ★★★   | Speed-critical, short runs                           |
| LXM (L64X128Mix)                                    | 2021 |             192 |    64 × 2¹²⁸ |  ★★★  |   ★★★   | Splittable streams (Java compat)                     |
| WyRand                                              | 2020 |              64 |           64 | ★★★★  |   ★★    | Hash-table speeds, casual use                        |
| JSF64                                               | 2007 |             256 |          ~64 | ★★★★  |   ★★★   | Tiny state, easy to embed                            |
| PCG (RXS-M-XS)                                      | 2014 |             128 |          128 |  ★★★  |   ★★★   | Statistically strong, jumpable                       |
| PCG (XSL-RR)                                        | 2014 |             128 |          128 |  ★★★  |   ★★★   | Same core, canonical 128→64 variant                  |
| PCG (XSH-RR)                                        | 2014 |             128 |          128 |  ★★★  |   ★★★   | Same core, third PCG output transform                |
| Xoroshiro128+                                       | 2018 |             128 |          128 | ★★★★  |   ★★★   | Floating-point output; small state                   |
| Xoshiro128** / ++ / +                               | 2018 |             128 |          128 | ★★★★  |   ★★★   | 32-bit-output variants (embedded, GPU)               |
| Xoshiro512** / ++ / +                               | 2018 |             512 |          512 |  ★★★  |   ★★★   | Larger-state variants for massive parallelism        |
| RomuQuad                                            | 2020 |             256 |          ~64 | ★★★★  |   ★★★   | Largest-capacity Romu variant                        |
| RomuDuo / RomuDuoJr                                 | 2020 |             128 |          ~64 | ★★★★  |   ★★★   | Smallest/fastest Romu variants                       |
| TinyMT                                              | 2011 |             127 |       2¹²⁷-1 |  ★★★  |   ★★    | Memory-constrained MT (~17× smaller state)           |
| SHISHUA                                             | 2020 |            1024 |       ≥ 2⁵¹² | ★★★★★ |   ★★★   | SIMD-first; "fastest PRNG" claim, passes BigCrush    |
| SplitMix64                                          | 2014 |              64 |           64 | ★★★★  |   ★★    | **Seeding** other generators                         |
| **Counter-based (parallel-friendly)**               |      |                 |              |       |         |                                                      |
| Philox-2x64-10                                      | 2011 |             128 |          128 |  ★★★  |   ★★★   | GPU / massively-parallel work                        |
| Threefry-2x64-20                                    | 2011 |             256 |          256 |  ★★   |   ★★★   | Same as Philox; ARX-only (no multiply)               |
| Squares (4+1)                                       | 2022 |             128 |           64 | ★★★★  |   ★★★   | Tiny counter-based                                   |
| sfc64                                               | 2019 |             256 |         ≥ 64 | ★★★★  |   ★★★   | NumPy default alternative                            |
| MRG32k3a                                            | 1999 |             192 |         ~191 |  ★★   |   ★★★   | Scientific / MATLAB / R compat                       |
| **Cryptographic**                                   |      |                 |              |       |         |                                                      |
| ChaCha20                                            | 2008 |             512 |        ≥ 256 |  ★★   |  ★★★★   | **Default CSPRNG choice**                            |
| ChaCha12                                            | 2008 |             512 |        ≥ 256 |  ★★★  |  ★★★★   | Rust's default CSPRNG; ~1.7× faster than ChaCha20    |
| ChaCha8                                             | 2008 |             512 |        ≥ 256 | ★★★★  |   ★★★   | Go 1.22+ default PRNG; ~2.5× faster, narrower margin |
| Salsa20                                             | 2005 |             512 |        ≥ 256 |  ★★   |  ★★★★   | Predecessor of ChaCha20; underlies NaCl/XSalsa       |
| Salsa20/12                                          | 2008 |             512 |        ≥ 256 |  ★★★  |  ★★★★   | eSTREAM speed-security middle ground                 |
| Salsa20/8                                           | 2008 |             512 |        ≥ 256 | ★★★★  |   ★★★   | Fastest Salsa; non-crypto use only                   |
| Ascon-PRF                                           | 2024 |             320 |         high |  ★★★  |  ★★★★   | NIST lightweight crypto winner; sponge-based         |
| AES-CTR DRBG                                        | 2007 |             384 |         2¹²⁸ |  ★★★  |  ★★★★   | When AES-NI is available                             |
| HMAC-SHA256 DRBG                                    | 2007 |             512 |         high |   ★   |  ★★★★   | Hash-only platforms                                  |
| Hash DRBG (SHA-256)                                 | 2007 |             880 |         high |  ★★   |  ★★★★   | Hash-only; third NIST DRBG variant                   |
| ISAAC-64                                            | 1996 |            ~16k |        ≥ 2⁵⁰ |  ★★★  |   ★★    | Historical; known biases (Aumasson 2006)             |
| Trivium                                             | 2008 |             288 |        ≥ 2⁸⁰ |   ★   |   ★★★   | Hardware/embedded                                    |
| Yarrow                                              | 1999 |             256 |         high |  ★★   |   ★★★   | Pre-Fortuna design; two-pool reseeding               |
| Fortuna                                             | 2003 |  256 + 32 pools |         high |  ★★   |  ★★★★   | 32-pool design with geometric reseeding              |
| ANSI X9.31                                          | 1985 |             256 |        ≥ 2⁶⁴ |  ★★   |   ★★★   | Modernised X9.17 with AES instead of DES             |
| Blum-Blum-Shub                                      | 1986 |           small | factor-of-pq |   ★   |  ★★★★   | Theoretical / educational                            |
| **Quasi-random (NOT random — for QMC integration)** |      |                 |              |       |         |                                                      |
| Halton (base 2)                                     | 1960 |              64 |          n/a | ★★★★  |   n/a   | Monte Carlo integration                              |
| Sobol (1D)                                          | 1967 |      64 + table |          n/a | ★★★★  |   n/a   | Same, with finer convergence                         |
| **Historical / educational**                        |      |                 |              |       |         |                                                      |
| Middle Square                                       | 1946 |              64 |   very short | ★★★★  |    ✗    | Don't use; teaching what not to do                   |
| Wichmann-Hill                                       | 1982 | 96 (3×32 orig.) | ~2⁴² (orig.) |  ★★   |   ★★    | Historical interest                                  |
| XorShift (basic)                                    | 2003 |              64 |        2⁶⁴-1 | ★★★★  |    ★    | Educational; fails BigCrush                          |
| Rule 30 (CA)                                        | 1986 |             256 |         long |   ★   |    ✗    | Educational; fails statistical tests                 |

> Speed and quality columns are qualitative summaries based on published evaluations (PractRand, TestU01) and the design intent of each algorithm; for the exact numbers on your hardware, regenerate the speed benchmark via `dotnet run --configuration Release` and inspect this repository's chi-squared/correlation outputs.


**Field timeline — 80 years of generator design:**

```mermaid
timeline
  title PRNG history at a glance
  1946 : Middle Square (von Neumann)
  1949 : Linear Congruential Generator (Lehmer)
  1958 : Box-Muller transform
  1965 : Lagged Fibonacci Generator
  1967 : Sobol' low-discrepancy sequence
  1982 : Wichmann-Hill / BBS / Blum-Micali
  1986 : MINSTD (Park & Miller) / Rule 30 CA (Wolfram)
  1991 : Subtract-with-Borrow (Marsaglia)
  1994 : ISAAC (Jenkins)
  1997 : Mersenne Twister (Matsumoto & Nishimura)
  1999 : MRG32k3a (L'Ecuyer) / Multiply-with-Carry / Yarrow (Schneier-Kelsey-Ferguson)
  2003 : XorShift (Marsaglia) / Fortuna (Schneier-Ferguson)
  2005 : Salsa20 (Bernstein)
  2006 : WELL (Panneton, L'Ecuyer, Matsumoto) / NIST SP 800-90A DRBGs published
  2008 : ChaCha20 (Bernstein) / Trivium (eSTREAM)
  2011 : Philox / Threefry (Salmon et al.)
  2014 : PCG (O'Neill) / SplitMix (Steele, Lea, Flood)
  2018 : Xoshiro256** / Xoroshiro128++ (Blackman & Vigna)
  2019 : sfc64 (Doty-Humphrey) / Lehmer128 popularised
  2020 : RomuTrio (Overton) / wyrand (Wang Yi)
  2021 : LXM family (JEP 356, Java 17)
  2022 : Squares (Widynski)
```

**State size vs period — the fundamental trade-off:**

```mermaid
xychart-beta
  title "log2(period) as a function of state size (bits) — selected generators"
  x-axis "State size (bits)" [64, 64, 128, 128, 192, 256, 256, 256, 288, 19937]
  y-axis "log2(period)" 0 --> 20000
  line [64, 64, 64, 128, 96, 128, 256, 256, 288, 19937]
```

Reading order on the x-axis: WyRand (64), Squares (64), Lehmer128 (128), Xoshiro128 (128), RomuTrio (192), Xoshiro256 (256), sfc64 (256), JSF (256), Trivium (288), Mersenne Twister (19937). The line shows that period mostly tracks state size 1:1 (every extra state bit roughly doubles the period). MT19937 dwarfs everything in the chart — its monstrous 19,937-bit state is exactly what gives it its $2^{19937}-1$ period, but as Vigna and others have argued, periods past $\sim 2^{256}$ are rarely useful in practice and the extra state space mostly costs cache footprint.

**Speed:**

To evaluate the speed of the random number generator, we simply measure how many random numbers are generated in a given timeframe and then normalize that to obtain an iterations per second metric. This metric helps compare the performance of different PRNGs and identify which ones are more efficient. A higher iterations per second value indicates a faster generator, which can be crucial in applications where large amounts of random data are needed quickly.

> [!NOTE]
> The chart below reflects an earlier benchmark run and does **not** include the generators added in subsequent updates: WyRand, sfc64, MRG32k3a, Philox, Threefry, Squares, JSF, RomuTrio, Lehmer128, LXM. Run `dotnet run --project Randomizer/Randomizer.csproj --configuration Release` to regenerate against the full current set.

```mermaid
xychart-beta
title "Iterations (PRNG)"
x-axis "Generator" [".Net (LFG-)",".Net (XSR)","ACORN","CLCG+","CLCG-","CLCG*","CLCG^","CMWC","FCSR","ICG","KISS+","KISS-","KISS*","KISS^","LFG+","LFG-","LFG*","LFG^","LCG","LFSR","MT","MS","MSWS","MIXMAX","MLCG","MWC","PCG","SM","SWB","WELL","WH","XRSR","XS","XS+","XS*","XW","XSR"]
y-axis "n per second" 0 --> 45000000
bar [25860404, 41502895, 24141225, 41068247, 41180385, 41121380, 41185898, 42286394, 3162065, 6544910, 36399195, 36608783, 36509695, 36584089, 41126514, 41048069, 41315409, 40896199, 44379396, 3580744, 35036819, 42932495, 42359983, 13300, 46167931, 45209291, 42154250, 45800946, 38437975, 38184646, 6437107, 46023155, 46447791, 46073354, 45825078, 43358518, 44978039]
```

```mermaid
xychart-beta
title "Iterations (CSRNG)"
x-axis "Generator" [".Net","BBS","CC20","BlumMicali","SSG"]
y-axis "n per second" 0 --> 11000000
bar [10941568,3076239,1673562,3708,668359]
```

> [!NOTE]
> The CSRNG chart label "BlumMicali" was previously abbreviated "BM", which collides with "BM" used elsewhere for Box-Muller. The chart also predates the addition of **ISAAC** and **Trivium** — re-run the benchmark to refresh.

**Histogram of 64 1-Bits:**

We create a histogram to analyze how often each bit position in a 64-bit sequence is set to 1 or 0. The goal is to visualize how often each of the 64 bit positions contains a set bit (1) versus an unset bit (0) across many generated random numbers.

For each bit position (0 to 63), count how often it is set to 1 and how often it is set to 0 across the entire sample. Plot these counts on a histogram where the counts of 1s are displayed above a central horizontal line, and the counts of 0s are displayed below. This creates a visual resembling a horizon with skyscrapers above and their reflections in water below. This visualization helps reveal any potential bias by showing if certain bit positions are set more frequently than others.

A well-behaved RNG (Xoshiro256SS) shows perfectly balanced bars — each bit is roughly 50% ones and 50% zeros. The Middle Square generator with the seed used here collapses very quickly to zero — every middle-of-the-square iteration with a value whose square has too few significant digits truncates to 0, and once it hits 0 it stays there. This is the classic failure mode von Neumann documented in 1946; *which* seeds collapse and how fast depends on the modulus and starting value.

| Xoshiro256SS — bars uniform height above and below the centre line: every bit is ~50% ones | Middle Square — no bars at all above the line: the generator has stopped emitting ones |
| ------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------- |
| ![Xoshiro256SS bit index](Images/Xoshiro256SS_bit_index.png)                               | ![MiddleSquare bit index](Images/MiddleSquare_bit_index.png)                           |

The Multiplicative LCG with default parameters shows the classic weakness of multiplicative generators — *look at the leftmost few bars*: bit 0 is missing entirely (stuck at zero), and the next few bit positions show progressively shrinking heights compared to the upper bits.

![MultiplicativeLCG bit index — leftmost bars are stunted while the right side is balanced](Images/MultiplicativeLCG_bit_index.png)

**Histogram of 1-Bit Counts:**

The histogram of 1-bit counts helps determine the distribution of the number of 1-bits across multiple generated numbers. For each 64-bit random number, count the number of bits set to 1. Accumulate these counts in 65 buckets, where each bucket represents a possible count of 1-bits from 0 to 64.

The goal is to observe how uniformly the 1-bits are distributed among the generated numbers. A balanced random generator should yield a roughly bell-shaped distribution centered around 32 (assuming 50% of bits are expected to be 1s). Significant deviations from this pattern may indicate non-uniformity or bias in the generator.

| Xoshiro256SS — bell-shaped curve symmetric around popcount 32, fading to zero by ~20 and ~44 | Middle Square — single tall bar at popcount 0, all other positions empty |
| -------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------ |
| ![Xoshiro256SS bit count](Images/Xoshiro256SS_bit_count.png)                                 | ![MiddleSquare bit count](Images/MiddleSquare_bit_count.png)             |

**Longest Run of Ones and Zeroes in a Histogram:**

The longest run test measures the maximum consecutive sequence of 1s or 0s in the generated random numbers. This test helps identify whether the PRNG produces unusually long sequences of identical bits, which may indicate a lack of randomness.

To implement this test, generate a series of random numbers and analyze each bit sequence to determine the longest contiguous run of 1s and 0s. Track the maximum length found for both 1s and 0s across the entire sample, and plot these values on a histogram. For a truly random sequence, the longest run should fall within expected limits based on the sample size.

The distribution of longest-run lengths follows a known geometric-like distribution — runs of length 4-6 should be most common in a 64-bit value, with longer runs becoming exponentially rarer.

![Xoshiro256SS longest run — symmetric peak at run length 4-6, exponential decay on both sides; if instead you see a flat band, suspiciously short peak, or runs significantly longer than ~12 dominating, the generator is biased](Images/Xoshiro256SS_longest_run.png)

**Histogram of Spacing Between Consecutive Values:**

To further evaluate randomness quality, we create a histogram of the spacing between consecutive values in the generated sequence. For each random number, calculate the difference between it and the previous value. Divide the possible range of the difference (i.e., the ulong range) into n evenly spaced buckets, and increment the corresponding bucket counter based on the observed spacing.

By accumulating these differences into a histogram, we can visualize the distribution of gaps between values.

For a uniformly distributed RNG, the distribution of the *raw* signed difference $x_n - x_{n-1}$ (treated on a circle of $2^{64}$ values, i.e. allowing wrap-around) is uniform; the distribution of the *absolute* difference $|x_n - x_{n-1}|$ ignoring wrap-around — which is what the implementation here measures — follows a triangular shape, peaking at 0 and decreasing linearly toward the maximum possible difference.

| Xoshiro256SS — bars tallest on the left and shrinking linearly to the right edge (classic triangular shape) | ChaCha20 — identical triangular profile, confirming the two unrelated algorithms agree on what "uniform" looks like |
| ----------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| ![Xoshiro256SS spacing](Images/Xoshiro256SS_spacing.png)                                                    | ![ChaCha20 spacing](Images/ChaCha20_spacing.png)                                                                    |

**Repetition Histogram:**

The repetition histogram helps determine how often numbers from all buckets are generated.

To conduct this test, we divide the output range into n buckets and generate n random numbers, recording how many times each bucket is hit. We expect a uniform distribution, where each bucket is hit roughly the same number of times. If the distribution is uneven, it may indicate bias or flaws in the random number generation process.

A good RNG produces a flat histogram — each bin receives approximately n/k hits with statistical noise. The Middle Square repetition histogram concentrates all values in the very first bin because the generator is producing only zero.

| Xoshiro256SS — all 64 bars at roughly the same height, with only small statistical noise between bins | Middle Square — only the leftmost bin has any height; the remaining 63 bins are empty |
| ----------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------- |
| ![Xoshiro256SS repetition](Images/Xoshiro256SS_repetition.png)                                        | ![MiddleSquare repetition](Images/MiddleSquare_repetition.png)                        |

**Repeat Streak Histogram:**

The repeat streak histogram tracks how often runs of length 1 to k occur in an n-number sliding window to detect patterns that suggest non-randomness. This helps identify potential patterns that could indicate a lack of randomness.

To perform this test, use k buckets to track runs of length 1 to k. For each sliding window of size n, determine all runs of length 1 to k and count how often they occur, accumulating these counts in the corresponding buckets. Ideally, shorter runs should appear more frequently, while longer runs should be rare. If longer runs are observed too often, it may indicate a bias or flaw in the PRNG, reducing unpredictability.

A related visualization is the **Hamming-distance histogram** between consecutive outputs — i.e. how many bits change from one value to the next. A good RNG should peak at 32 (half the bits flip on average) with binomial spread. A weak generator will show shifted or narrowed distributions.

| Xoshiro256SS — symmetric bell curve peaking exactly at 32 (half the bits flip between outputs) | Multiplicative LCG — peak visibly shifted away from 32 and the curve is narrower, meaning each step changes a non-random number of bits |
| ---------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| ![Xoshiro256SS hamming](Images/Xoshiro256SS_hamming.png)                                       | ![MultiplicativeLCG hamming](Images/MultiplicativeLCG_hamming.png)                                                                      |

**Randograms:**

To construct these randograms, generate random values and assign them to points in the corresponding 3D or 2D space. The goal is to observe whether the points are uniformly distributed or if there are patterns, clusters, or gaps. Ideally, a good random generator should produce a uniform scatter without discernible patterns.

The four 256×256 randograms below show how dramatic the differences can be. Each pixel's intensity reflects how often that (x, y) pair occurred over 100,000 samples; uniform speckle indicates uniformity, while visible patterns or single-pixel concentration reveal structural flaws.

|                                                                                                                                             |                                                                                                                                                                       |
| ------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Xoshiro256SS** — uniform grey-and-white speckle covering the entire square, no lines, bands, or empty regions                             | **ChaCha20** — same uniform speckle pattern as Xoshiro: a cryptographic and a non-cryptographic generator agree on what "random" should look like                     |
| ![Xoshiro256SS randogram](Images/Xoshiro256SS_randogram.png)                                                                                | ![ChaCha20 randogram](Images/ChaCha20_randogram.png)                                                                                                                  |
| **Multiplicative LCG** — visible horizontal striping/banding: certain y-values (built from low-order bits) recur far more often than others | **Middle Square** — almost entirely blank: every sample landed at the same (x, y) coordinate so a single dot (sometimes invisible at this zoom) contains all the data |
| ![MultiplicativeLCG randogram](Images/MultiplicativeLCG_randogram.png)                                                                      | ![MiddleSquare randogram](Images/MiddleSquare_randogram.png)                                                                                                          |

## The NuGet package

The library includes performance-oriented implementations of most of the examples discussed above. The code is designed to be both readable and understandable, with a focus on clarity without compromising on efficiency. It offers parameterized entities and methods, making it easy to customize and extend according to your specific needs.

# Links

* [RNG Engines](https://pracrand.sourceforge.net/RNG_engines.txt)
* [Reversable RNG](http://robotics.ucsd.edu/ReversibleRNG.pdf)
* [FastRNG](https://github.com/SommerEngineering/FastRng)
* [Randomness in .NET](https://blogs.siliconorchid.com/post/coding-inspiration/randomness-in-dotnet/)

# History

* 20240711 - Initial GitHub Version
* 20240624 - Initial Version

## ❤️ Support

If this project saves you time or money, consider supporting its development:

[![GitHub Sponsors](https://img.shields.io/badge/GitHub-Sponsor-EA4AAA?logo=githubsponsors)](https://github.com/sponsors/Hawkynt)
[![PayPal](https://img.shields.io/badge/PayPal-Donate-00457C?logo=paypal)](https://www.paypal.me/hawkynt)

## 📜 License

Licensed under LGPL-3.0-or-later — see [LICENSE](LICENSE).
