using System;
using Hawkynt.RandomNumberGenerators.Composites;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

public class BoxMuller(ArbitraryNumberGenerator generator) {

  public (double, double) Next() {
    var x = 2 * generator.NextDouble() - 1;
    var y = 2 * generator.NextDouble() - 1;
   
    var r = Math.Sqrt(-2.0 * Math.Log(x));
    var theta = 2.0 * Math.PI * y;

    var z0 = r * Math.Cos(theta);
    var z1 = r * Math.Sin(theta);

    return (z0, z1);
  }

}