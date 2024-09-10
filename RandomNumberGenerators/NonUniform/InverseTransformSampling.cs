using System;
using Hawkynt.RandomNumberGenerators.Composites;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

public class InverseTransformSampling : IDoubleRandomNumberGenerator {

  private readonly double _lambda;
  private readonly ArbitraryNumberGenerator _generator;

  public InverseTransformSampling(ArbitraryNumberGenerator generator, double lambda = 1) {
    this._generator=generator;
    this._lambda = lambda;
    this._factory = lambda == 1 ? this._NextOne : this._NextLambda;
  }

  private readonly Func<double> _factory;

  public double Next() => this._factory();

  private double _NextOne() {
    var u = this._generator.NextDouble();
    return -Math.Log(1 - u);
  }

  private double _NextLambda() {
    var u = this._generator.NextDouble();
    return -Math.Log(1 - u) / this._lambda;
  }
}
