using System;
using System.Collections.Generic;
using System.Diagnostics;
using ScottPlot.Avalonia;
using ScottPlot;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Types;
using RutGeo.Core.Helpers;

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

        RenderConicElements(canon?.Elements);

        // Si es circunferencia o elipse, AutoScale funciona perfecto.
        // Si es hipérbola o parábola generada masivamente, limitamos la vista inicial 
        // para que no se vea minúscula.
        if (conic.Type == ConicType.Circunferencia || conic.Type == ConicType.Elipse)
        {
            _plot.Plot.Axes.AutoScale();
        }
        else
        {
            double centerX = eq.A != 0 ? -eq.C / (2 * eq.A) : 0;
            double centerY = eq.B != 0 ? -eq.D / (2 * eq.B) : 0;
            
            // Fija la vista en un marco de ±15 unidades alrededor del centro o vértice principal
            _plot.Plot.Axes.SetLimits(centerX - 15, centerX + 15, centerY - 15, centerY + 15);
        }
        
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

    private void RenderConicElements(ConicElements? elements)
    {
        if (elements == null) return;
        
        RenderCenterIfPresent(elements.Center);

        switch (elements)
        {
            case EllipseElements ellipse:
                RenderEllipseElements(ellipse);
                break;
            case ParabolaElements parabola:
                RenderParabolaElements(parabola);
                break;
            case HyperbolaElements hyperbola:
                RenderHyperbolaElements(hyperbola);
                break;
        }
    }

    private void RenderCenterIfPresent(Point2D? center)
    {
        if (center != null)
            AddMarker(center.X, center.Y, Colors.Red, MarkerShape.FilledCircle);
    }

    private void RenderEllipseElements(EllipseElements ellipse)
    {
        RenderPoints(ellipse.Foci, Colors.Orange, MarkerShape.OpenCircle);
        RenderPoints(ellipse.MajorVertices, Colors.Red, MarkerShape.FilledCircle);
        RenderPoints(ellipse.MinorVertices, Colors.Red, MarkerShape.FilledCircle);
    }

    private void RenderParabolaElements(ParabolaElements parabola)
    {
        AddMarker(parabola.Vertex.X, parabola.Vertex.Y, Colors.Red, MarkerShape.FilledCircle);
        AddMarker(parabola.Focus.X, parabola.Focus.Y, Colors.Orange, MarkerShape.OpenCircle);
        
        DrawLine2D(parabola.Directrix, Colors.Gray);
        RenderLatusRectum(parabola);
    }

    private void RenderLatusRectum(ParabolaElements parabola)
    {
        double halfLatusRectum = 2 * RutGeoMath.Abs(parabola.FocalDistance);
        bool isVerticalParabola = RutGeoMath.Abs(parabola.AxisOfSymmetry.A) > 0.5;

        if (isVerticalParabola)
        {
            DrawDashedLine(parabola.Focus.X - halfLatusRectum, parabola.Focus.Y, 
                           parabola.Focus.X + halfLatusRectum, parabola.Focus.Y, Colors.Blue);
        }
        else
        {
            DrawDashedLine(parabola.Focus.X, parabola.Focus.Y - halfLatusRectum, 
                           parabola.Focus.X, parabola.Focus.Y + halfLatusRectum, Colors.Blue);
        }
    }

    private void RenderHyperbolaElements(HyperbolaElements hyperbola)
    {
        RenderPoints(hyperbola.Foci, Colors.Orange, MarkerShape.OpenCircle);
        RenderPoints(hyperbola.Vertices, Colors.Red, MarkerShape.FilledCircle);

        foreach (var asymptote in hyperbola.Asymptotes)
            DrawLine2D(asymptote, Colors.Gray);
    }

    private void RenderPoints(IEnumerable<Point2D> points, Color color, MarkerShape shape)
    {
        foreach (var point in points)
            AddMarker(point.X, point.Y, color, shape);
    }

    private void AddMarker(double x, double y, Color color, MarkerShape shape)
    {
        var marker = _plot.Plot.Add.Marker(x, y);
        marker.Color = color;
        marker.Shape = shape;
    }

    private void DrawDashedLine(double x1, double y1, double x2, double y2, Color color)
    {
        var line = _plot.Plot.Add.Line(x1, y1, x2, y2);
        line.Color = color;
        line.LinePattern = LinePattern.Dashed;
    }

    private void DrawLine2D(Line2D line, Color color)
    {
        bool isVerticalLine = RutGeoMath.Abs(line.B) < 0.0001;

        if (isVerticalLine)
        {
            bool isValidEquation = RutGeoMath.Abs(line.A) > 0.0001;
            if (isValidEquation)
            {
                double x = -line.C / line.A;
                var verticalLine = _plot.Plot.Add.VerticalLine(x);
                verticalLine.Color = color;
                verticalLine.LinePattern = LinePattern.Dashed;
            }
        }
        else
        {
            double leftBoundaryX = -10000;
            double leftBoundaryY = (-line.A * leftBoundaryX - line.C) / line.B;
            
            double rightBoundaryX = 10000;
            double rightBoundaryY = (-line.A * rightBoundaryX - line.C) / line.B;
            
            DrawDashedLine(leftBoundaryX, leftBoundaryY, rightBoundaryX, rightBoundaryY, color);
        }
    }
}
