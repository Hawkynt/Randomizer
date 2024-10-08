﻿using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class PermutedCongruentialGenerator : IRandomNumberGenerator {
  private UInt128 _state;

  private readonly UInt128 MULTIPLIER = UInt128.Parse("110282366920938463463374607431768211483", NumberStyles.Integer, CultureInfo.InvariantCulture);
  private readonly UInt128 INCREMENT = 1442695040888963407UL;

  public void Seed(ulong seed) => this._state = ((UInt128)seed << 64) | ~seed;

  public ulong Next() {
    var state=this._state;
    state = state * MULTIPLIER + INCREMENT;
    this._state = state;

    return Permute(state);

    // Apply RXS-M-XS permutation
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong Permute(UInt128 state) {
      var count = (int)(state >> 122);
      state ^= state >> (5 + count);
      state *= 12605985483714917081UL;
      state ^= state >> 43;

      return (ulong)state;
    }
  }
}
