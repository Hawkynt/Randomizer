using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

// ReSharper disable UnusedMember.Global

namespace Hawkynt.RandomNumberGenerators.Composites;

partial class ArbitraryNumberGenerator {
  /// <summary>
  ///   Generates a random 32-bit unsigned integer by truncating the high bits of a random 64-bit unsigned integer.
  /// </summary>
  /// <returns>A 32-bit unsigned integer.</returns>
  /// <example>
  ///   <code>
  /// uint randomValue = Truncate32();
  /// Console.WriteLine(randomValue);
  /// // Output: A random 32-bit unsigned integer.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public uint Truncate32() => (uint)rng.Next();

  /// <summary>
  ///   Generates a random 16-bit unsigned integer by truncating the high bits of a random 64-bit unsigned integer.
  /// </summary>
  /// <returns>A 16-bit unsigned integer.</returns>
  /// <example>
  ///   <code>
  /// ushort randomValue = Truncate16();
  /// Console.WriteLine(randomValue);
  /// // Output: A random 16-bit unsigned integer.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ushort Truncate16() => (ushort)rng.Next();

  /// <summary>
  ///   Generates a random 8-bit unsigned integer by truncating the high bits of a random 64-bit unsigned integer.
  /// </summary>
  /// <returns>An 8-bit unsigned integer.</returns>
  /// <example>
  ///   <code>
  /// byte randomValue = Truncate8();
  /// Console.WriteLine(randomValue);
  /// // Output: A random 8-bit unsigned integer.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte Truncate8() => (byte)rng.Next();

  /// <summary>
  ///   Generates a random boolean value by extracting the least significant bit of a random 64-bit unsigned integer.
  /// </summary>
  /// <returns><see langword="true" /> if the least significant bit is 1; otherwise, <see langword="false" />.</returns>
  /// <example>
  ///   <code>
  /// bool randomValue = Truncate1();
  /// Console.WriteLine(randomValue);
  /// // Output: A random boolean value (true or false).
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Truncate1() => (rng.Next() & 1) == 1;

