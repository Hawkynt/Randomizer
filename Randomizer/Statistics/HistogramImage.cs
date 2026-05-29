using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Randomizer.Statistics;

internal static class HistogramImage {

  private const int IMG_WIDTH = 800;
  private const int IMG_HEIGHT = 400;
  private const int MARGIN_LEFT = 60;
  private const int MARGIN_RIGHT = 20;
  private const int MARGIN_TOP = 40;
  private const int MARGIN_BOTTOM = 40;

  public static void Render(string title, ulong[] data, FileInfo file, ulong[]? secondary = null) {
    file.Directory?.Create();
    using var bitmap = new Bitmap(IMG_WIDTH, IMG_HEIGHT);
    using var g = Graphics.FromImage(bitmap);
    g.Clear(Color.White);
    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

    using var titleFont = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
    using var labelFont = new Font(FontFamily.GenericSansSerif, 8);
    using var blackPen = new Pen(Color.Black);
    using var primaryBrush = new SolidBrush(Color.FromArgb(50, 110, 200));
    using var secondaryBrush = new SolidBrush(Color.FromArgb(220, 90, 90));

    g.DrawString(title, titleFont, Brushes.Black, MARGIN_LEFT, 8);

    var plotWidth = IMG_WIDTH - MARGIN_LEFT - MARGIN_RIGHT;
    var plotHeight = IMG_HEIGHT - MARGIN_TOP - MARGIN_BOTTOM;
    var plotLeft = MARGIN_LEFT;
    var plotRight = MARGIN_LEFT + plotWidth;

    var max = data.Max();
    if (secondary != null && secondary.Length > 0)
      max = Math.Max(max, secondary.Max());
    if (max == 0)
      max = 1;

    var bins = data.Length;
    var barWidth = plotWidth / (double)bins;

    if (secondary == null) {
      // Single series: bars rise from baseline
      var baseline = MARGIN_TOP + plotHeight;
      for (var i = 0; i < bins; ++i) {
        var h = (int)(data[i] * (double)plotHeight / max);
        var x = (int)(plotLeft + i * barWidth);
        var w = Math.Max(1, (int)Math.Ceiling(barWidth) - 1);
        g.FillRectangle(primaryBrush, x, baseline - h, w, h);
      }
      g.DrawLine(blackPen, plotLeft, baseline, plotRight, baseline);
    } else {
      // Dual series: primary above midline, secondary below
      var midline = MARGIN_TOP + plotHeight / 2;
      var halfPlot = plotHeight / 2;
      for (var i = 0; i < bins; ++i) {
        var x = (int)(plotLeft + i * barWidth);
        var w = Math.Max(1, (int)Math.Ceiling(barWidth) - 1);

        var hPrimary = (int)(data[i] * (double)halfPlot / max);
        g.FillRectangle(primaryBrush, x, midline - hPrimary, w, hPrimary);

        if (i < secondary.Length) {
          var hSecondary = (int)(secondary[i] * (double)halfPlot / max);
          g.FillRectangle(secondaryBrush, x, midline, w, hSecondary);
        }
      }
      g.DrawLine(blackPen, plotLeft, midline, plotRight, midline);
    }

    // Y-axis labels (4 ticks: 0, max/3, 2*max/3, max)
    using var stringFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
    if (secondary == null) {
      var baseline = MARGIN_TOP + plotHeight;
      for (var t = 0; t <= 3; ++t) {
        var value = max * (ulong)t / 3.0;
        var y = baseline - (int)(t / 3.0 * plotHeight);
        g.DrawString(_FormatValue((ulong)value), labelFont, Brushes.Black, plotLeft - 4, y, stringFormat);
        g.DrawLine(Pens.LightGray, plotLeft, y, plotRight, y);
      }
    } else {
      var midline = MARGIN_TOP + plotHeight / 2;
      var halfPlot = plotHeight / 2;
      g.DrawString(_FormatValue(max), labelFont, Brushes.Black, plotLeft - 4, MARGIN_TOP, stringFormat);
      g.DrawString("0", labelFont, Brushes.Black, plotLeft - 4, midline, stringFormat);
      g.DrawString(_FormatValue(max), labelFont, Brushes.Black, plotLeft - 4, MARGIN_TOP + plotHeight, stringFormat);
    }

    // X-axis labels (every 8 bins for 64+ bin charts, otherwise every bin)
    var stride = bins > 16 ? 8 : 1;
    using var xLabelFormat = new StringFormat { Alignment = StringAlignment.Center };
    var xAxisY = secondary == null
      ? MARGIN_TOP + plotHeight + 4
      : MARGIN_TOP + plotHeight + 4;
    for (var i = 0; i < bins; i += stride) {
      var x = plotLeft + (int)((i + 0.5) * barWidth);
      g.DrawString(i.ToString(), labelFont, Brushes.Black, x, xAxisY, xLabelFormat);
    }

    bitmap.Save(file.FullName, ImageFormat.Png);
  }

  private static string _FormatValue(ulong value) => value switch {
    >= 1_000_000_000 => $"{value / 1_000_000_000.0:F1}B",
    >= 1_000_000 => $"{value / 1_000_000.0:F1}M",
    >= 1_000 => $"{value / 1_000.0:F1}k",
    _ => value.ToString(),
  };
}
