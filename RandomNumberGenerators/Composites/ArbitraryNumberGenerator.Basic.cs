using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Composites;

public partial class ArbitraryNumberGenerator(IRandomNumberGenerator rng) : IRandomNumberGenerator {
  public void Seed(ulong seed) => rng.Seed(seed);
  public ulong Next() => rng.Next();
}
