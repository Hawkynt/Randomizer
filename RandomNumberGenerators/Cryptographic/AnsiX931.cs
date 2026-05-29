using System;
using System.Security.Cryptography;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   ANSI X9.31 — the AES update of the older ANSI X9.17 standard (1985).
///   Maintains a secret key K and an internal state V; each step encrypts a
///   counter substitute (originally date/time), XORs that with V, encrypts
///   again to produce the output, then re-encrypts to advance V.
/// </summary>
/// <remarks>
///   Uses AES-128 instead of the original DES; DES has been broken for decades.
///   Without a real date/time / entropy source, we drive D with a monotonic counter.
/// </remarks>
public sealed class AnsiX931 : IRandomNumberGenerator, IDisposable {
  private const int BLOCK_BYTES = 16;
  private const int KEY_BYTES = 16; // AES-128

  private readonly Aes _aes = Aes.Create();
  private byte[] _v = new byte[BLOCK_BYTES];
  private ulong _counter;

  public AnsiX931() {
    this._aes.Mode = CipherMode.ECB;
    this._aes.Padding = PaddingMode.None;
    this._aes.KeySize = 128;
  }

  public void Seed(ulong seed) {
    var s = seed;
    var key = new byte[KEY_BYTES];
    for (var i = 0; i < KEY_BYTES; i += 8)
      BitConverter.TryWriteBytes(key.AsSpan(i, 8), SplitMix64.Next(ref s));
    this._aes.Key = key;

    for (var i = 0; i < BLOCK_BYTES; i += 8)
      BitConverter.TryWriteBytes(this._v.AsSpan(i, 8), SplitMix64.Next(ref s));

    this._counter = SplitMix64.Next(ref s);
  }

  public ulong Next() {
    // D substitute: a 128-bit big-endian block derived from the monotonic counter.
    var d = new byte[BLOCK_BYTES];
    BitConverter.TryWriteBytes(d.AsSpan(0, 8), this._counter++);

    using var encryptor = this._aes.CreateEncryptor();

    // I = E(K, D)
    var i = new byte[BLOCK_BYTES];
    encryptor.TransformBlock(d, 0, BLOCK_BYTES, i, 0);

    // R = E(K, I XOR V)  -- this is the output
    var iv = _Xor(i, this._v);
    var r = new byte[BLOCK_BYTES];
    encryptor.TransformBlock(iv, 0, BLOCK_BYTES, r, 0);

    // V = E(K, R XOR I)
    var ri = _Xor(r, i);
    encryptor.TransformBlock(ri, 0, BLOCK_BYTES, this._v, 0);

    return BitConverter.ToUInt64(r, 0);
  }

  private static byte[] _Xor(byte[] a, byte[] b) {
    var result = new byte[a.Length];
    for (var i = 0; i < a.Length; ++i)
      result[i] = (byte)(a[i] ^ b[i]);
    return result;
  }

  public void Dispose() => this._aes.Dispose();
}
