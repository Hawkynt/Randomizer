namespace Hawkynt.RandomNumberGenerators.Interfaces;

/// <summary>
/// Defines an interface for a random number generator with seeding capability.
/// </summary>
public interface IRandomNumberGenerator {

  /// <summary>
  /// Seeds the random number generator with the specified seed value.
  /// </summary>
  /// <param name="seed">The seed value to initialize the random number generator.</param>
  /// <remarks>You may not assume that the seed is the next random number to be generated.</remarks>
  void Seed(ulong seed);

  /// <summary>
  /// Generates the next random 64-bit unsigned integer.
  /// </summary>
  /// <returns>A randomly generated 64-bit unsigned integer.</returns>
  /// <remarks>The random numbers are assumed to be uniformly distributed.</remarks>
  ulong Next();

}
