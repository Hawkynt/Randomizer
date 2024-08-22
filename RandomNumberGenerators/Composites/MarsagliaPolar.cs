using System;

namespace Hawkynt.RandomNumberGenerators.Composites;

public class MarsagliaPolar(ArbitraryNumberGenerator generator) {

  public (double, double) Next() {
    for (;;) {
      var x = 2 * generator.NextDouble() - 1;
      var y = 2 * generator.NextDouble() - 1;
      var s = x * x + y * y;
      if (s is <= 0 or >= 1)
        continue;

      var multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
      return (x * multiplier, y * multiplier);
    }
  }

}