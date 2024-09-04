using System.Runtime.CompilerServices;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

public class InversiveCongruentialGenerator : IRandomNumberGenerator {
  private ulong _state;
  private const ulong _A = 6364136223846793005;
  private const ulong _C = 1442695040888963407;
  private const ulong _Q = 18446744073709551557;

  public void Seed(ulong seed) => this._state = seed % _Q;

  public ulong Next() {
    return this._state = this._state == 0 ? _C : (_A * ModInverse(this._state, _Q) + _C) % _Q;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ulong ModInverse(ulong value, ulong modulus) {
      ulong t = 0, newT = 1;
      ulong r = modulus, newR = value;

      while (newR != 0) {
        var quotient = r / newR;
        var tProduct = quotient * newT;
        var rProduct = quotient * newR;

        (t, newT) = (newT, tProduct > t ? modulus + t - tProduct : t - tProduct);
        (r, newR) = (newR, rProduct > t ? modulus + r - rProduct : r - rProduct);
      }

      return r > 1 ? 0 : t;
    }
  }
}
