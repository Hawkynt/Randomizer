using System;

namespace Hawkynt.RandomNumberGenerators.Composites;

public class Ziggurat(ArbitraryNumberGenerator generator) {

  private const int NUM_LAYERS = 128;
  private const double R = 3.442619855899;
  private const double R_INVERSE = 1 / R;
  private static readonly double[] layerWidths = new double[NUM_LAYERS];
  
  static Ziggurat() {
    
    // Precompute the widths and heights of the layers
    var f = Math.Exp(-0.5 * R * R);

    var layerHeights = new double[NUM_LAYERS];
    for (var i = 0; i < NUM_LAYERS; ++i) {

      // because the first layer is fixed, we don't store it in the array and calculate it here if needed
      var previousHeight = i == 0 ? f : layerHeights[i - 1];

      var value = Math.Sqrt(-2 * Math.Log(previousHeight + f));
      layerWidths[i] = value;
      layerHeights[i] = value * f;
    }

  }

  public double Next() {
    for(;;) {

      // the first layer is not stored in the array due to it causing always a tail sampling
      var layer = (int)generator.ModuloRejectionSampling(NUM_LAYERS + 1);
      if (layer == 0)
        return SampleTail();

      var layerWidth = layerWidths[layer - 1];
      var x = generator.NextDouble() * layerWidth;
      if (x < layerWidth)
        return x;
      
      if (generator.NextDouble() < Math.Exp(-0.5 * x * x))
        return x;
    }

    double SampleTail() {
      for(;;) {
        var x = -Math.Log(generator.NextDouble()) * R_INVERSE;
        var y = -Math.Log(generator.NextDouble());
        if (y + y >= x * x)
          continue;

        var result = R + x;
        if (x <= 0)
          result = -result;

        return result;
      }
    }
  }

}