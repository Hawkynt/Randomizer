using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Hawkynt.RandomNumberGenerators.Composites;

partial class ArbitraryNumberGenerator {
  /// <summary>
  /// Extracts bits from the specified <paramref name="value"/> according to the bit positions set in the <paramref name="mask"/>.
  /// </summary>
  /// <param name="value">The <see cref="ulong"/> value from which bits are extracted.</param>
  /// <param name="mask">The <see cref="ulong"/> mask that specifies which bits to extract.</param>
  /// <returns>
  /// A <see cref="ulong"/> value containing the bits extracted from <paramref name="value"/> and positioned contiguously
  /// based on the <paramref name="mask"/>.
  /// </returns>
  /// <remarks>
  /// If the CPU supports the BMI2 instruction set, the PEXT (Parallel Bits Extract) instruction is used for optimal performance.
  /// If not, a manual bit-by-bit extraction is performed.
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
}
