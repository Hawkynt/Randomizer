namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   ChaCha12 — 12-round variant of <see cref="ChaCha20"/> (Bernstein).
///   The recommended balance of speed and security margin for general
///   CSPRNG use; Rust's standard CSPRNG (rand_chacha) defaults to ChaCha12.
/// </summary>
public class ChaCha12() : ChaCha20(rounds: 12);
