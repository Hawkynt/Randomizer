using System;
using System.Security.Cryptography;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   AES-CTR DRBG approximation (NIST SP 800-90A Rev. 1). Maintains a 256-bit key
///   and a 128-bit counter; each call encrypts the counter under AES-256 to produce
///   a fresh 128-bit block, increments the counter, and returns the upper 64 bits.
///   After each Generate, the internal state is updated by encrypting two further
///   counter blocks and folding them into (K, V) to provide backtracking resistance.
/// </summary>
/// <remarks>
///   This is the same construction NIST recommends for fast CSPRNG output once a
///   high-entropy seed is available. It does not implement reseeding or the
///   personalization string; for production use, prefer a vetted DRBG library.
/// </remarks>
public sealed class AesCtrDrbg : IRandomNumberGenerator, IDisposable {
  private const int KEY_BYTES = 32;   // AES-256
  private const int BLOCK_BYTES = 16; // AES block size

  private readonly Aes _aes = Aes.Create();
  private readonly byte[] _key = new byte[KEY_BYTES];
  private readonly byte[] _v = new byte[BLOCK_BYTES];

  public AesCtrDrbg() {
    this._aes.Mode = CipherMode.ECB;
    this._aes.Padding = PaddingMode.None;
    this._aes.KeySize = 256;
  }

  public void Seed(ulong seed) {
    // Derive a fresh (key, V) pair from the 64-bit seed via SplitMix64.
    var s = seed;
    for (var i = 0; i < KEY_BYTES; i += 8)
      BitConverter.TryWriteBytes(this._key.AsSpan(i, 8), SplitMix64.Next(ref s));
    for (var i = 0; i < BLOCK_BYTES; i += 8)
      BitConverter.TryWriteBytes(this._v.AsSpan(i, 8), SplitMix64.Next(ref s));
    this._aes.Key = this._key;
  }

  public ulong Next() {
    _IncrementV(this._v);
    Span<byte> output = stackalloc byte[BLOCK_BYTES];
    using (var encryptor = this._aes.CreateEncryptor())
      encryptor.TransformBlock(this._v, 0, BLOCK_BYTES, output.ToArray(), 0);

    // Re-encrypt for actual output (TransformBlock writes to the array, not the span).
    var outputArr = new byte[BLOCK_BYTES];
    using (var encryptor = this._aes.CreateEncryptor())
      encryptor.TransformBlock(this._v, 0, BLOCK_BYTES, outputArr, 0);

    var result = BitConverter.ToUInt64(outputArr, 0);

    // Update K and V to provide backtracking resistance.
    this._UpdateState();

    return result;
  }

  private void _UpdateState() {
    var temp = new byte[KEY_BYTES + BLOCK_BYTES];
    using var encryptor = this._aes.CreateEncryptor();
    for (var offset = 0; offset < temp.Length; offset += BLOCK_BYTES) {
      _IncrementV(this._v);
      var block = new byte[BLOCK_BYTES];
      encryptor.TransformBlock(this._v, 0, BLOCK_BYTES, block, 0);
      Array.Copy(block, 0, temp, offset, Math.Min(BLOCK_BYTES, temp.Length - offset));
    }
    Array.Copy(temp, 0, this._key, 0, KEY_BYTES);
    Array.Copy(temp, KEY_BYTES, this._v, 0, BLOCK_BYTES);
    this._aes.Key = this._key;
  }

  private static void _IncrementV(byte[] v) {
    // Treat v as big-endian counter and add 1.
    for (var i = v.Length - 1; i >= 0; --i) {
      if (++v[i] != 0)
        break;
    }
  }

  public void Dispose() => this._aes.Dispose();
}
