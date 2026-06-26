using System;
using System.Collections.Generic;
using System.Diagnostics;
using ScottPlot.Avalonia;

namespace RutGeo.UI.Helpers;

public class LimitPlotService
{
    private readonly AvaPlot _plot;

    public LimitPlotService(AvaPlot plot)
    {
        _plot = plot;
    }

    public void Plot(int condition, int criticalPoint, int[] digits)
    {
        _plot.Plot.Clear();
        _plot.DrawAxes();

        List<double> xs = new();
        List<double> ys = new();

        try
        {
            LimitPlotter.PlotLimitFunction(condition, criticalPoint, digits, xs, ys);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LimitPlotService failed: {ex.Message}");
        }

        if (xs.Count > 1)
            _plot.AddScatterNoMarkers(xs.ToArray(), ys.ToArray());

        _plot.Plot.Axes.AutoScale();
        _plot.Refresh();
    }
}
