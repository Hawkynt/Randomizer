namespace Randomizer.Deterministic;

public class MultiplicativeLinearCongruentialGenerator : IRandomNumberGenerator {

  private const ulong _MULTIPLIER = 6364136223846793005;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed == 0 ? 1 : seed;

  public ulong Next() => this._state *= MultiplicativeLinearCongruentialGenerator._MULTIPLIER; // implicit mod 2^64

}
