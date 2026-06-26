using System;
using System.Collections.Generic;
using System.Diagnostics;
using ScottPlot.Avalonia;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Types;

namespace RutGeo.UI.Helpers;

public class ConicPlotService
{
    private readonly AvaPlot _plot;

    public ConicPlotService(AvaPlot plot)
    {
        _plot = plot;
    }

    public void Plot(GeneralEquation eq, Conic conic, CanonicalEquation canon)
    {
        _plot.Plot.Clear();
        _plot.DrawAxes();

        List<double> xs = new();
        List<double> ys = new();

        try
        {
            switch (conic.Type)
            {
                case ConicType.Circunferencia:
                    ConicPlotter.PlotCircle(eq, xs, ys);
                    break;
                case ConicType.Elipse:
                    ConicPlotter.PlotEllipse(eq, xs, ys);
                    break;
                case ConicType.Hyperbola:
                    ConicPlotter.PlotHyperbola(eq, xs, ys);
                    break;
                case ConicType.Parabola:
                    ConicPlotter.PlotParabola(eq, xs, ys);
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ConicPlotService failed: {ex.Message}");
        }

        if (xs.Count > 1)
            RenderSegments(xs, ys);

        _plot.Plot.Axes.AutoScale();
        _plot.Refresh();
    }

    private void RenderSegments(List<double> xs, List<double> ys)
    {
        List<double> segXs = new();
        List<double> segYs = new();

        for (int i = 0; i < xs.Count; i++)
        {
            if (double.IsNaN(xs[i]) || double.IsNaN(ys[i]))
            {
                if (segXs.Count > 1)
                    _plot.AddScatterNoMarkers(segXs.ToArray(), segYs.ToArray());
                segXs.Clear();
                segYs.Clear();
            }
            else
            {
                segXs.Add(xs[i]);
                segYs.Add(ys[i]);
            }
        }

        if (segXs.Count > 1)
            _plot.AddScatterNoMarkers(segXs.ToArray(), segYs.ToArray());
    }
}
