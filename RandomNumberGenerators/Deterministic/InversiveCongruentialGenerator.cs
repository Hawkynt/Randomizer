using System.Runtime.CompilerServices;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class InversiveCongruentialGenerator : IRandomNumberGenerator {
  private ulong _state;
  private readonly ulong _a;
  private readonly ulong _c;
  private readonly ulong _q;

  public InversiveCongruentialGenerator(ulong a = 6364136223846793005, ulong c = 1442695040888963407, ulong q = 18446744073709551557) {
    this._a = a;
    this._c = c;
    this._q = q;
  }

  public void Seed(ulong seed) => this._state = seed % this._q;

  public ulong Next() {
    return this._state = this._state == 0 ? this._c : (this._a * ModInverse(this._state, this._q) + this._c) % this._q;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ulong ModInverse(ulong value, ulong modulus) {
      ulong t = 0, newT = 1;
      ulong r = modulus, newR = value;

      while (newR != 0) {
        var quotient = r / newR;
        var tProduct = quotient * newT;
        var rProduct = quotient * newR;

        (t, newT) = (newT, tProduct > t ? modulus + t - tProduct : t - tProduct);
        (r, newR) = (newR, rProduct > r ? modulus + r - rProduct : r - rProduct);
      }

      return r > 1 ? 0 : t;
    }
  }
}
