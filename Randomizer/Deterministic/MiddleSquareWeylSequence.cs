namespace Randomizer;

public class MiddleSquareWeylSequence : IRandomNumberGenerator {

  private const ulong _WEYL_CONSTANT = 0xB5AD4ECEDA1CE2A9;
  private ulong _state;
  private ulong _weyl;

  public void Seed(ulong seed) => this._state = this._weyl = seed;

  public ulong Next() {
    this._state *= this._state;
    this._state += this._weyl += MiddleSquareWeylSequence._WEYL_CONSTANT;
    var register = this._state;
    var high = (register >> 32);
    var low = register << 32;
    return this._state = low | high;
  }

}
