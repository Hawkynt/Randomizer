namespace Hawkynt.RandomNumberGenerators.Cryptographic;

/// <summary>
///   Salsa20/12 — 12-round variant of <see cref="Salsa20"/> (Bernstein).
///   The eSTREAM portfolio member: chosen as the "middle" speed-security
///   trade-off in the Salsa family.
/// </summary>
public class Salsa12() : Salsa20(rounds: 12);
