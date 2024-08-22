using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Security.Cryptography;
using Hawkynt.RandomNumberGenerators.Deterministic;

// ReSharper disable UnusedMember.Global

namespace Hawkynt.RandomNumberGenerators.Composites;

partial class ArbitraryNumberGenerator {
  
  public UInt128 Concat128() => (UInt128)rng.Next() << 64 | rng.Next();
  
  public Vector256<ulong> Concat256() => Vector256.Create(rng.Next(), rng.Next(), rng.Next(), rng.Next());
  
  public Vector512<ulong> Concat512() => Vector512.Create(
    rng.Next(), rng.Next(), rng.Next(), rng.Next(),
    rng.Next(), rng.Next(), rng.Next(), rng.Next()
  );

  public IEnumerable<byte> ConcatGenerator() {
    for (;;) {
      var random = new SliceUnion(rng.Next());
      yield return random.R8_0;
      yield return random.R8_1;
      yield return random.R8_2;
      yield return random.R8_3;
      yield return random.R8_4;
      yield return random.R8_5;
      yield return random.R8_6;
      yield return random.R8_7;
    }
    // ReSharper disable once IteratorNeverReturns
  }

  public unsafe byte[] ConcatGenerator(int count) {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

    var result = new byte[count];

    fixed (byte* pointer = &result[0]) {
      var index = pointer;
      var random = rng.Next();

      // full rounds
      while (count >= 8) {
        *(ulong*)index = random;
        random = rng.Next();
        index += 8;
        count -= 8;
      }

      // remaining bytes
      switch (count) {
        case 0: break;
        case 1:
          *index = (byte)random;
          break;
        case 2:
          *(ushort*)index = (ushort)random;
          break;
        case 3:
          *(ushort*)index = (ushort)random;
          index[2] = (byte)(random >> 16);
          break;
        case 4:
          *(uint*)index = (uint)random;
          break;
        case 5:
          *(uint*)index = (uint)random;
          index[4] = (byte)(random >> 32);
          break;
        case 6:
          *(uint*)index = (uint)random;
          ((ushort*)index)[2] = (ushort)(random >> 32);
          break;
        case 7:
          *(uint*)index = (uint)random;
          ((ushort*)index)[2] = (ushort)(random >> 32);
          index[6] = (byte)(random >> 48);
          break;
      }
    }

    return result;
  }

  public UInt128 SplitMix128() {
    var random = rng.Next();
    return (UInt128)random << 64 | SplitMix64.Next(ref random);
  }

  public Vector256<ulong> SplitMix256() {
    var random = rng.Next();
    return Vector256.Create(random, SplitMix64.Next(ref random), SplitMix64.Next(ref random), SplitMix64.Next(ref random));
  }

  public Vector512<ulong> SplitMix512() {
    var random = rng.Next();
    return Vector512.Create(
      random, SplitMix64.Next(ref random), SplitMix64.Next(ref random), SplitMix64.Next(ref random),
      SplitMix64.Next(ref random), SplitMix64.Next(ref random), SplitMix64.Next(ref random), SplitMix64.Next(ref random)
    );
  }

  public UInt128 SpreadBits128(UInt128 mask) {
    ArgumentOutOfRangeException.ThrowIfZero(mask);
    var bitCount = _PopCount(mask);
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, 64);

    var random = rng.Next();
    var result = UInt128.Zero;
    for (var i = 0; i < bitCount; ++i) {
      UInt128 bit = random & 1;
      random >>= 1;

      var upperZero= BitOperations.TrailingZeroCount((ulong)(mask >> 64));
      var nextPosition = BitOperations.TrailingZeroCount((ulong)mask);
      if (nextPosition == 64)
        nextPosition += upperZero;

      result |= bit << nextPosition;
      mask &= ~(UInt128.One << nextPosition);
    }
    
