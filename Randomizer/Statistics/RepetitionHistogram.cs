using Randomizer.Statistics;

internal class RepetitionHistogram : IValueTracker {
  private const int NumBins = 64;
  private const double ScaleFactor = NumBins / (double)ulong.MaxValue;
  private readonly ulong[] _bins = new ulong[NumBins];

  #region Implementation of IValueTracker

  public void Feed(ulong value) => ++this._bins[(int)(value * ScaleFactor)];

  public void Print() => HistogramDrawer.DrawHistogram("Repetition Histogram", 10, this._bins);

  #endregion
}