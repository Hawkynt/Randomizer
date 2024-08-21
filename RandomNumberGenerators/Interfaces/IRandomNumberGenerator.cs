namespace Hawkynt.RandomNumberGenerators.Interfaces;

public interface IRandomNumberGenerator {
  void Seed(ulong seed);
  ulong Next();
}
