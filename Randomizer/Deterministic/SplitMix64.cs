namespace Randomizer.Deterministic;

public class SplitMix64 : IRandomNumberGenerator {

  private const ulong _GOLDEN_GAMMA = 0x9E3779B97F4A7C15;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() => Next(ref this._state);

  public static ulong Next(ref ulong z) {
    z += _GOLDEN_GAMMA;
    z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
    z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
    z ^= z >> 31;
    return z;
  }

}
