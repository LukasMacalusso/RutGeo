using ScottPlot.Avalonia;

namespace RutGeo.UI.Helpers;

public static class PlotRenderer
{
    public static void DrawAxes(this AvaPlot plot)
    {
        var xAxis = plot.Plot.Add.HorizontalLine(0);
        xAxis.Color = ScottPlot.Colors.Gray;
        xAxis.LineStyle.Width = 1;

        var yAxis = plot.Plot.Add.VerticalLine(0);
        yAxis.Color = ScottPlot.Colors.Gray;
        yAxis.LineStyle.Width = 1;
    }

    public static void AddScatterNoMarkers(this AvaPlot plot, double[] xs, double[] ys)
    {
        var scatter = plot.Plot.Add.Scatter(xs, ys);
        scatter.MarkerSize = 0;
    }

    public static void ClearAndReset(this AvaPlot plot)
    {
        plot.Plot.Clear();
        plot.DrawAxes();
        plot.Plot.Axes.AutoScale();
        plot.Refresh();
    }
}
