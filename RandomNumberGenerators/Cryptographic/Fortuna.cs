using System;
using System.Security.Cryptography;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   Fortuna-style CSPRNG (Schneier and Ferguson, 2003) — the successor to Yarrow.
///   Uses <b>32 entropy pools</b> and AES-256 in CTR mode for output. Reseeding is
///   triggered when pool 0 fills past a threshold; on the <i>n</i>-th reseed only
///   the pools <i>i</i> where 2^<i>i</i> divides <i>n</i> are mixed in, giving a
///   geometric schedule that frustrates any attempt to compromise the state by
///   poisoning a single pool.
/// </summary>
/// <remarks>
///   Educational implementation: as with the Yarrow class, no real entropy source
///   is wired in — entropy is simulated by stirring output back into pools in
///   round-robin fashion. Production use requires real entropy injection.
/// </remarks>
public sealed class Fortuna : IRandomNumberGenerator, IDisposable {
  private const int POOL_COUNT = 32;
  private const int BLOCK_BYTES = 16;
  private const int KEY_BYTES = 32;             // AES-256
  private const int RESEED_THRESHOLD = 64;      // pool 0 size required to trigger reseed

  private readonly Aes _aes = Aes.Create();
  private readonly SHA256[] _pools = new SHA256[POOL_COUNT];
  private byte[] _key = new byte[KEY_BYTES];
  private readonly byte[] _counter = new byte[BLOCK_BYTES];
  private int _pool0Bytes;
  private int _reseedCount;
  private int _nextPool;

  public Fortuna() {
    this._aes.Mode = CipherMode.ECB;
    this._aes.Padding = PaddingMode.None;
    this._aes.KeySize = 256;
    for (var i = 0; i < POOL_COUNT; ++i)
      this._pools[i] = SHA256.Create();
  }

  public void Seed(ulong seed) {
    var s = seed;
    for (var i = 0; i < KEY_BYTES; i += 8)
      BitConverter.TryWriteBytes(this._key.AsSpan(i, 8), SplitMix64.Next(ref s));
    this._aes.Key = this._key;
    Array.Clear(this._counter);
    this._pool0Bytes = 0;
    this._reseedCount = 0;
    this._nextPool = 0;

    // Reset each pool's accumulator.
    for (var i = 0; i < POOL_COUNT; ++i) {
      this._pools[i].Dispose();
      this._pools[i] = SHA256.Create();
    }
  }

  public ulong Next() {
    if (this._pool0Bytes >= RESEED_THRESHOLD)
      this._Reseed();

    _IncrementCounter(this._counter);
    var block = new byte[BLOCK_BYTES];
    using (var encryptor = this._aes.CreateEncryptor())
      encryptor.TransformBlock(this._counter, 0, BLOCK_BYTES, block, 0);

    // Stir output into round-robin pool (educational entropy substitute).
    this._pools[this._nextPool].TransformBlock(block, 0, BLOCK_BYTES, null, 0);
    if (this._nextPool == 0)
      this._pool0Bytes += BLOCK_BYTES;
    this._nextPool = (this._nextPool + 1) % POOL_COUNT;

    return BitConverter.ToUInt64(block, 0);
  }

  private void _Reseed() {
    ++this._reseedCount;

    // Hash all pools that participate in this reseed: pool i participates iff 2^i divides _reseedCount.
    using var combiner = SHA256.Create();
    combiner.TransformBlock(this._key, 0, this._key.Length, null, 0);
    for (var i = 0; i < POOL_COUNT; ++i) {
      if ((this._reseedCount & ((1 << i) - 1)) != 0)
        break; // 2^i does not divide _reseedCount
      this._pools[i].TransformFinalBlock(Array.Empty<byte>(), 0, 0);
      var poolHash = this._pools[i].Hash!;
      combiner.TransformBlock(poolHash, 0, poolHash.Length, null, 0);
      this._pools[i].Dispose();
      this._pools[i] = SHA256.Create();
    }
    combiner.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
    this._key = combiner.Hash!;
    this._aes.Key = this._key;
    this._pool0Bytes = 0;
  }

  private static void _IncrementCounter(byte[] counter) {
    for (var i = counter.Length - 1; i >= 0; --i)
      if (++counter[i] != 0)
        break;
  }

  public void Dispose() {
    this._aes.Dispose();
    foreach (var pool in this._pools)
      pool.Dispose();
    Array.Clear(this._key);
  }
}
