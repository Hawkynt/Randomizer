using System;
using System.Security.Cryptography;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   HMAC-SHA-256 DRBG approximation (NIST SP 800-90A Rev. 1). Maintains a 256-bit key
///   K and a 256-bit state V; each Generate call computes V = HMAC(K, V), returns the
///   upper 64 bits, then runs an Update step with empty additional input to provide
///   backtracking resistance.
/// </summary>
/// <remarks>
///   Like <see cref="AesCtrDrbg"/>, this is a simplified, non-reseeding variant
///   intended for educational comparison; for production use, prefer a vetted library.
/// </remarks>
public sealed class HmacDrbg : IRandomNumberGenerator, IDisposable {
  private const int HASH_BYTES = 32; // SHA-256 output size

  private readonly byte[] _key = new byte[HASH_BYTES];
  private readonly byte[] _v = new byte[HASH_BYTES];

  public void Seed(ulong seed) {
    // NIST init: K := 0x00…, V := 0x01…, then Update(seed_material).
    Array.Fill(this._key, (byte)0x00);
    Array.Fill(this._v, (byte)0x01);

    var s = seed;
    var seedMaterial = new byte[HASH_BYTES];
    for (var i = 0; i < HASH_BYTES; i += 8)
      BitConverter.TryWriteBytes(seedMaterial.AsSpan(i, 8), SplitMix64.Next(ref s));

    this._Update(seedMaterial);
  }

  public ulong Next() {
    // V = HMAC(K, V); return upper 64 bits; then re-Update with empty data.
    var newV = HMACSHA256.HashData(this._key, this._v);
    Array.Copy(newV, this._v, HASH_BYTES);
    var result = BitConverter.ToUInt64(this._v, 0);
    this._Update(null);
    return result;
  }

  private void _Update(byte[]? providedData) {
    // K = HMAC(K, V || 0x00 || providedData)
    var buffer = new byte[HASH_BYTES + 1 + (providedData?.Length ?? 0)];
    Array.Copy(this._v, 0, buffer, 0, HASH_BYTES);
    buffer[HASH_BYTES] = 0x00;
    if (providedData != null)
      Array.Copy(providedData, 0, buffer, HASH_BYTES + 1, providedData.Length);
    var newKey = HMACSHA256.HashData(this._key, buffer);
    Array.Copy(newKey, this._key, HASH_BYTES);

    // V = HMAC(K, V)
    var newV = HMACSHA256.HashData(this._key, this._v);
    Array.Copy(newV, this._v, HASH_BYTES);

    if (providedData == null)
      return;

    // K = HMAC(K, V || 0x01 || providedData)
    buffer[HASH_BYTES] = 0x01;
    newKey = HMACSHA256.HashData(this._key, buffer);
    Array.Copy(newKey, this._key, HASH_BYTES);

    // V = HMAC(K, V)
    newV = HMACSHA256.HashData(this._key, this._v);
    Array.Copy(newV, this._v, HASH_BYTES);
  }

  public void Dispose() {
    Array.Clear(this._key);
    Array.Clear(this._v);
  }
}
