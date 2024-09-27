using System;
using System.Runtime.CompilerServices;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators;

internal static class Utils {

  public static Func<ulong, ulong, ulong> GetOperation(CombinationMode mode) => mode switch {
    CombinationMode.Additive => MathEx.Add,
    CombinationMode.Subtractive => MathEx.Subtract,
    CombinationMode.Multiplicative => MathEx.MultipliedWith,
    CombinationMode.Xor => MathEx.Xor,
    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
  };
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsNotSet(this ulong modulo) => modulo <= 1;

}