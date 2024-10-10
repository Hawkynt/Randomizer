using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Hawkynt.RandomNumberGenerators.Composites;

partial class ArbitraryNumberGenerator {
  
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
  private static byte _PopCount(UInt128 mask) => (byte)(((ulong)mask).CountSetBits() + ((ulong)(mask >> 64)).CountSetBits());
  
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
    var pc0 = mask.GetElement(0).CountSetBits();
    var pc1 = mask.GetElement(1).CountSetBits();
    var pc2 = mask.GetElement(2).CountSetBits();
    var pc3 = mask.GetElement(3).CountSetBits();

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
