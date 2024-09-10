using System;
using Hawkynt.RandomNumberGenerators.Composites;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

public class BoxMuller(ArbitraryNumberGenerator generator) : IDoubleRandomNumberGenerator {
  public (double, double) Next() {
    var x = generator.NextDouble();
    var y = generator.NextDouble();

    x *= 2;
    y *= 2;

    x -= 1;
    y -= 1;

    var r = Math.Sqrt(-2.0 * Math.Log(x));
    var theta = 2.0 * Math.PI * y;

    var z0 = r * Math.Cos(theta);
    var z1 = r * Math.Sin(theta);

    return (z0, z1);
  }
}
