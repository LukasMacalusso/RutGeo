using Avalonia.Controls;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;
using RutGeo.UI.Helpers;

namespace RutGeo.UI.Views;

public partial class Graphic : UserControl
{
    private readonly ConicPlotService _conicPlotter;
    private readonly LimitPlotService _limitPlotter;

    public Graphic()
    {
        InitializeComponent();
        var plot = this.Find<ScottPlot.Avalonia.AvaPlot>("MainPlot")!;
        _conicPlotter = new ConicPlotService(plot);
        _limitPlotter = new LimitPlotService(plot);
    }

    public void ClearGraph()
    {
        var plot = this.Find<ScottPlot.Avalonia.AvaPlot>("MainPlot")!;
        plot.ClearAndReset();
    }

    public void UpdatePlot(GeneralEquation eq, Conic conic, CanonicalEquation canon)
    {
        _conicPlotter.Plot(eq, conic, canon);
    }

    public void UpdateLimitPlot(int condition, int a, int[] digits)
    {
        _limitPlotter.Plot(condition, a, digits);
    }
}
