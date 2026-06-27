using System;
using System.Collections.Generic;
using System.Diagnostics;
using ScottPlot.Avalonia;
using ScottPlot;

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

        RenderLimitElements(condition, criticalPoint, digits);
        
        _plot.Plot.Axes.SetLimits(criticalPoint - 6, criticalPoint + 6, -10, 30);
        _plot.Refresh();
    }

    private const int RemovableDiscontinuity = 0;
    private const int JumpDiscontinuity = 1;
    private const int InfiniteDiscontinuity = 2;

    private void RenderLimitElements(int condition, int criticalPoint, int[] digits)
    {
        switch (condition)
        {
            case RemovableDiscontinuity:
                RenderRemovableDiscontinuity(criticalPoint, digits);
                break;
            case JumpDiscontinuity:
                RenderJumpDiscontinuity(criticalPoint, digits);
                break;
            case InfiniteDiscontinuity:
                RenderInfiniteDiscontinuity(criticalPoint);
                break;
        }
    }

    private void RenderRemovableDiscontinuity(int criticalPoint, int[] digits)
    {
        double limitValue = criticalPoint + digits[0];
        AddMarker(criticalPoint, limitValue, MarkerShape.OpenCircle);
    }

    private void RenderJumpDiscontinuity(int criticalPoint, int[] digits)
    {
        double leftLimit = criticalPoint + digits[1];
        double rightLimit = criticalPoint + digits[3];

        AddMarker(criticalPoint, leftLimit, MarkerShape.OpenCircle);
        AddMarker(criticalPoint, rightLimit, MarkerShape.FilledCircle);
    }

    private void RenderInfiniteDiscontinuity(int criticalPoint)
    {
        var asymptote = _plot.Plot.Add.VerticalLine(criticalPoint);
        asymptote.Color = Colors.Gray;
        asymptote.LinePattern = LinePattern.Dashed;
    }

    private void AddMarker(double x, double y, MarkerShape shape)
    {
        var marker = _plot.Plot.Add.Marker(x, y);
        marker.Color = Colors.Red;
        marker.Shape = shape;
        marker.Size = 10;
    }
}
