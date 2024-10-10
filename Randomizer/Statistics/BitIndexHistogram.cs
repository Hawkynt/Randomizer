namespace Randomizer.Statistics;

internal class BitIndexHistogram:IValueTracker {
  private readonly ulong[] _ones=new ulong[64];
  private readonly ulong[] _zeros = new ulong[64];

  #region Implementation of IValueTracker

  public void Feed(ulong value) {
    for (var i = 0; i < 64; ++i)
      ++((value & (1UL << i)) == 0 ? this._zeros : this._ones)[i];
  }

  public void Print() => HistogramDrawer.DrawHistogram("Bit Index Histogram", 10, this._ones,this._zeros);

  #endregion
}