    return result;
  }

  public Vector256<ulong> SpreadBits256(Vector256<ulong> mask) {
    var bitCount = _PopCount(mask);
    ArgumentOutOfRangeException.ThrowIfZero(bitCount, nameof(mask));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, 64);

    var random = rng.Next();
    var result = Vector256<ulong>.Zero;
    for (var i = 0; i < bitCount; ++i) {
      var bit = random & 1;
      random >>= 1;

      var value0 = mask.GetElement(0);
      var value1 = mask.GetElement(1);
      var value2 = mask.GetElement(2);
      var value3 = mask.GetElement(3);

      var zeroes0 = BitOperations.TrailingZeroCount(value0);
      var zeroes1 = BitOperations.TrailingZeroCount(value1);
      var zeroes2 = BitOperations.TrailingZeroCount(value2);
      var zeroes3 = BitOperations.TrailingZeroCount(value3);

      var nextPosition = zeroes0;
      if (zeroes0 == 64) {
        nextPosition += zeroes1;
        if (zeroes1 == 64) {
          nextPosition += zeroes2;
          if (zeroes2 == 64)
            nextPosition += zeroes3;
        }
      }

      var elementIndex = nextPosition >> 6;
      var intraElementIndex = nextPosition & 63;
      var element = result.GetElement(elementIndex);
      element |= bit << intraElementIndex;
      result = result.WithElement(elementIndex, element);

      mask = mask.WithElement(elementIndex, mask.GetElement(elementIndex) & ~(1UL << intraElementIndex));
    }

    return result;
  }

  public Vector512<ulong> SpreadBits512(Vector512<ulong> mask) {
    var bitCount = _PopCount(mask);
    ArgumentOutOfRangeException.ThrowIfZero(bitCount, nameof(mask));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount, 64);

    var random = rng.Next();
    var result = Vector512<ulong>.Zero;
    for (var i = 0; i < bitCount; ++i) {
      var bit = random & 1;
      random >>= 1;

      var nextPosition = 0;
      for (var j = 0; j < 8; ++j) {
        var currentZeroes = BitOperations.TrailingZeroCount(mask.GetElement(j));
        nextPosition += currentZeroes;
        if (currentZeroes != 64)
          break;
      }

      var elementIndex = nextPosition >> 6;
      var intraElementIndex = nextPosition & 63;
      var element = result.GetElement(elementIndex);
      element |= bit << intraElementIndex;
      result = result.WithElement(elementIndex, element);

      mask = mask.WithElement(elementIndex, mask.GetElement(elementIndex) & ~(1UL << intraElementIndex));
    }

    return result;
  }

  public IEnumerable<byte> FeistelGenerator() {
    var state = rng.Next();
    var key = rng.Next();
    var counter = rng.Next();

    var counterIndex = 0;
    for (;;) {
      var roundBits = (int)(counter >> counterIndex) & 0b1111;
      counterIndex += 4;
      if (counterIndex >= 64) {
        (counter, state) = (state, counter);
        counterIndex = 0;
      }

      ++roundBits;
      for (var i = 0; i < roundBits; ++i) {
        DoFeistelRound(ref state, key);
        DoFeistelRound(ref state, key);
        (state, key) = (key, state);
      }

      var result = new SliceUnion(state);
      yield return result.R8_0;
      yield return result.R8_1;
      yield return result.R8_2;
      yield return result.R8_3;
      yield return result.R8_4;
      yield return result.R8_5;
      yield return result.R8_6;
      yield return result.R8_7;
    }
    
    void DoFeistelRound(ref ulong plainText, ulong roundKey) {
      var left = (uint)plainText;
      var right = (uint)(plainText>>32);
      left ^= RoundFunction(right, roundKey);
      (left, right) = (right, left);
      plainText = left | (ulong)right << 32;
    }

    uint RoundFunction(uint right,ulong roundKey) {
      var result=BitOperations.RotateLeft(right, 3);
      result ^= (uint)roundKey;
      result = BitOperations.RotateRight(result, 17);
      result ^= (uint)(roundKey >> 32);
      return result;
    }
    // ReSharper disable once IteratorNeverReturns
  }

  public IEnumerable<byte> HashGenerator<THash>() where THash : HashAlgorithm, new() {
    using var instance=new THash();
      return this.HashGenerator(instance);
  }

  public IEnumerable<byte> HashGenerator(HashAlgorithm instance) {
    ArgumentNullException.ThrowIfNull(instance);

    var entropyBitsNeeded = instance.HashSize;
    var entropyBytesNeeded = entropyBitsNeeded >> 3;
    
    // Generate the initial salt using RNG
    var salt = this.ConcatGenerator(entropyBytesNeeded);
    
    // Initialize the counter
    var counter = new byte[entropyBytesNeeded];

    for (;;) {

      // Combine the salt and counter using XOR
      var plainData = salt.Zip(counter, (s, c) => (byte)(s ^ c)).ToArray();

      // Generate the hash
      var hash = instance.ComputeHash(plainData);
      
      // Yield each byte of the hash as part of the random stream
      foreach (var entry in hash)
        yield return entry;

      _Increment(counter);
    }
    // ReSharper disable once IteratorNeverReturns
  }

  public IEnumerable<byte> CipherGenerator<TCipher>() where TCipher : SymmetricAlgorithm, new() {
    using var instance = new TCipher();
    return this.CipherGenerator(instance);
  }

  public IEnumerable<byte> CipherGenerator(SymmetricAlgorithm instance) {
    instance.Mode = CipherMode.ECB; // CTR mode is simulated with ECB
    instance.Padding = PaddingMode.None;

    // Generate a random key and initialization vector (IV)
    var key = this.ConcatGenerator(instance.KeySize >> 3);

    var blockSizeInBytes = instance.BlockSize >> 3;
    var iv = this.ConcatGenerator(blockSizeInBytes);
    
    instance.Key = key;
    instance.IV = iv;

    // Initialize the counter
    var counter = new byte[blockSizeInBytes];

    var cipherText = new byte[blockSizeInBytes];
    using var encryptor = instance.CreateEncryptor();
    for (;;) {
      
      // Encrypt the counter block
      encryptor.TransformBlock(counter, 0, blockSizeInBytes, cipherText, 0);

      // Yield each byte from the encrypted block as random output
      foreach (var value in cipherText)
        yield return value;

      // Increment the counter
      _Increment(counter);
    }
    // ReSharper disable once IteratorNeverReturns
  }

}