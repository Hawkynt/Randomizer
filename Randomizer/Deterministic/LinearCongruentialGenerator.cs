namespace Randomizer.Deterministic;

public class LinearCongruentialGenerator : IRandomNumberGenerator {

  private const ulong _MULTIPLIER = 6364136223846793005;
  private const ulong _INCREMENT = 1442695040888963407;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() => this._state = (LinearCongruentialGenerator._MULTIPLIER * this._state + LinearCongruentialGenerator._INCREMENT); // implicit mod 2^64

}
