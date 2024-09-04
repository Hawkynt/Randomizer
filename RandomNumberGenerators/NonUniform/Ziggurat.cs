using System;
using Hawkynt.RandomNumberGenerators.Composites;
using Hawkynt.RandomNumberGenerators.Interfaces;

namespace Hawkynt.RandomNumberGenerators.NonUniform;

public unsafe class Ziggurat(ArbitraryNumberGenerator generator) : IDoubleRandomNumberGenerator {
  private const int NUM_LAYERS = 128;
  private const double R = 3.442619855899;
  private const double V = 9.91256303526217e-3;
  private const double R_INVERSE = 1 / R;

  private struct InlineDoubleArray {
    public fixed double Array[NUM_LAYERS];
  }

  private static readonly InlineDoubleArray layerWidths;
  private static readonly InlineDoubleArray layerHeights;

  static Ziggurat() {
    // Precompute the widths and heights of the layers
    var f = Math.Exp(-0.5 * R * R);

    layerWidths.Array[0] = V / f; /* [0] is bottom block: V / f(R) */
    layerWidths.Array[1] = R;

    var lastLayerWidth = R;
    for (var i = 2; i < NUM_LAYERS; ++i) {
      lastLayerWidth = layerWidths.Array[i] = Math.Sqrt(-2 * Math.Log(V / lastLayerWidth + f));
      f = Math.Exp(-0.5 * lastLayerWidth * lastLayerWidth);
    }

    layerHeights.Array[NUM_LAYERS - 1] = 0;
    for (var i = 0; i < NUM_LAYERS - 1; ++i)
      layerHeights.Array[i] = layerWidths.Array[i + 1] / layerWidths.Array[i];
  }

  public double Next() {
    double result;
    for (;;) {
      var i = (int)generator.ModuloRejectionSampling(NUM_LAYERS);
      var u = 2 * generator.NextDouble() - 1;

      /* first try the rectangular boxes */
      var layerWidth = layerWidths.Array[i];
      result = u * layerWidth;
      if (Math.Abs(u) < layerHeights.Array[i])
        break;

      /* bottom box: sample from the tail */
      if (i == 0) {
        result = SampleTail(u < 0);
        break;
      }

      /* is this a sample from the wedges? */
      var xSqr = result * result;
      var nextLayerWidth = i == NUM_LAYERS - 1 ? 0 : layerWidths.Array[i + 1];

      var f0 = Math.Exp(-0.5 * (layerWidth * layerWidth - xSqr));
      var f1 = Math.Exp(-0.5 * (nextLayerWidth * nextLayerWidth - xSqr));
      if (f1 + generator.NextDouble() * (f0 - f1) < 1.0)
        break;
    }

    return result;

    double SampleTail(bool isNegative) {
      for (;;) {
        var x = -Math.Log(generator.NextDouble()) * R_INVERSE;
        var y = -Math.Log(generator.NextDouble());
        if (y + y < x * x)
          continue;

        var result = R + x;
        if (isNegative)
          result = -result;

        return result;
      }
    }
  }
}
