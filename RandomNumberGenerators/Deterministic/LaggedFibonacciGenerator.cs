﻿using System;
using System.ComponentModel;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class LaggedFibonacciGenerator : IRandomNumberGenerator {

  public enum Mode {
    Additive,
    Subtractive,
    Multiplicative,
    Xor,
  }

  private readonly int _shortLag;
  private readonly int _longLag;
  private readonly Func<ulong, ulong, ulong> _operation;
  private readonly ulong[] _state;
  private int _index;

  public LaggedFibonacciGenerator(int size = 56, int shortLag = 0, int longLag = 21, Mode mode = Mode.Additive) : this(
    size,
    shortLag,
    longLag,
    mode switch {
      Mode.Additive => _Additive,
      Mode.Subtractive => _Subtractive,
      Mode.Multiplicative => _Multiplicative,
      Mode.Xor => _Xor,
      _ => throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(Mode))
    }
  ) { }

  private LaggedFibonacciGenerator(int size, int shortLag, int longLag, Func<ulong, ulong, ulong> operation) {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(shortLag, size);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(longLag, shortLag);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(longLag, size);
    ArgumentNullException.ThrowIfNull(operation);

    this._shortLag = shortLag;
    this._longLag = longLag;
    this._operation = operation;
    this._state = new ulong[size];
  }

  public void Seed(ulong seed) {
    for (var i = 0; i < this._state.Length; ++i)
      this._state[i] = SplitMix64.Next(ref seed);
  }

  public ulong Next() { // implicit mod 2^64
    var state = this._state;
    var length = state.Length;
    var index = this._index;

    var shortIndex = index - this._shortLag;
    if (shortIndex < 0)
      shortIndex += length;

    var longIndex = index - this._longLag;
    if (longIndex < 0)
      longIndex += length;

    var a = state[shortIndex];
    var b = state[longIndex];
    var result = this._operation(a, b);
    state[index++] = result;

    if (index >= length)
      index -= index;

    this._index = index;
    return result;
  }

  private static ulong _Additive(ulong a, ulong b) => a + b;
  private static ulong _Subtractive(ulong a, ulong b) => unchecked(a - b);
  private static ulong _Multiplicative(ulong a, ulong b) => a * b;
  private static ulong _Xor(ulong a, ulong b) => a ^ b;

}