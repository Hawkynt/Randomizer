namespace Randomizer;

public class XorShift : IRandomNumberGenerator {

  private ulong _state;

  public void Seed(ulong seed) => this._state = seed == 0 ? 1 : seed;

  public ulong Next() {
    var s = this._state;
    s ^= s << 7;
    s ^= s >> 9;
    return this._state = s;
  }

}
