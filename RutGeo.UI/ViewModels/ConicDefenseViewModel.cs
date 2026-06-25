using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using RutGeo.Core.Models;

namespace RutGeo.UI.ViewModels;

public partial class ConicDefenseViewModel : ObservableObject
{
    public DefenseField Center { get; } = new();
    public DefenseField Radius { get; } = new();
    public DefenseField Vertices { get; } = new();
    public DefenseField Foci { get; } = new();
    public DefenseField Axis { get; } = new();
    public DefenseField Directive { get; } = new();

    [ObservableProperty] private string _centerLabel = "Centro (h, k):";
    [ObservableProperty] private string _axisLabel = "Eje mayor / Eje menor:";
    [ObservableProperty] private string _directrixLabel = "Directriz:";

    public void SetExpectedValues(ConicElements? elements)
    {
        Radius.IsVisible = false;
        Vertices.IsVisible = false;
        Foci.IsVisible = false;
        Axis.IsVisible = false;
        Directive.IsVisible = false;
        CenterLabel = "Centro (h, k):";
        AxisLabel = "Eje mayor / Eje menor:";
        DirectrixLabel = "Directriz:";

        Center.ExpectedValue = "";
        Radius.ExpectedValue = "";
        Vertices.ExpectedValue = "";
        Foci.ExpectedValue = "";
        Axis.ExpectedValue = "";
        Directive.ExpectedValue = "";

        if (elements == null)
        {
            Center.ExpectedValue = "";
            return;
        }

        Center.ExpectedValue = FormatPoint(elements.Center);

        switch (elements)
        {
            case CircleElements c:
                Radius.IsVisible = true;
                Radius.ExpectedValue = $"{c.Radius:F2}";
                break;

            case EllipseElements e:
                Vertices.IsVisible = true;
                Foci.IsVisible = true;
                Axis.IsVisible = true;

                Foci.ExpectedValue = string.Join("; ", e.Foci.Select(f => FormatPoint(f)));
                Vertices.ExpectedValue = $"Mayores: {string.Join("; ", e.MajorVertices.Select(v => FormatPoint(v)))}" +
                    $"\nMenores: {string.Join("; ", e.MinorVertices.Select(v => FormatPoint(v)))}";
                Axis.ExpectedValue = $"Mayor = {e.MajorAxisLength:F2}\nMenor = {e.MinorAxisLength:F2}";
                break;

            case HyperbolaElements h:
                Vertices.IsVisible = true;
                Foci.IsVisible = true;
                Axis.IsVisible = true;
                Directive.IsVisible = true;
                AxisLabel = "Eje transverso / Eje conjugado:";
                DirectrixLabel = "Asíntotas:";

                Foci.ExpectedValue = string.Join("; ", h.Foci.Select(f => FormatPoint(f)));
                Vertices.ExpectedValue = string.Join("; ", h.Vertices.Select(v => FormatPoint(v)));
                Axis.ExpectedValue = $"Transverso = {h.TransverseAxisLength:F2}\nConjugado = {h.ConjugateAxisLength:F2}";
                Directive.ExpectedValue = string.Join("; ", h.Asymptotes.Select(a => a.EquationString));
                break;

            case ParabolaElements p:
                Foci.IsVisible = true;
                Axis.IsVisible = true;
                Directive.IsVisible = true;
                CenterLabel = "Vértice (h, k):";

                Center.ExpectedValue = FormatPoint(p.Vertex);
                Foci.ExpectedValue = FormatPoint(p.Focus);
                Axis.ExpectedValue = p.AxisOfSymmetry.EquationString;
                Directive.ExpectedValue = p.Directrix.EquationString;
                break;
        }
    }

    private static string FormatPoint(Point2D? p)
    {
        return p == null ? "-" : $"({p.X:F2}, {p.Y:F2})";
    }

    public void Corroborate()
    {
        Center.Corroborate();
        Radius.Corroborate();
        Vertices.Corroborate();
        Foci.Corroborate();
        Axis.Corroborate();
        Directive.Corroborate();
    }
}
