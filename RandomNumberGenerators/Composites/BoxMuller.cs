using System;

namespace Hawkynt.RandomNumberGenerators.Composites;

public class BoxMuller(ArbitraryNumberGenerator generator) {

  public (double, double) Next() {
    double x = 2 * generator.NextDouble() - 1;
    double y = 2 * generator.NextDouble() - 1;
   
    double r = Math.Sqrt(-2.0 * Math.Log(x));
    double theta = 2.0 * Math.PI * y;

    double z0 = r * Math.Cos(theta);
    double z1 = r * Math.Sin(theta);

    return (z0, z1);
  }

}