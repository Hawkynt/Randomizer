using System;
using System.Security.Cryptography;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   Yarrow-160-style CSPRNG (Schneier, Kelsey, Ferguson, 1999), updated to use
///   AES-256 and SHA-256. The design keeps two entropy pools — a fast pool that
///   reseeds the generator key frequently, and a slow pool that reseeds rarely
///   but with high-confidence entropy. When the fast pool reaches a threshold we
///   mix it into the key via SHA-256 to provide post-compromise recovery.
/// </summary>
/// <remarks>
///   Educational implementation: since we have no real entropy source here,
///   the pools are pre-filled from SplitMix64 at seed time and topped up from
///   the generator's own output (which is exactly what NOT to do in production —
///   real Yarrow requires independent entropy sources).
/// </remarks>
public sealed class Yarrow : IRandomNumberGenerator, IDisposable {
  private const int BLOCK_BYTES = 16;
  private const int KEY_BYTES = 32;             // AES-256
  private const int FAST_POOL_THRESHOLD = 64;   // bytes accumulated before reseeding from fast pool
  private const int OUTPUT_RESEED_LIMIT = 1024; // hard cap on outputs between reseeds

  private readonly Aes _aes = Aes.Create();
  private byte[] _key = new byte[KEY_BYTES];
  private readonly byte[] _counter = new byte[BLOCK_BYTES];
  private readonly byte[] _fastPool = new byte[FAST_POOL_THRESHOLD * 2];
  private int _fastPoolFill;
  private int _outputsSinceReseed;

  public Yarrow() {
    this._aes.Mode = CipherMode.ECB;
    this._aes.Padding = PaddingMode.None;
    this._aes.KeySize = 256;
  }

  public void Seed(ulong seed) {
    var s = seed;
    for (var i = 0; i < KEY_BYTES; i += 8)
      BitConverter.TryWriteBytes(this._key.AsSpan(i, 8), SplitMix64.Next(ref s));
    this._aes.Key = this._key;
    Array.Clear(this._counter);
    Array.Clear(this._fastPool);
    this._fastPoolFill = 0;
    this._outputsSinceReseed = 0;
  }

  public ulong Next() {
    if (this._fastPoolFill >= FAST_POOL_THRESHOLD || this._outputsSinceReseed >= OUTPUT_RESEED_LIMIT)
      this._Reseed();

    _IncrementCounter(this._counter);
    var block = new byte[BLOCK_BYTES];
    using (var encryptor = this._aes.CreateEncryptor())
      encryptor.TransformBlock(this._counter, 0, BLOCK_BYTES, block, 0);

    // Stir the output back into the fast pool (educational — real Yarrow uses
    // independent entropy sources here).
    var copyLen = Math.Min(BLOCK_BYTES, this._fastPool.Length - this._fastPoolFill);
    Array.Copy(block, 0, this._fastPool, this._fastPoolFill, copyLen);
    this._fastPoolFill += copyLen;
    ++this._outputsSinceReseed;

    return BitConverter.ToUInt64(block, 0);
  }

  private void _Reseed() {
    // Mix fast pool into the key via SHA-256.
    var input = new byte[KEY_BYTES + this._fastPoolFill];
    Array.Copy(this._key, 0, input, 0, KEY_BYTES);
    Array.Copy(this._fastPool, 0, input, KEY_BYTES, this._fastPoolFill);
    this._key = SHA256.HashData(input);
    this._aes.Key = this._key;
    Array.Clear(this._fastPool);
    this._fastPoolFill = 0;
    this._outputsSinceReseed = 0;
  }

  private static void _IncrementCounter(byte[] counter) {
    for (var i = counter.Length - 1; i >= 0; --i)
      if (++counter[i] != 0)
        break;
  }

  public void Dispose() {
    this._aes.Dispose();
    Array.Clear(this._key);
  }
}
