using System;
using Avalonia.Controls;
using Avalonia.Threading;
using ScottPlot.Avalonia;
using RutGeo.Core.Models;
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

    public void UpdatePlot(GeneralEquation eq, Conic conic, CanonicalEquation canon)
    {
        _plot.Plot.Clear();
        _plot.Plot.Axes.AutoScale();
        _plot.Refresh();
    }

    public void SwitchToConics() { }
    public void SwitchToLimits() { }
}