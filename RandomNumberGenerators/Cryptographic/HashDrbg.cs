using System;
using System.Security.Cryptography;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   Hash DRBG (SHA-256 instantiation, NIST SP 800-90A Rev. 1). Maintains a 440-bit
///   secret state V and a 440-bit constant C; each Generate call repeatedly hashes V
///   to produce output bytes, then updates V via V = V + H(0x03 || V) + C +
///   reseed_counter to provide backtracking resistance.
/// </summary>
/// <remarks>
///   This completes the NIST SP 800-90A DRBG family in the package
///   (<see cref="AesCtrDrbg"/>, <see cref="HmacDrbg"/>, Hash DRBG). All three are
///   simplified educational implementations that omit reseeding from a live
///   entropy source and the personalization-string construction; for production
///   use, prefer a vetted DRBG library.
/// </remarks>
public sealed class HashDrbg : IRandomNumberGenerator, IDisposable {
  private const int SEED_BYTES = 55; // 440 bits, the SP 800-90A seedlen for SHA-256
  private const int HASH_BYTES = 32;

  private readonly byte[] _v = new byte[SEED_BYTES];
  private readonly byte[] _c = new byte[SEED_BYTES];
  private ulong _reseedCounter;

  public void Seed(ulong seed) {
    var s = seed;
    var material = new byte[SEED_BYTES];
    for (var i = 0; i + 8 <= SEED_BYTES; i += 8)
      BitConverter.TryWriteBytes(material.AsSpan(i, 8), SplitMix64.Next(ref s));
    // Trailing bytes (SEED_BYTES is not a multiple of 8).
    var remainder = SEED_BYTES % 8;
    if (remainder > 0) {
      Span<byte> tail = stackalloc byte[8];
      BitConverter.TryWriteBytes(tail, SplitMix64.Next(ref s));
      tail[..remainder].CopyTo(material.AsSpan(SEED_BYTES - remainder));
    }

    // V = Hash_df(seed_material || 0x00, seedlen)
    _HashDf(material, 0x00, this._v);
    // C = Hash_df(0x00 || V, seedlen)
    Span<byte> cInput = stackalloc byte[1 + SEED_BYTES];
    cInput[0] = 0x00;
    this._v.AsSpan().CopyTo(cInput[1..]);
    _HashDf(cInput.ToArray(), 0x00, this._c);

    this._reseedCounter = 1;
  }

  public ulong Next() {
    // Hashgen: produce one block of output by iteratively hashing V (a copy thereof).
    var data = (byte[])this._v.Clone();
    var block = SHA256.HashData(data);
    var result = BitConverter.ToUInt64(block, 0);

    // Update step (V = V + H(0x03 || V) + C + reseed_counter, mod 2^seedlen)
    Span<byte> hashInput = stackalloc byte[1 + SEED_BYTES];
    hashInput[0] = 0x03;
    this._v.AsSpan().CopyTo(hashInput[1..]);
    var h = SHA256.HashData(hashInput.ToArray());

    _AddInto(this._v, h);
    _AddInto(this._v, this._c);
    var counterBytes = BitConverter.GetBytes(this._reseedCounter);
    _AddInto(this._v, counterBytes);

    ++this._reseedCounter;
    return result;
  }

  private static void _HashDf(byte[] inputString, byte addByte, byte[] output) {
    var bitsToReturn = output.Length * 8;
    var len = (bitsToReturn + 255) / 256; // number of hash invocations
    var pos = 0;
    var counter = (byte)1;
    var working = new byte[output.Length];
    for (var i = 0; i < len; ++i) {
      var prefix = new byte[5 + 1 + inputString.Length];
      prefix[0] = counter;
      // big-endian bitsToReturn (32-bit)
      prefix[1] = (byte)(bitsToReturn >> 24);
      prefix[2] = (byte)(bitsToReturn >> 16);
      prefix[3] = (byte)(bitsToReturn >> 8);
      prefix[4] = (byte)bitsToReturn;
      prefix[5] = addByte;
      Array.Copy(inputString, 0, prefix, 6, inputString.Length);
      var hash = SHA256.HashData(prefix);
      var copyLen = Math.Min(HASH_BYTES, working.Length - pos);
      Array.Copy(hash, 0, working, pos, copyLen);
      pos += copyLen;
      ++counter;
    }
    Array.Copy(working, output, output.Length);
  }

  private static void _AddInto(byte[] target, byte[] addend) {
    // target = (target + addend) mod 2^(8*target.Length)
    var carry = 0;
    for (var i = 0; i < target.Length; ++i) {
      var a = target[i];
      var b = i < addend.Length ? addend[i] : 0;
      var sum = a + b + carry;
      target[i] = (byte)sum;
      carry = sum >> 8;
    }
  }

  public void Dispose() {
    Array.Clear(this._v);
    Array.Clear(this._c);
  }
}
