using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.Deterministic;

/// <summary>
///   Rule 30 elementary cellular automaton, used by Stephen Wolfram as a random number
///   generator in early versions of <i>Mathematica</i>. Each cell of a 1D bit array
///   updates simultaneously by the local rule
///   <c>new = left XOR (center OR right)</c>. Despite the deterministic, fully
///   local nature of the rule, the centre column of the evolving pattern passes many
///   randomness tests.
/// </summary>
/// <remarks>
///   This is a paradigmatically different generator from every other algorithm in
///   the package — no shifts, no multiplies, no XOR cascades; just simultaneous
///   updates of a bit array under a 3-bit lookup. Output bits are taken from the
///   centre cell, with 64 evolution steps producing one 64-bit value.
/// </remarks>
public class Rule30 : IRandomNumberGenerator {
  private const int WIDTH = 256; // bit array length (wider = harder to detect periodicity)

  private readonly bool[] _cells = new bool[WIDTH];
  private readonly bool[] _next = new bool[WIDTH];

  public void Seed(ulong seed) {
    // Spread the 64 seed bits across the centre of the array; flip a single bit
    // at the centre if the seed would otherwise leave the array all-zero (Rule 30
    // has the all-zero pattern as a fixed point).
    System.Array.Clear(this._cells);
    var anySet = false;
    for (var i = 0; i < 64; ++i) {
      var bit = ((seed >> i) & 1) != 0;
      this._cells[WIDTH / 2 - 32 + i] = bit;
      anySet |= bit;
    }

    if (!anySet)
      this._cells[WIDTH / 2] = true;
  }

  public ulong Next() {
    var result = 0UL;
    for (var step = 0; step < 64; ++step) {
      // One synchronous step of Rule 30 over the cyclic array.
      for (var i = 0; i < WIDTH; ++i) {
        var left = this._cells[(i + WIDTH - 1) % WIDTH];
        var center = this._cells[i];
        var right = this._cells[(i + 1) % WIDTH];
        this._next[i] = left ^ (center | right);
      }

      if (this._cells[WIDTH / 2])
        result |= 1UL << step;

      // Promote _next into _cells for the following step.
      for (var i = 0; i < WIDTH; ++i)
        this._cells[i] = this._next[i];
    }

    return result;
  }
}
