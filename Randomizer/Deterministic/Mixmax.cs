using System.Runtime.CompilerServices;

namespace Randomizer.Deterministic;
public class Mixmax : IRandomNumberGenerator {
  
  private const int _matrixSize = 256;
  private const long _magicNumber = -3;
  private ulong[] _state;
  private readonly ulong[,] _matrix;

  public Mixmax() {
    this._state = new ulong[Mixmax._matrixSize];
    this._matrix = new ulong[Mixmax._matrixSize, Mixmax._matrixSize];
    this.InitializeMatrix();
  }

  private void InitializeMatrix() {
    for (var row = 0; row < Mixmax._matrixSize; ++row) {
      this._matrix[row, 0] = 1;
      for (var column = 1; column < Mixmax._matrixSize; ++column)
        if (column > row)
          this._matrix[row, column] = 1;
        else
          this._matrix[row, column] = (ulong)(row - column + 2);
    }

    this._matrix[2, 1]+= unchecked((ulong)Mixmax._magicNumber);
  }

  public void Seed(ulong seed) {
    for (var i = 0; i < Mixmax._matrixSize; ++i)
      this._state[i] = SplitMix64(ref seed);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong SplitMix64(ref ulong z) {
      z += 0x9E3779B97F4A7C15;
      z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
      z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
      return z ^= (z >> 31);
    }
  }

  public ulong Next() { // implicit mod 2^64

    ulong result = 0;
    for (var i = 0; i < Mixmax._matrixSize; ++i)
      result += this._state[i];

    var newState = new ulong[Mixmax._matrixSize];
    for (var i = 0; i < Mixmax._matrixSize; ++i) {
      newState[i] = 0;
      for (var j = 0; j < Mixmax._matrixSize; ++j)
        newState[i] += this._matrix[i, j] * this._state[j];
    }

    this._state = newState;
    return result;
  }
}