  /// <summary>
  ///   Generates a random unsigned integer by truncating a specified number of bits from a random 64-bit unsigned integer.
  /// </summary>
  /// <param name="bitCount">The number of bits to retain in the result. Must be between 1 and 63.</param>
  /// <returns>An unsigned integer with the specified number of bits.</returns>
  /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="bitCount" /> is 0 or greater than 63.</exception>
  /// <example>
  ///   <code>
  /// ulong randomValue = Truncate(10);
  /// Console.WriteLine(randomValue);
  /// // Output: A random unsigned integer with up to 10 bits.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ulong Truncate(byte bitCount) {
    ArgumentOutOfRangeException.ThrowIfZero(bitCount);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, (byte)63);

    return rng.Next() & ((1UL << bitCount) - 1);
  }

  /// <summary>
  ///   Generates a random 32-bit unsigned integer by truncating the lower 32 bits of a random 64-bit unsigned integer.
  /// </summary>
  /// <returns>A 32-bit unsigned integer.</returns>
  /// <example>
  ///   <code>
  /// uint randomValue = Shift32();
  /// Console.WriteLine(randomValue);
  /// // Output: A random 32-bit unsigned integer.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public uint Shift32() => (uint)(rng.Next() >> 32);

  /// <summary>
  ///   Generates a random 16-bit unsigned integer by truncating the lower 48 bits of a random 64-bit unsigned integer.
  /// </summary>
  /// <returns>A 16-bit unsigned integer.</returns>
  /// <example>
  ///   <code>
  /// ushort randomValue = Shift16();
  /// Console.WriteLine(randomValue);
  /// // Output: A random 16-bit unsigned integer.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ushort Shift16() => (ushort)(rng.Next() >> 48);

  /// <summary>
  ///   Generates a random 8-bit unsigned integer by truncating the lower 56 bits of a random 64-bit unsigned integer.
  /// </summary>
  /// <returns>An 8-bit unsigned integer.</returns>
  /// <example>
  ///   <code>
  /// byte randomValue = Shift8();
  /// Console.WriteLine(randomValue);
  /// // Output: A random 8-bit unsigned integer.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte Shift8() => (byte)(rng.Next() >> 56);

  /// <summary>
  ///   Generates a random boolean value by truncating the lower 63 bits of a random 64-bit unsigned integer.
  /// </summary>
  /// <returns><see langword="true" /> if the most significant bit is 1; otherwise, <see langword="false" />.</returns>
  /// <example>
  ///   <code>
  /// bool randomValue = Shift1();
  /// Console.WriteLine(randomValue);
  /// // Output: A random boolean value (true or false).
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Shift1() => rng.Next() >> 63 == 1;

  /// <summary>
  ///   Generates a random unsigned integer by shifting the specified number of bits from a random 64-bit unsigned integer.
  /// </summary>
  /// <param name="bitCount">The number of most significant bits to retain in the result. Must be between 1 and 63.</param>
  /// <returns>An unsigned integer with the specified number of bits.</returns>
  /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="bitCount" /> is 0 or greater than 63.</exception>
  /// <example>
  ///   <code>
  /// ulong randomValue = Shift(10);
  /// Console.WriteLine(randomValue);
  /// // Output: A random unsigned integer with up to 10 bits.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ulong Shift(byte bitCount) {
    ArgumentOutOfRangeException.ThrowIfZero(bitCount);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, (byte)63);

    return rng.Next() >> (64 - bitCount);
  }

  /// <summary>
  ///   Generates a random 32-bit unsigned integer by applying a mask to a random 64-bit unsigned integer.
  /// </summary>
  /// <param name="mask">The bitmask used to extract the desired bits. The mask must have between 1 and 32 bits set.</param>
  /// <returns>A 32-bit unsigned integer containing the bits extracted by the mask.</returns>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   Thrown if <paramref name="mask" /> is 0 or if the number of bits
  ///   set in <paramref name="mask" /> exceeds 32.
  /// </exception>
  /// <example>
  ///   <code>
  /// uint randomValue = Mask32(0x00000000FFFFFFFF);
  /// Console.WriteLine(randomValue);
  /// // Output: A random 32-bit unsigned integer based on the lower 32 bits of the mask.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public uint Mask32(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 32UL, nameof(mask));

    return (uint)_ParallelBitExtract(rng.Next(), mask);
  }

  /// <summary>
  ///   Generates a random 16-bit unsigned integer by applying a mask to a random 64-bit unsigned integer.
  /// </summary>
  /// <param name="mask">The bitmask used to extract the desired bits. The mask must have between 1 and 16 bits set.</param>
  /// <returns>A 16-bit unsigned integer containing the bits extracted by the mask.</returns>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   Thrown if <paramref name="mask" /> is 0 or if the number of bits
  ///   set in <paramref name="mask" /> exceeds 16.
  /// </exception>
  /// <example>
  ///   <code>
  /// ushort randomValue = Mask16(0x000000000000FFFF);
  /// Console.WriteLine(randomValue);
  /// // Output: A random 16-bit unsigned integer based on the lower 16 bits of the mask.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ushort Mask16(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 16UL, nameof(mask));

    return (ushort)_ParallelBitExtract(rng.Next(), mask);
  }

  /// <summary>
  ///   Generates a random 8-bit unsigned integer by applying a mask to a random 64-bit unsigned integer.
  /// </summary>
  /// <param name="mask">The bitmask used to extract the desired bits. The mask must have between 1 and 8 bits set.</param>
  /// <returns>An 8-bit unsigned integer containing the bits extracted by the mask.</returns>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   Thrown if <paramref name="mask" /> is 0 or if the number of bits
  ///   set in <paramref name="mask" /> exceeds 8.
  /// </exception>
  /// <example>
  ///   <code>
  /// byte randomValue = Mask8(0x00000000000000FF);
  /// Console.WriteLine(randomValue);
  /// // Output: A random 8-bit unsigned integer based on the lower 8 bits of the mask.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte Mask8(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 8UL, nameof(mask));

    return (byte)_ParallelBitExtract(rng.Next(), mask);
  }

  /// <summary>
  ///   Generates a random boolean value by applying a single-bit mask to a random 64-bit unsigned integer.
  /// </summary>
  /// <param name="mask">The bitmask used to extract the desired bit. The mask must have exactly 1 bit set.</param>
  /// <returns><see langword="true" /> if the extracted bit is 1; otherwise, <see langword="false" />.</returns>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   Thrown if <paramref name="mask" /> is 0 or if the number of bits
  ///   set in <paramref name="mask" /> exceeds 1.
  /// </exception>
  /// <example>
  ///   <code>
  /// bool randomValue = Mask1(0x0000001000000000);
  /// Console.WriteLine(randomValue);
  /// // Output: A random boolean value based on the masked bit.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Mask1(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 1UL, nameof(mask));

    return _ParallelBitExtract(rng.Next(), mask) != 0;
  }

  /// <summary>
  ///   Generates a random unsigned integer by applying a mask to a random 64-bit unsigned integer.
  /// </summary>
  /// <param name="mask">The bitmask used to extract the desired bits. The mask must have between 1 and 63 bits set.</param>
  /// <returns>An unsigned integer containing the bits extracted by the mask.</returns>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   Thrown if <paramref name="mask" /> is 0 or if the number of bits
  ///   set in <paramref name="mask" /> exceeds 63.
  /// </exception>
  /// <example>
  ///   <code>
  /// ulong randomValue = Mask(0x0F0F010020F0F00F);
  /// Console.WriteLine(randomValue);
  /// // Output: A random unsigned integer based on the masked bits.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ulong Mask(ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(ulong.PopCount(mask), 63UL, nameof(mask));

    return _ParallelBitExtract(rng.Next(), mask);
  }

  /// <summary>
  ///   Generates a random 32-bit unsigned integer by applying a sponge function to a random 64-bit unsigned integer.
  /// </summary>
  /// <returns>A 32-bit unsigned integer.</returns>
  /// <example>
  ///   <code>
  /// uint randomValue = Sponge32();
  /// Console.WriteLine(randomValue);
  /// // Output: A random 32-bit unsigned integer.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public uint Sponge32() {
    var result = rng.Next();
    result ^= result >> 32;
    return (uint)result;
  }

  /// <summary>
  ///   Generates a random 16-bit unsigned integer by applying a sponge function to a random 64-bit unsigned integer.
  /// </summary>
  /// <returns>A 16-bit unsigned integer.</returns>
  /// <example>
  ///   <code>
  /// ushort randomValue = Sponge16();
  /// Console.WriteLine(randomValue);
  /// // Output: A random 16-bit unsigned integer.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ushort Sponge16() {
    var result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    return (ushort)result;
  }

  /// <summary>
  ///   Generates a random 8-bit unsigned integer by applying a sponge function to a random 64-bit unsigned integer.
  /// </summary>
  /// <returns>An 8-bit unsigned integer.</returns>
  /// <example>
  ///   <code>
  /// byte randomValue = Sponge8();
  /// Console.WriteLine(randomValue);
  /// // Output: A random 8-bit unsigned integer.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte Sponge8() {
    var result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    result ^= result >> 8;
    return (byte)result;
  }

  /// <summary>
  ///   Generates a random 4-bit unsigned integer by applying a sponge function to a random 64-bit unsigned integer.
  /// </summary>
  /// <returns>A 4-bit unsigned integer.</returns>
  /// <example>
  ///   <code>
  /// byte randomValue = Sponge4();
  /// Console.WriteLine(randomValue);
  /// // Output: A random 4-bit unsigned integer (0-15).
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte Sponge4() {
    var result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    result ^= result >> 8;
    result ^= result >> 4;
    return (byte)(result & 0xF);
  }

  /// <summary>
  ///   Generates a random 2-bit unsigned integer by applying a sponge function to a random 64-bit unsigned integer.
  /// </summary>
  /// <returns>A 2-bit unsigned integer.</returns>
  /// <example>
  ///   <code>
  /// byte randomValue = Sponge2();
  /// Console.WriteLine(randomValue);
  /// // Output: A random 2-bit unsigned integer (0-3).
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public byte Sponge2() {
    var result = rng.Next();
    result ^= result >> 32;
    result ^= result >> 16;
    result ^= result >> 8;
    result ^= result >> 4;
    result ^= result >> 2;
    return (byte)(result & 0x3);
  }

  /// <summary>
  ///   Generates a random boolean value by applying a sponge function to a random 64-bit unsigned integer.
  /// </summary>
  /// <returns><see langword="true" /> if the result is non-zero; otherwise, <see langword="false" />.</returns>
  /// <example>
  ///   <code>
  /// bool randomValue = Sponge1();
  /// Console.WriteLine(randomValue);
  /// // Output: A random boolean value (true or false).
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Sponge1() {
    var result = rng.Next();
    if (Popcnt.X64.IsSupported)
      return (Popcnt.X64.PopCount(result) & 1) != 0;

    result ^= result >> 32;
    result ^= result >> 16;
    result ^= result >> 8;
    result ^= result >> 4;
    result ^= result >> 2;
    result ^= result >> 1;
    return result != 0;
  }

  /// <summary>
  ///   Constructs a random unsigned integer by repeatedly applying a mask to random 64-bit unsigned integers.
  /// </summary>
  /// <param name="bitsTotal">The total number of bits to construct. Must be between 1 and 64.</param>
  /// <param name="mask">
  ///   The bitmask used to extract bits in each round. The mask must have a number of bits that evenly
  ///   divides <paramref name="bitsTotal" />.
  /// </param>
  /// <returns>An unsigned integer with the specified total number of bits.</returns>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   Thrown if <paramref name="bitsTotal" /> is 0 or greater than 64, if <paramref name="mask" /> is 0,
  ///   if the number of bits in <paramref name="mask" /> is greater than <paramref name="bitsTotal" />,
  ///   or if <paramref name="bitsTotal" /> is not evenly divisible by the number of bits in <paramref name="mask" />.
  /// </exception>
  /// <example>
  ///   <code>
  /// ulong randomValue = Construct(32, 0x00000000FF000000);
  /// Console.WriteLine(randomValue);
  /// // Output: A random 32-bit unsigned integer constructed using the specified mask.
  /// </code>
  /// </example>
  public ulong Construct(byte bitsTotal, ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(bitsTotal);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitsTotal, 64);
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    var bitsPerRound = (byte)BitOperations.PopCount(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitsPerRound, bitsTotal);
    ArgumentOutOfRangeException.ThrowIfNotEqual(bitsTotal % bitsPerRound, 0, nameof(mask));

    var result = 0UL;
    do {
      var random = rng.Next();
      var roundBits = _ParallelBitExtract(random, mask);
      result <<= bitsPerRound;
      result |= roundBits;
      bitsTotal -= bitsPerRound;
    } while (bitsTotal > 0);

    return result;
  }

  /// <summary>
  ///   Generates two random 32-bit unsigned integers by slicing a random 64-bit unsigned integer into two parts.
  /// </summary>
  /// <returns>A tuple containing two 32-bit unsigned integers.</returns>
  /// <example>
  ///   <code>
  /// var (first, second) = Slice32x2();
  /// Console.WriteLine($"First: {first}, Second: {second}");
  /// // Output: Two random 32-bit unsigned integers.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (uint, uint) Slice32x2() {
    var random = new SliceUnion(rng.Next());
    return (
      random.R32_0,
      random.R32_1
    );
  }

  /// <summary>
  ///   Generates four random 16-bit unsigned integers by slicing a random 64-bit unsigned integer into four parts.
  /// </summary>
  /// <returns>A tuple containing four 16-bit unsigned integers.</returns>
  /// <example>
  ///   <code>
  /// var (first, second, third, fourth) = Slice16x4();
  /// Console.WriteLine($"First: {first}, Second: {second}, Third: {third}, Fourth: {fourth}");
  /// // Output: Four random 16-bit unsigned integers.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (ushort, ushort, ushort, ushort) Slice16x4() {
    var random = new SliceUnion(rng.Next());
    return (
      random.R16_0,
      random.R16_1,
      random.R16_2,
      random.R16_3
    );
  }

  /// <summary>
  ///   Generates eight random 8-bit unsigned integers by slicing a random 64-bit unsigned integer into eight parts.
  /// </summary>
  /// <returns>A tuple containing eight 8-bit unsigned integers.</returns>
  /// <example>
  ///   <code>
  /// var (first, second, third, fourth, fifth, sixth, seventh, eighth) = Slice8x8();
  /// Console.WriteLine($"First: {first}, Second: {second}, Third: {third}, Fourth: {fourth}, Fifth: {fifth}, Sixth: {sixth}, Seventh: {seventh}, Eighth: {eighth}");
  /// // Output: Eight random 8-bit unsigned integers.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (byte, byte, byte, byte, byte, byte, byte, byte) Slice8x8() {
    var random = new SliceUnion(rng.Next());
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

  /// <summary>
  ///   Generates a random unsigned integer by applying the modulus operation to a random 64-bit unsigned integer.
  /// </summary>
  /// <param name="mod">The modulus to apply. Must be greater than zero.</param>
  /// <returns>A random unsigned integer in the range [0, <paramref name="mod" /> - 1].</returns>
  /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="mod" /> is zero.</exception>
  /// <example>
  ///   <code>
  /// ulong randomValue = Modulo(10);
  /// Console.WriteLine(randomValue);
  /// // Output: A random unsigned integer between 0 and 9.
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ulong Modulo(ulong mod) {
    ArgumentOutOfRangeException.ThrowIfZero(mod);

    return rng.Next() % mod;
  }

  /// <summary>
  ///   Generates a random unsigned integer using rejection sampling to ensure the result is less than the specified modulus.
  /// </summary>
  /// <param name="mod">The modulus to apply. Must be greater than zero.</param>
  /// <returns>A random unsigned integer in the range [0, <paramref name="mod" /> - 1].</returns>
  /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="mod" /> is zero.</exception>
  /// <example>
  ///   <code>
  /// ulong randomValue = RejectionSampling(10);
  /// Console.WriteLine(randomValue);
  /// // Output: A random unsigned integer between 0 and 9, generated using rejection sampling.
  /// </code>
  /// </example>
  public ulong RejectionSampling(ulong mod) {
    ArgumentOutOfRangeException.ThrowIfZero(mod);

    ulong result;
    do {
      result = rng.Next();
    } while (result >= mod);

    return result;
  }

  /// <summary>
  ///   Generates a random unsigned integer within a specified range using a combination of modulo and rejection sampling.
  /// </summary>
  /// <param name="mod">The modulus that defines the upper bound for the random number. Must be greater than zero.</param>
  /// <returns>A random unsigned integer in the range [0, <paramref name="mod" /> - 1].</returns>
  /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="mod" /> is zero.</exception>
  /// <example>
  ///   <code>
  /// ulong randomValue = ModuloRejectionSampling(10);
  /// Console.WriteLine(randomValue);
  /// // Output: A random unsigned integer between 0 and 9, generated using an optimized combination of modulo and rejection sampling.
  /// </code>
  /// </example>
  public ulong ModuloRejectionSampling(ulong mod) {
    ArgumentOutOfRangeException.ThrowIfZero(mod);

    // Check if mod is a power of 2
    if ((mod & (mod - 1)) == 0)
      // If mod is a power of 2, we can directly return the result of modulo operation
      return rng.Next() & (mod - 1);

    var maxValidRange = ulong.MaxValue - ulong.MaxValue % mod;
    ulong result;
    do {
      result = rng.Next();
    } while (result >= maxValidRange);

    return result % mod;
  }

  /// <summary>
  ///   Scales a random number to a specified range by multiplying it by a scale factor and normalizing it against
  ///   <see cref="ulong.MaxValue" />.
  /// </summary>
  /// <param name="scale">The factor by which to scale the random number. This defines the upper bound of the scaled value.</param>
  /// <returns>A random double in the range [0, <paramref name="scale" />).</returns>
  /// <example>
  ///   <code>
  /// double scaledValue = Scale(100.0);
  /// Console.WriteLine(scaledValue);
  /// // Output: A random double in the range [0, 100).
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public double Scale(double scale) => rng.Next() * scale / ulong.MaxValue;

  /// <summary>
  ///   Generates a random single-precision floating-point number uniformly distributed between 0 (inclusive) and 1
  ///   (exclusive).
  /// </summary>
  /// <returns>A random float in the range [0.0f, 1.0f).</returns>
  /// <example>
  ///   <code>
  /// float randomValue = NextSingle();
  /// Console.WriteLine(randomValue);
  /// // Output: A random float in the range [0.0f, 1.0f).
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public float NextSingle() {
    var mantissa = (uint)(rng.Next() >> (64 - 23));
    var floatBits = (127 << 23) | mantissa;
    return BitConverter.Int32BitsToSingle((int)floatBits) - 1.0f;
  }

  /// <summary>
  ///   Generates a random double-precision floating-point number uniformly distributed between 0 (inclusive) and 1
  ///   (exclusive).
  /// </summary>
  /// <returns>A random double in the range [0.0d, 1.0d).</returns>
  /// <example>
  ///   <code>
  /// double randomValue = NextDouble();
  /// Console.WriteLine(randomValue);
  /// // Output: A random double in the range [0.0d, 1.0d).
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public double NextDouble() {
    var mantissa = rng.Next() >> (64 - 52);
    var doubleBits = (1023UL << 52) | mantissa;
    return BitConverter.Int64BitsToDouble((long)doubleBits) - 1.0d;
  }
}
