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
        
        var mainColor = Colors.Red;
        var subColor = Colors.Orange;
        
        if (elements.Center != null)
        {
            var m = _plot.Plot.Add.Marker(elements.Center.X, elements.Center.Y);
            m.Color = mainColor;
            m.Shape = MarkerShape.FilledCircle;
        }

        if (elements is EllipseElements ellipse)
        {
            foreach (var f in ellipse.Foci)
            {
                var m = _plot.Plot.Add.Marker(f.X, f.Y);
                m.Color = subColor;
                m.Shape = MarkerShape.OpenCircle;
            }
            foreach (var v in ellipse.MajorVertices)
            {
                var m = _plot.Plot.Add.Marker(v.X, v.Y);
                m.Color = mainColor;
                m.Shape = MarkerShape.FilledCircle;
            }
            foreach (var v in ellipse.MinorVertices)
            {
                var m = _plot.Plot.Add.Marker(v.X, v.Y);
                m.Color = mainColor;
                m.Shape = MarkerShape.FilledCircle;
            }
        }
        else if (elements is ParabolaElements parabola)
        {
            var m = _plot.Plot.Add.Marker(parabola.Vertex.X, parabola.Vertex.Y);
            m.Color = mainColor;
            m.Shape = MarkerShape.FilledCircle;

            var f = _plot.Plot.Add.Marker(parabola.Focus.X, parabola.Focus.Y);
            f.Color = subColor;
            f.Shape = MarkerShape.OpenCircle;

            DrawLine2D(parabola.Directrix, Colors.Gray);
            
            double halfLr = 2 * RutGeoMath.Abs(parabola.FocalDistance);
            if (RutGeoMath.Abs(parabola.AxisOfSymmetry.A) > 0.5)
            {
                var lr = _plot.Plot.Add.Line(parabola.Focus.X - halfLr, parabola.Focus.Y, parabola.Focus.X + halfLr, parabola.Focus.Y);
                lr.Color = Colors.Blue;
                lr.LinePattern = LinePattern.Dashed;
            }
            else
            {
                var lr = _plot.Plot.Add.Line(parabola.Focus.X, parabola.Focus.Y - halfLr, parabola.Focus.X, parabola.Focus.Y + halfLr);
                lr.Color = Colors.Blue;
                lr.LinePattern = LinePattern.Dashed;
            }
        }
        else if (elements is HyperbolaElements hyperbola)
        {
            foreach (var f in hyperbola.Foci)
            {
                var m = _plot.Plot.Add.Marker(f.X, f.Y);
                m.Color = subColor;
                m.Shape = MarkerShape.OpenCircle;
            }
            foreach (var v in hyperbola.Vertices)
            {
                var m = _plot.Plot.Add.Marker(v.X, v.Y);
                m.Color = mainColor;
                m.Shape = MarkerShape.FilledCircle;
            }
            foreach (var asympt in hyperbola.Asymptotes)
            {
                DrawLine2D(asympt, Colors.Gray);
            }
        }
    }

    private void DrawLine2D(Line2D line, Color color)
    {
        
        if (RutGeoMath.Abs(line.B) < 0.0001)
        {
            
            if (RutGeoMath.Abs(line.A) > 0.0001)
            {
                double x = -line.C / line.A;
                var vl = _plot.Plot.Add.VerticalLine(x);
                vl.Color = color;
                vl.LinePattern = LinePattern.Dashed;
            }
        }
        else
        {
            
            double x1 = -10000;
            double y1 = (-line.A * x1 - line.C) / line.B;
            double x2 = 10000;
            double y2 = (-line.A * x2 - line.C) / line.B;
            
            var l = _plot.Plot.Add.Line(x1, y1, x2, y2);
            l.Color = color;
            l.LinePattern = LinePattern.Dashed;
        }
    }
}
