using System.Numerics;
using System.Runtime.CompilerServices;
using Hawkynt.RandomNumberGenerators.Deterministic;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Cryptographic;
  
public class ChaCha20 : IRandomNumberGenerator {
  
  // The number of rounds used in the ChaCha20 algorithm, as specified by RFC 7539 for enhanced security.
  private const int ROUNDS = 20;

  // State array to hold the internal state of the ChaCha20 algorithm.
  // It's initialized with 16 32-bit words: 4 constant words, 8 key words, 1 counter, and 3 nonce words.
  private readonly uint[] state=new uint[16];
  
  // Constants for positioning the counter and nonce within the state array
  // COUNTER is used to track the block number being processed.
  private const int COUNTER = 12;
  private const int NONCE_0 = COUNTER + 1;
  private const int NONCE_1 = NONCE_0 + 1;
  private const int NONCE_2 = NONCE_1 + 1;
  private const int NONCE_3 = NONCE_2 + 1;

  public void Seed(ulong seed) {

    // Set the first 4 words to ChaCha20-specific constants (from "expand 32-byte k").
    this.state[0] = 0x61707865;
    this.state[1] = 0x3320646e;
    this.state[2] = 0x79622d32;
    this.state[3] = 0x6b206574;

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

      for (var i = 0; i < ROUNDS; i += 2) {

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


