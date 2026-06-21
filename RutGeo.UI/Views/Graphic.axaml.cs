using System;
using System.Collections.Generic;
using Avalonia.Controls;
using ScottPlot.Avalonia;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Types;
using RutGeo.Core.Services;

namespace RutGeo.UI.Views;

public partial class Graphic : UserControl
{
    private AvaPlot _plot;

    public Graphic()
    {
        InitializeComponent();
        _plot = this.Find<AvaPlot>("MainPlot")!;
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

    // TEMPORAL
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
                    double h = -eq.C / (2 * eq.A);
                    double k = -eq.D / (2 * eq.A);
                    double r = Math.Sqrt(Math.Max(0, h * h + k * k - eq.E / eq.A));
                    for (double t = 0; t <= 2 * Math.PI; t += 0.05)
                    {
                        xs.Add(h + r * Math.Cos(t));
                        ys.Add(k + r * Math.Sin(t));
                    }
                    break;

                case ConicType.Elipse:
                    double eh = -eq.C / (2 * eq.A);
                    double ek = -eq.D / (2 * eq.B);
                    double rhsE = -eq.E + eq.A * eh * eh + eq.B * ek * ek;
                    double ea = Math.Sqrt(Math.Max(0, rhsE / eq.A));
                    double eb = Math.Sqrt(Math.Max(0, rhsE / eq.B));
                    for (double t = 0; t <= 2 * Math.PI; t += 0.05)
                    {
                        xs.Add(eh + ea * Math.Cos(t));
                        ys.Add(ek + eb * Math.Sin(t));
                    }
                    break;

                case ConicType.Hyperbola:
                    double hh = -eq.C / (2 * eq.A);
                    double hk = -eq.D / (2 * eq.B);
                    double rhsH = -eq.E + eq.A * hh * hh + eq.B * hk * hk;
                    if (rhsH > 0)
                    {
                        double ah = Math.Sqrt(rhsH / eq.A);
                        double bh = Math.Sqrt(Math.Abs(rhsH / eq.B));
                        for (double t = -1.5; t <= 1.5; t += 0.05)
                        {
                            double cosT = Math.Cos(t);
                            if (Math.Abs(cosT) > 0.01) {
                                xs.Add(hh + ah / cosT);
                                ys.Add(hk + bh * Math.Tan(t));
                                xs.Add(hh - ah / cosT);
                                ys.Add(hk + bh * Math.Tan(t));
                            }
                        }
                    }
                    else
                    {
                        double ah = Math.Sqrt(Math.Abs(rhsH / eq.A));
                        double bh = Math.Sqrt(Math.Abs(rhsH / eq.B));
                        for (double t = -1.5; t <= 1.5; t += 0.05)
                        {
                            double cosT = Math.Cos(t);
                            if (Math.Abs(cosT) > 0.01) {
                                xs.Add(hh + ah * Math.Tan(t));
                                ys.Add(hk + bh / cosT);
                                xs.Add(hh + ah * Math.Tan(t));
                                ys.Add(hk - bh / cosT);
                            }
                        }
                    }
                    break;

                case ConicType.Parabola:
                    if (Math.Abs(eq.B) < 1e-6)
                    {
                        for (double x = -10; x <= 10; x += 0.1)
                        {
                            xs.Add(x);
                            ys.Add((-eq.A / eq.D) * x * x + (-eq.C / eq.D) * x + (-eq.E / eq.D));
                        }
                    }
                    else
                    {
                        for (double y = -10; y <= 10; y += 0.1)
                        {
                            ys.Add(y);
                            xs.Add((-eq.B / eq.C) * y * y + (-eq.D / eq.C) * y + (-eq.E / eq.C));
                        }
                    }
                    break;
            }
        }
        catch
        {
        }

        if (xs.Count > 0)
        {
            _plot.Plot.Add.Scatter(xs.ToArray(), ys.ToArray());
        }

        _plot.Plot.Axes.AutoScale();
        _plot.Refresh();
    }
}
