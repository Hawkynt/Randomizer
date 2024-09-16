using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class SplitMix64 : IRandomNumberGenerator {
  private const ulong _GOLDEN_GAMMA = 0x9E3779B97F4A7C15;
  private ulong _state;

  public void Seed(ulong seed) => this._state = seed;

  public ulong Next() => Next(ref this._state);

  public static ulong Next(ref ulong z) {
    var local = z;
    local += _GOLDEN_GAMMA;
    local = (local ^ (local >> 30)) * 0xBF58476D1CE4E5B9;
    local = (local ^ (local >> 27)) * 0x94D049BB133111EB;
    local ^= local >> 31;
    z = local;
    return local;
  }
}
