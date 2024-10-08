﻿using System;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class ComplementaryMultiplyWithCarry : IRandomNumberGenerator {
  private static readonly UInt128 A = 6364136223846793005UL;
  private const int R = 4096;
  private readonly ulong[] _state = new ulong[R];
  private ulong _carry;
  private int _index = R - 1;

  public void Seed(ulong seed) {
    for (var i = 0; i < R; ++i)
      this._state[i] = SplitMix64.Next(ref seed);

    this._carry = SplitMix64.Next(ref seed);
  }

  public ulong Next() {
    // implicit mod 2^64
    var state = this._state;
    var index = (this._index + 1) % R;
    
    var t = A * state[index] + this._carry;

    this._carry = (ulong)(t >> 64);
    state[index] = (ulong)t;

    this._index = index;
    return ulong.MaxValue - state[index];
  }
}
