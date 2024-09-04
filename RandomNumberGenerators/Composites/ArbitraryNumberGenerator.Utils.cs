using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Hawkynt.RandomNumberGenerators.Composites;

partial class ArbitraryNumberGenerator {
  /// <summary>
  ///   Extracts bits from the specified <paramref name="value" /> according to the bit positions set in the
  ///   <paramref name="mask" />.
  /// </summary>
  /// <param name="value">The <see cref="ulong" /> value from which bits are extracted.</param>
  /// <param name="mask">The <see cref="ulong" /> mask that specifies which bits to extract.</param>
  /// <returns>
  ///   A <see cref="ulong" /> value containing the bits extracted from <paramref name="value" /> and positioned contiguously
  ///   based on the <paramref name="mask" />.
  /// </returns>
  /// <remarks>
  ///   If the CPU supports the BMI2 instruction set, the PEXT (Parallel Bits Extract) instruction is used for optimal
  ///   performance.
  ///   If not, a manual bit-by-bit extraction is performed.
  /// </remarks>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static ulong _ParallelBitExtract(ulong value, ulong mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);

    if (Bmi2.X64.IsSupported)
      return Bmi2.X64.ParallelBitExtract(value, mask);

    var result = 0UL;
    var random = value & mask;
    var position = 0;
    do {
      if ((mask & 1) != 0)
        result |= (random & 1) << position++;

      mask >>= 1;
      random >>= 1;
    } while (mask > 0);

    return result;
  }

  /// <summary>
  ///   Calculates the population count (the number of 1-bits) in a 128-bit unsigned integer.
  /// </summary>
  /// <param name="mask">The 128-bit unsigned integer.</param>
  /// <returns>The number of 1-bits in the provided 128-bit unsigned integer.</returns>
  /// <example>
  ///   <code>
  /// UInt128 mask = 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF;
  /// byte count = _PopCount(mask);
  /// Console.WriteLine(count); 
  /// // Output: 128
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static byte _PopCount(UInt128 mask) => (byte)(ulong.PopCount((ulong)mask) + ulong.PopCount((ulong)(mask >> 64)));

  /// <summary>
  ///   Calculates the population count (the number of 1-bits) in a 256-bit vector.
  /// </summary>
  /// <param name="mask">The 256-bit vector.</param>
  /// <returns>The total number of 1-bits in the vector.</returns>
  /// <example>
  ///   <code>
  /// var vector = Vector256.Create(0xFFFFFFFFFFFFFFFFUL, 0xFFFFFFFFFFFFFFFFUL, 0x0UL, 0x0UL);
  /// ushort count = _PopCount(vector);
  /// Console.WriteLine(count);
  /// // Output: 128 (64 bits from the first element + 64 bits from the second element)
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static ushort _PopCount(Vector256<ulong> mask) {
    var pc0 = ulong.PopCount(mask.GetElement(0));
    var pc1 = ulong.PopCount(mask.GetElement(1));
    var pc2 = ulong.PopCount(mask.GetElement(2));
    var pc3 = ulong.PopCount(mask.GetElement(3));

    var pc01 = pc0 + pc1;
    var pc23 = pc2 + pc3;

    return (ushort)(pc01 + pc23);
  }

  /// <summary>
  ///   Calculates the population count (the number of 1-bits) in a 512-bit vector.
  /// </summary>
  /// <param name="mask">The 512-bit vector.</param>
  /// <returns>The total number of 1-bits in the vector.</returns>
  /// <example>
  ///   <code>
  /// var vector = Vector512.Create(0xFFFFFFFFFFFFFFFFUL, 0xFFFFFFFFFFFFFFFFUL, 0xFFFFFFFFFFFFFFFFUL, 0xFFFFFFFFFFFFFFFFUL,
  ///                                0x0UL, 0x0UL, 0x0UL, 0x0UL);
  /// ushort count = _PopCount(vector);
  /// Console.WriteLine(count);
  /// // Output: 256 (64 bits from each of the first four elements)
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static ushort _PopCount(Vector512<ulong> mask) {
    var pc0123 = _PopCount(mask.GetLower());
    var pc4567 = _PopCount(mask.GetUpper());

    return (ushort)(pc0123 + pc4567);
  }

  /// <summary>
  ///   Increments a multi-byte counter represented as a byte array, treating the byte array as a little-endian integer.
  /// </summary>
  /// <param name="counter">The byte array representing the counter to increment.</param>
  /// <example>
  ///   <code>
  /// byte[] counter = { 0xFF, 0xFF, 0xFF, 0xFE };
  /// _Increment(counter);
  /// // After increment: counter = { 0x00, 0x00, 0x00, 0xFF }
  /// </code>
  /// </example>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static void _Increment(byte[] counter) {
    for (var i = 0; i < counter.Length; ++i) {
      var result = counter[i] + 1;
      counter[i] = (byte)result;
      if (result < 256)
        break;
    }
  }
}
