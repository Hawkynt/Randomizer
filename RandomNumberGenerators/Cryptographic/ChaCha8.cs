namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   ChaCha8 — 8-round variant of <see cref="ChaCha20"/> (Bernstein).
///   Roughly 2.5× faster than ChaCha20 with a reduced security margin;
///   used as the default PRNG in Go (since 1.22) and recommended for
///   non-security-critical applications where ChaCha's diffusion quality
///   matters but full-strength margin does not.
/// </summary>
public class ChaCha8() : ChaCha20(rounds: 8);
