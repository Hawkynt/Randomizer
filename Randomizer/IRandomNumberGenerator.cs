namespace Randomizer;

public interface IRandomNumberGenerator {
  void Seed(ulong seed);
  ulong Next();
}
