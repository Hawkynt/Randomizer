using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Randomizer.Statistics;

internal class Randogram : IValueTracker {

  public enum BitSelectionMethod {
    Adjacent,
    OppositeHalves,
    StartAndReverse,
    Interleaved,
    Consecutive
  }

  private byte BitCount => (byte)(this._xBits + this._yBits);
  private readonly BitSelectionMethod _method;
  private readonly byte _yBits;
  private readonly byte _xBits;
  private readonly ulong[,] _histogram;

  private ulong? _lastValue;

  public Randogram(byte bitCount, BitSelectionMethod method) {
    if (bitCount is < 2 or > 64)
      throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, $"Bit count has to be 2 <= x <= 64, got {bitCount}");
    
    this._method = Enum.IsDefined(method) ? method : throw new ArgumentOutOfRangeException(nameof(method), method, $"Unknown bit selection method:{method}");
    this._yBits = (byte)(bitCount >> 1);
    this._xBits = (byte)(bitCount - this._yBits);
    this._histogram = new ulong[1 << this._yBits, 1 << this._xBits];
  }

  public void Feed(ulong value) {
    var coordinates = this._SelectBits(value);
    if (coordinates!=null)
      ++this._histogram[coordinates.Value.y, coordinates.Value.x];

    this._lastValue = value;
  }

  private (ulong x, ulong y)? _SelectBits(ulong value) {
    return this._method switch {
      BitSelectionMethod.Adjacent => (value.Bits(0, this._xBits), value.Bits(this._xBits, this._yBits)),
      BitSelectionMethod.OppositeHalves => (value.Bits(0, this._xBits), value.Bits(32, this._yBits)),
      BitSelectionMethod.StartAndReverse => (value.Bits(0, this._xBits), value.ReverseBits().Bits(0, this._yBits)),
      BitSelectionMethod.Consecutive when this._lastValue == null => null,
      BitSelectionMethod.Consecutive => (value.Bits(0, this._xBits), this._lastValue.Value.Bits(0, this._yBits)),
      BitSelectionMethod.Interleaved when value.DeinterleaveBits() is var (o, e) =>  (o.Bits(0, this._xBits), e.Bits(0, this._yBits)),
      _ => throw new NotSupportedException($"Unknown method: {this._method}")
    };
  }
  
  public void Print() => this.SaveToPng(new("randogram.png"));

  public void SaveToPng(FileInfo file) {
    var width = 1 << this._xBits;
    var height = 1 << this._yBits;
    using var bitmap = new Bitmap(width, height);

    var max = this._histogram.Cast<ulong>().Max();
    var maxLog = Math.Log((max == 0 ? 1 : max) + 1);

    using (var locker = bitmap.Lock())
      for (var y = 0; y < height; ++y)
      for (var x = 0; x < width; ++x) {
        var intensityFactor = Math.Log((double)this._histogram[y, x] + 1) / maxLog;
        var intensity = (int)(255 * (1 - intensityFactor));
        locker[x, y] = Color.FromArgb(intensity, intensity, intensity);
      }

    bitmap.SaveToPng(file);
  }
  
}
