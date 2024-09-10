using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;

public class ChaCha20 : IRandomNumberGenerator {

  // The number of rounds used in the ChaCha20 algorithm, as specified by RFC 7539 for enhanced security.
  // Default constants (from "expand 32-byte k")
  public ChaCha20(
    int rounds = 20, 
    uint constant1 = 0x61707865, 
    uint constant2 = 0x3320646e, 
    uint constant3 = 0x79622d32, 
    uint constant4 = 0x6b206574
  ) {
    ArgumentOutOfRangeException.ThrowIfLessThan(rounds, 1);
    this._rounds = rounds;
    this._constant1 = constant1;
    this._constant2 = constant2;
    this._constant3 = constant3;
    this._constant4 = constant4;
  }

  public ChaCha20(int rounds, Vector128<uint> constants):this(
    rounds, 
    constants.GetElement(0), 
    constants.GetElement(1), 
    constants.GetElement(2), 
    constants.GetElement(3)
  ) { }

  public ChaCha20(Vector128<uint> constants) : this(
    constant1: constants.GetElement(0),
    constant2: constants.GetElement(1),
    constant3: constants.GetElement(2),
    constant4: constants.GetElement(3)
  ) { }

  public ChaCha20(int rounds, ulong constants12, ulong constants34) : this(
    rounds,
    (uint)(constants12 >> 32),
    (uint)constants12,
    (uint)(constants34 >> 32),
    (uint)constants34
  ) { }

  public ChaCha20(ulong constants12, ulong constants34) : this(
    constant1: (uint)(constants12 >> 32),
    constant2: (uint)constants12,
    constant3: (uint)(constants34 >> 32),
    constant4: (uint)constants34
  ) { }

  // State array to hold the internal state of the ChaCha20 algorithm.
  // It's initialized with 16 32-bit words: 4 constant words, 8 key words, 1 counter, and 3 nonce words.
  private readonly uint[] state = new uint[16];
  private readonly int _rounds;
  private readonly uint _constant1;
  private readonly uint _constant2;
  private readonly uint _constant3;
  private readonly uint _constant4;
  
  // Constants for positioning the counter and nonce within the state array
  // COUNTER is used to track the block number being processed.
  private const int COUNTER = 12;
  private const int NONCE_0 = COUNTER + 1;
  private const int NONCE_1 = NONCE_0 + 1;
  private const int NONCE_2 = NONCE_1 + 1;
  private const int NONCE_3 = NONCE_2 + 1;

  public void Seed(ulong seed) {

    // Set the first 4 words to ChaCha20-specific constants.
    this.state[0] = this._constant1;
    this.state[1] = this._constant2;
    this.state[2] = this._constant3;
    this.state[3] = this._constant4;

    // Derive the key and nonce from the seed to fill the remaining 12 state elements.
    // This design choice ensures that the nonce is unique per seed, preventing nonce reuse across different sessions.
    for (var i = 4; i < this.state.Length; ++i) {
      var current = SplitMix64.Next(ref seed);
      this.state[i] = (uint)((current >> 32) ^ current);
    }

    // Initialize the block counter to 0, as required by the ChaCha20 algorithm.
    this.state[COUNTER] = 0;
  }

  public ulong Next() {
    var result = new uint[this.state.Length];
    ChaCha20Block(ref result);
    return ((ulong)result[0] << 32) | result[1];

    void ChaCha20Block(ref uint[] output) {

      // Copy the current state into the output buffer before applying the ChaCha20 rounds.
      for (var i = 0; i < this.state.Length; ++i)
        output[i] = this.state[i];

      for (var i = 0; i < this._rounds; i += 2) {

        // Column rounds
        QuarterRound(ref output[0], ref output[4], ref output[8], ref output[12]);
        QuarterRound(ref output[1], ref output[5], ref output[9], ref output[13]);
        QuarterRound(ref output[2], ref output[6], ref output[10], ref output[14]);
        QuarterRound(ref output[3], ref output[7], ref output[11], ref output[15]);

        // Diagonal rounds
        QuarterRound(ref output[0], ref output[5], ref output[10], ref output[15]);
        QuarterRound(ref output[1], ref output[6], ref output[11], ref output[12]);
        QuarterRound(ref output[2], ref output[7], ref output[8], ref output[13]);
        QuarterRound(ref output[3], ref output[4], ref output[9], ref output[14]);
      }

      for (var i = 0; i < this.state.Length; ++i)
        output[i] += this.state[i];

      // Increment the block counter.
      if (++this.state[COUNTER] != 0)
        return;

      // Handle counter overflow by incrementing the nonce, ensuring continuous unique state and extending the counter space.
      // This deviates from RFC 7539 where the nonce is fixed and counter wraps around.
      // Implication: Enables generation of a larger stream.
      if (++this.state[NONCE_0] == 0)
        if (++this.state[NONCE_1] == 0)
          if (++this.state[NONCE_2] == 0)
            ++this.state[NONCE_3];

      return;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      static void QuarterRound(ref uint a, ref uint b, ref uint c, ref uint d) {
        a += b;
        d ^= a;
        d = BitOperations.RotateLeft(d, 16);
        c += d;
        b ^= c;
        b = BitOperations.RotateLeft(b, 12);
        a += b;
        d ^= a;
        d = BitOperations.RotateLeft(d, 8);
        c += d;
        b ^= c;
        b = BitOperations.RotateLeft(b, 7);
      }

    }


  }

}
