using System.Runtime.CompilerServices;

namespace Randomizer.Deterministic;

public class WellEquidistributedLongperiodLinear : IRandomNumberGenerator {
  private const int R = 32;
  private const int M1 = 3;
  private const int M2 = 24;
  private const int M3 = 10;

  private uint index;
  private readonly uint[] _state = new uint[R];
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static uint MAT0POS(int t, uint v) => v ^ (v >> t);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static uint MAT0NEG(int t, uint v) => v ^ (v << -t);

  private uint V0 {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => this._state[this.index];
  }

  private uint VM1 {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => this._state[(this.index + M1) % R];
  }

  private uint VM2 {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => this._state[(this.index + M2) % R];
  }

  private uint VM3 {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => this._state[(this.index + M3) % R];
  }

  private uint VRm1 {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => this._state[(this.index + R - 1) % R]; 
  }

  private uint newV0 {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => this._state[(this.index + R - 1) % R] = value;
  }

  private uint newV1 {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    set => this._state[this.index] = value;
  }

  public void Seed(ulong seed) {
    this.index = 0;
    for (var i = 0; i < this._state.Length; ++i)
      this._state[i] = (uint)SplitMix64(ref seed);
    
    return;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong SplitMix64(ref ulong z) {
      z += 0x9E3779B97F4A7C15;
      z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
      z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
      return z ^= (z >> 31);
    }
  }

  public ulong Next() {
    return (ulong)Next32() << 32 | Next32();

    uint Next32() {
      const int T1 = 8;
      const int T2 = -19;
      const int T3 = -14;
      const int T4 = -11;
      const int T5 = -7;
      const int T6 = -13;
      
      var z0 = this.VRm1;
      var z1 = this.V0 ^ MAT0POS(T1, this.VM1);
      var z2 = MAT0NEG(T2, this.VM2) ^ MAT0NEG(T3, this.VM3);
      var z3 = z1 ^ z2;

      this.newV1 = z3;
      this.newV0 = MAT0NEG(T4, z0) ^ MAT0NEG(T5, z1) ^ MAT0NEG(T6, z2);
      this.index = (this.index + R - 1) % R;
      return this._state[this.index];
    }
  }
}
