using System;
using System.Collections.Generic;
using Avalonia.Controls;
using ScottPlot.Avalonia;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Types;
using RutGeo.UI.Helpers;

namespace RutGeo.UI.Views;

public partial class Graphic : UserControl
{
    private AvaPlot _plot;

    public Graphic()
    {
        InitializeComponent();
        _plot = this.Find<AvaPlot>("MainPlot")!;
    }

    private void AddScatterNoMarkers(double[] xs, double[] ys)
    {
        var scatter = _plot.Plot.Add.Scatter(xs, ys);
        scatter.MarkerSize = 0;
    }

    private void DrawAxes()
    {
        var xAxis = _plot.Plot.Add.HorizontalLine(0);
        xAxis.Color = ScottPlot.Colors.Gray;
        xAxis.LineStyle.Width = 1;

        var yAxis = _plot.Plot.Add.VerticalLine(0);
        yAxis.Color = ScottPlot.Colors.Gray;
        yAxis.LineStyle.Width = 1;
    }

    public void ClearGraph()
    {
        _plot.Plot.Clear();
        DrawAxes();
        _plot.Plot.Axes.AutoScale();
        _plot.Refresh();
    }

    public void UpdatePlot(GeneralEquation eq, Conic conic, CanonicalEquation canon)
    {
        _plot.Plot.Clear();
        DrawAxes();

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
        catch
        {
        }

        if (xs.Count > 1)
        {
            List<double> segXs = new();
            List<double> segYs = new();
            for (int i = 0; i < xs.Count; i++)
            {
                if (double.IsNaN(xs[i]) || double.IsNaN(ys[i]))
                {
                    if (segXs.Count > 1)
                        AddScatterNoMarkers(segXs.ToArray(), segYs.ToArray());
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
                AddScatterNoMarkers(segXs.ToArray(), segYs.ToArray());
        }

        _plot.Plot.Axes.AutoScale();
        _plot.Refresh();
    }

    public void UpdateLimitPlot(int condition, int a, int[] digits)
    {
        _plot.Plot.Clear();
        DrawAxes();

        List<double> xs = new();
        List<double> ys = new();

        try
        {
            LimitPlotter.PlotLimitFunction(condition, a, digits, xs, ys);
        }
        catch
        {
        }

        if (xs.Count > 1)
        {
            AddScatterNoMarkers(xs.ToArray(), ys.ToArray());
        }

        _plot.Plot.Axes.AutoScale();
        _plot.Refresh();
    }
}
