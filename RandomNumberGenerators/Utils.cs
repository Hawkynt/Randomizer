using System;
using System.Runtime.CompilerServices;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators;

internal static class Utils {

  public static Func<ulong, ulong, ulong> GetOperation(CombinationMode mode) => mode switch {
    CombinationMode.Additive => _Additive,
    CombinationMode.Subtractive => _Subtractive,
    CombinationMode.Multiplicative => MathEx.MultipliedWith,
    CombinationMode.Xor => MathEx.Xor,
    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
  };

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static ulong _Additive(ulong a, ulong b) => unchecked(a + b);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static ulong _Subtractive(ulong a, ulong b) => unchecked(a - b);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsNotSet(this ulong modulo) => modulo <= 1;

}