using System;
using Hawkynt.RandomNumberGenerators.Composites;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

public class InverseTransformSampling(ArbitraryNumberGenerator generator) : IDoubleRandomNumberGenerator {
  public double Next() => this.Exponential(1);

  public double Exponential(double lambda) {
    var u = generator.NextDouble();
    return -Math.Log(1 - u) / lambda;
  }
}
