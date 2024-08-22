using System.Runtime.InteropServices;

namespace Hawkynt.RandomNumberGenerators.Composites;

partial class ArbitraryNumberGenerator {
  /// <summary>
  /// A structure used to extract <see cref="uint"/>, <see cref="ushort"/>, and <see cref="byte"/> values directly from a <see cref="ulong"/> 
  /// without performing manual bit shifts and masks. This approach is typically faster, although the compiler (as of 2024) generates
  /// unnecessary stack copies.
  /// </summary>
  /// <param name="value">The <see cref="ulong"/> value from which to extract parts.</param>
  [StructLayout(LayoutKind.Explicit)]
  private readonly struct SliceUnion(ulong value) {

    [FieldOffset(0)]
    public readonly ulong Value = value;

    [FieldOffset(0)]
    public readonly uint R32_0;
    [FieldOffset(4)]
    public readonly uint R32_1;

    [FieldOffset(0)]
    public readonly ushort R16_0;
    [FieldOffset(2)]
    public readonly ushort R16_1;
    [FieldOffset(4)]
    public readonly ushort R16_2;
    [FieldOffset(6)]
    public readonly ushort R16_3;

    [FieldOffset(0)]
    public readonly byte R8_0;
    [FieldOffset(1)]
    public readonly byte R8_1;
    [FieldOffset(2)]
    public readonly byte R8_2;
    [FieldOffset(3)]
    public readonly byte R8_3;
    [FieldOffset(4)]
    public readonly byte R8_4;
    [FieldOffset(5)]
    public readonly byte R8_5;
    [FieldOffset(6)]
    public readonly byte R8_6;
    [FieldOffset(7)]
    public readonly byte R8_7;
    [FieldOffset(8)]
    public readonly byte R8_8;

  }
}
