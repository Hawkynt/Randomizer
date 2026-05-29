namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   Salsa20/8 — 8-round variant of <see cref="Salsa20"/> (Bernstein).
///   The fastest member of the Salsa family at the cost of the narrowest
///   security margin; later cryptanalysis has progressively reduced the
///   gap to a practical attack, so this variant is recommended only for
///   non-cryptographic use such as Monte Carlo simulation.
/// </summary>
public class Salsa8() : Salsa20(rounds: 8);
