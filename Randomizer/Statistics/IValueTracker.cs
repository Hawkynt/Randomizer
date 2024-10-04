namespace Randomizer.Statistics;
internal interface IValueTracker {
  void Feed(ulong value);
  void Print();
}