using System;
using System.Numerics;

namespace Randomizer.Statistics;

/* Randogramm
Those are powerful visualization tools used to detect patterns or flaws in the output of RNGs. They provide a 2D representation of the state space, offering an intuitive way to assess the randomness of an RNG's output. By visualizing the output in this manner, we can quickly identify regularities or anomalies that might not be apparent through numerical analysis alone. Randograms extract distinct bits from different parts of the output value and convert them into (x, y) coordinates, using pixel intensity to represent occurrence frequency.

We can select bits using:

Groups of adjacent bits
Groups of bits from opposite halves of the output
Groups of bits from the start and their reverse from the end
Interleaved groups of bits
Bits from two consecutive outputs of the RNG
Visualizing 64-Bit Output with 256x256 Randograms
For a comprehensive analysis of a 64-bit RNG, we can use four separate 256x256 randograms, each visualizing a 16-bit segment of the output. Each randogram will represent a different 16-bit slice, allowing us to scrutinize the entire 64-bit output in detail.

Pixel Intensity Representation: In a randogram, each pixel represents the frequency of occurrence of a specific 16-bit value pair (x, y). The intensity of the pixel is determined by how often that particular pair appears in the output:
White (100% intensity) indicates that the pair does not appear at all.
50% grey indicates the pair appears once.
Darker shades represent multiple occurrences, with each additional occurrence halving the pixel's intensity.
This method allows us to identify potential patterns or repetitions that could indicate flaws in the RNG.

8-Bit Visualizations
For smaller-scale analysis, especially when working with simpler or reduced versions of RNGs, an 8-bit segment of the output can be visualized using a tiny randogram. This smaller grid is particularly useful for identifying issues in the least significant bits, which are often more prone to predictability or bias.

16x16 Randogram: In this visualization, each pixel corresponds to one of the possible 8-bit output values. The same intensity rules apply, with pixels turning darker as specific value pairs occur more frequently. A well-functioning RNG should produce a 16x16 randogram that appears random, with no discernible patterns.
Interpreting Randograms
"Random-looking": A high-quality RNG should produce randograms, whether 16x16 or 256x256, that appear as a uniform field of noise, without discernible patterns or regular structures. Such a visualization suggests that the RNG’s output is sufficiently random for practical use.

Patterns and Structures: If a randogram exhibits clear patterns, such as diagonal lines, clusters, or grids, it indicates that the RNG might have structural flaws or that its output is not sufficiently random. These patterns suggest that certain pairs of output values are more likely to occur together, which could compromise the statistical quality of the RNG.

 */
internal class Randogram : IValueTracker {

  public enum BitSelectionMethod {
    Adjacent,
    OppositeHalves,
    StartAndReverse,
    Interleaved,
    Consecutive
  }

  private readonly byte _bitCount;
  private readonly BitSelectionMethod _method;
  private readonly byte _yBits;
  private readonly byte _xBits;
  private readonly ulong[,] _histogram;

  private ulong? _lastValue;

  public Randogram(byte bitCount, BitSelectionMethod method) {
    this._bitCount = bitCount is >= 2 and <= 64 ? bitCount : throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, $"Bit count has to be 2 <= x <= 64, got {bitCount}");
    this._method = Enum.IsDefined(method) ? method : throw new ArgumentOutOfRangeException(nameof(method), method, $"Unknown bit selection method:{method}");
    this._yBits = (byte)(bitCount >> 1);
    this._xBits = (byte)(bitCount - this._yBits);
    this._histogram = new ulong[1 << this._yBits, 1 << this._xBits];
  }

  public void Feed(ulong value) {
    var coordinates = this._SelectBits(value);
    if (coordinates!=null)
      ++this._histogram[coordinates.Item2, coordinates.Item1];

    this._lastValue = value;
  }

  private Tuple<ulong, ulong>? _SelectBits(ulong value) {
    return this._method switch {
      BitSelectionMethod.Adjacent => Tuple.Create(value.Bits(0, this._xBits), value.Bits(this._xBits, this._yBits)),
      BitSelectionMethod.OppositeHalves => Tuple.Create(value.Bits(0, this._xBits), value.Bits(32, this._yBits)),
      BitSelectionMethod.StartAndReverse => Tuple.Create(value.Bits(0, this._xBits), value.ReverseBits().Bits(0, this._yBits)),
      BitSelectionMethod.Consecutive when this._lastValue == null => null,
      BitSelectionMethod.Consecutive => Tuple.Create(value.Bits(0, this._xBits), this._lastValue.Value.Bits(0, this._yBits)),
      BitSelectionMethod.Interleaved when value.DeinterleaveBits() is var (o, e) =>  Tuple.Create((ulong)o.Bits(0,this._xBits), (ulong)e.Bits(0, this._yBits)),
      _ => throw new NotSupportedException($"Unknown method: {this._method}")
    };
  }


  public void Print() {
    throw new NotImplementedException();
  }

}
