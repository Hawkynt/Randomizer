namespace Randomizer.Deterministic;
public class MersenneTwister : IRandomNumberGenerator {
  private const int N = 624;
  private const int PERIOD = 397;
  private const uint MATRIX_A = 0x9908B0DF;
  private const uint UPPER_MASK = 0x80000000;
  private const uint LOWER_MASK = 0x7FFFFFFF;
  private const uint _TEMPERING_MASK_B = 0x9D2C5680;
  private const uint _TEMPERING_MASK_C = 0xEFC60000;

  private readonly uint[] _state = new uint[MersenneTwister.N];
  private int _index = MersenneTwister.N + 1;
  private static readonly uint[] _MAG01 = [0, MersenneTwister.MATRIX_A];

  public void Seed(ulong seed) {
    seed ^= seed >> 32;
    this._state[0] = (uint)seed;
    for (this._index = 1; this._index < MersenneTwister.N; ++this._index)
      this._state[this._index] = 1812433253 * (this._state[this._index - 1] ^ (this._state[this._index - 1] >> 30)) + (uint)this._index;
  }

  public ulong Next() {
    return (ulong)Next32() << 32 | Next32();
    
    uint Next32() {
      if (this._index >= MersenneTwister.N) {
        int i;

        for (i = 0; i < MersenneTwister.N - MersenneTwister.PERIOD; ++i) {
          var y = (this._state[i] & MersenneTwister.UPPER_MASK) | (this._state[i + 1] & MersenneTwister.LOWER_MASK);
          this._state[i] = this._state[i + MersenneTwister.PERIOD] ^ (y >> 1) ^ MersenneTwister._MAG01[y & 1];
        }

        for (; i < MersenneTwister.N - 1; ++i) {
          var y = (this._state[i] & MersenneTwister.UPPER_MASK) | (this._state[i + 1] & MersenneTwister.LOWER_MASK);
          this._state[i] = this._state[i + (MersenneTwister.PERIOD - MersenneTwister.N)] ^ (y >> 1) ^ MersenneTwister._MAG01[y & 1];
        }

        {
          var y = (this._state[MersenneTwister.N - 1] & MersenneTwister.UPPER_MASK) | (this._state[0] & MersenneTwister.LOWER_MASK);
          this._state[MersenneTwister.N - 1] = this._state[MersenneTwister.PERIOD - 1] ^ (y >> 1) ^ MersenneTwister._MAG01[y & 1];
        }

        this._index = 0;
      }

      var x = this._state[this._index++];

      x ^= x >> 11;
      x ^= (x << 7) & MersenneTwister._TEMPERING_MASK_B;
      x ^= (x << 15) & MersenneTwister._TEMPERING_MASK_C;
      x ^= x >> 18;

      return x;
    }
  }
  
}