namespace Randomizer.Deterministic;

public class Mixmax : IRandomNumberGenerator {
  
  private const int _matrixSize = 256;
  private const long _magicNumber = -3;
  private ulong[] _state;
  private readonly ulong[,] _matrix;

  public Mixmax() {
    this._state = new ulong[_matrixSize];
    this._matrix = new ulong[_matrixSize, _matrixSize];
    this.InitializeMatrix();
  }

  private void InitializeMatrix() {
    for (var row = 0; row < _matrixSize; ++row) {
      this._matrix[row, 0] = 1;
      for (var column = 1; column < _matrixSize; ++column)
        if (column > row)
          this._matrix[row, column] = 1;
        else
          this._matrix[row, column] = (ulong)(row - column + 2);
    }

    this._matrix[2, 1]+= unchecked((ulong)_magicNumber);
  }

  public void Seed(ulong seed) {
    for (var i = 0; i < _matrixSize; ++i)
      this._state[i] = SplitMix64.Next(ref seed);
  }

  public ulong Next() { // implicit mod 2^64

    ulong result = 0;
    for (var i = 0; i < _matrixSize; ++i)
      result += this._state[i];

    var newState = new ulong[_matrixSize];
    for (var i = 0; i < _matrixSize; ++i) {
      newState[i] = 0;
      for (var j = 0; j < _matrixSize; ++j)
        newState[i] += this._matrix[i, j] * this._state[j];
    }

    this._state = newState;
    return result;
  }
}
