using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Randomizer;

/// <summary>
///   Prints a single-line progress indicator to stderr at a fixed interval, showing the
///   current count out of a total, an elapsed-time hint, and (optionally) which generators
///   are still in flight. Caller increments a shared counter and adds/removes
///   in-flight items via the supplied callbacks — this avoids tight coupling between
///   worker threads and the rendering logic.
/// </summary>
internal sealed class ProgressTicker : IDisposable {
  private static readonly char[] _SPINNER = ['|', '/', '-', '\\'];

  private readonly Func<int> _readCount;
  private readonly Func<IReadOnlyList<(string name, double elapsedSec)>> _readInFlight;
  private readonly int _total;
  private readonly Stopwatch _stopwatch;
  private readonly Timer _timer;
  private int _spinnerIndex;
  private int _disposed;
  private int _maxLineWidth;

  public ProgressTicker(
    Func<int> readCount,
    Func<IReadOnlyList<(string name, double elapsedSec)>> readInFlight,
    int total,
    Stopwatch stopwatch,
    int intervalMs = 500) {
    this._readCount = readCount;
    this._readInFlight = readInFlight;
    this._total = total;
    this._stopwatch = stopwatch;
    this._timer = new Timer(this._Tick, null, intervalMs, intervalMs);
  }

  private void _Tick(object? state) {
    if (Volatile.Read(ref this._disposed) != 0)
      return;

    var done = this._readCount();
    var spinner = _SPINNER[this._spinnerIndex++ & 3];
    var elapsed = this._stopwatch.Elapsed.TotalSeconds;
    var line = $"  [{spinner}] {done}/{this._total} done  ({elapsed,4:F1}s)";

    // When most generators have finished, show the slowest 2 in-flight items so the
    // user can see *which* generators we're still waiting on.
    var inFlight = this._readInFlight();
    if (inFlight.Count > 0 && inFlight.Count <= 8) {
      var slowest = inFlight
        .OrderByDescending(x => x.elapsedSec)
        .Take(2)
        .Select(x => $"{x.name} ({x.elapsedSec:F0}s)");
      line += "  waiting on: " + string.Join(", ", slowest);
    }

    if (line.Length > this._maxLineWidth)
      this._maxLineWidth = line.Length;

    // Pad to the historical max width so previous longer text gets overwritten.
    Console.Error.Write("\r" + line.PadRight(this._maxLineWidth + 2));
  }

  public void Dispose() {
    if (Interlocked.Exchange(ref this._disposed, 1) != 0)
      return;

    this._timer.Dispose();
    // Clear the progress line cleanly so the next stdout line starts at column 0.
    Console.Error.Write("\r" + new string(' ', this._maxLineWidth + 2) + "\r");
  }
}
