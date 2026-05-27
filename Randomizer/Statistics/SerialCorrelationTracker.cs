using System;

namespace Randomizer.Statistics;

internal class SerialCorrelationTracker : IValueTracker {
  private double _sumX;
  private double _sumY;
  private double _sumXY;
  private double _sumX2;
  private double _sumY2;
  private ulong _count;
  private ulong? _lastValue;

  public void Feed(ulong value) {
    if (this._lastValue == null) {
      this._lastValue = value;
      return;
    }

    var x = (double)this._lastValue.Value;
    var y = (double)value;
    this._sumX += x;
    this._sumY += y;
    this._sumXY += x * y;
    this._sumX2 += x * x;
    this._sumY2 += y * y;
    ++this._count;
    this._lastValue = value;
  }

  public void Print() {
    if (this._count < 2) {
      Console.WriteLine("Serial Correlation (lag-1): insufficient data");
      return;
    }

    var n = (double)this._count;
    var numerator = n * this._sumXY - this._sumX * this._sumY;
    var denominator = Math.Sqrt((n * this._sumX2 - this._sumX * this._sumX) * (n * this._sumY2 - this._sumY * this._sumY));
    var correlation = denominator == 0 ? 0 : numerator / denominator;
    Console.WriteLine($"Serial Correlation (lag-1): {correlation:+0.000000;-0.000000} (ideal: 0.000000)");
  }
}
