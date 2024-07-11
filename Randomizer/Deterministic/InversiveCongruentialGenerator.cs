namespace Randomizer;

public class InversiveCongruentialGenerator : IRandomNumberGenerator {

  private ulong _state;
  private const ulong _a = 6364136223846793005;
  private const ulong _c = 1; // 1442695040888963407;
  private const ulong _q = 18446744073709551557;

  public void Seed(ulong seed) => this._state = seed % InversiveCongruentialGenerator._q;

  public ulong Next() {
    return this._state = this._state == 0 ? InversiveCongruentialGenerator._c : (InversiveCongruentialGenerator._a * ModInverse(this._state, InversiveCongruentialGenerator._q) + InversiveCongruentialGenerator._c) % InversiveCongruentialGenerator._q;

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
