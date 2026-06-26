using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using RutGeo.Core.Models;
using RutGeo.UI.Helpers;

namespace RutGeo.UI.ViewModels;

public partial class ConicDefenseViewModel : ObservableObject
{
    public DefenseField Center { get; } = new();
    public DefenseField Radius { get; } = new();
    public DefenseField Vertices { get; } = new();
    public DefenseField Foci { get; } = new();
    public DefenseField Axis { get; } = new();
    public DefenseField Directrix { get; } = new();

    [ObservableProperty] private string _centerLabel = "Centro (h, k):";
    [ObservableProperty] private string _axisLabel = "Eje mayor / Eje menor:";
    [ObservableProperty] private string _directrixLabel = "Directriz:";
    
    public void SetExpectedValues(ConicElements? elements)
    {
        ResetFields();
        if (elements == null) return;

        Center.ExpectedValue = UiFormat.Point(elements.Center);

        switch (elements)
        {
            case CircleElements c: SetupCircle(c); break;
            case EllipseElements e: SetupEllipse(e); break;
            case HyperbolaElements h: SetupHyperbola(h); break;
            case ParabolaElements p: SetupParabola(p); break;
        }
    }

    private void SetupCircle(CircleElements c)
    {
        Radius.IsVisible = true;
        Radius.ExpectedValue = $"{c.Radius:F2}";
    }

    private void SetupEllipse(EllipseElements e)
    {
        Vertices.IsVisible = true;
        Foci.IsVisible = true;
        Axis.IsVisible = true;

        Foci.ExpectedValue = string.Join("; ", e.Foci.Select(f => UiFormat.Point(f)));
        Vertices.ExpectedValue = $"Mayores: {string.Join("; ", e.MajorVertices.Select(v => UiFormat.Point(v)))}" +
            $"\nMenores: {string.Join("; ", e.MinorVertices.Select(v => UiFormat.Point(v)))}";
        Axis.ExpectedValue = $"Mayor = {e.MajorAxisLength:F2}\nMenor = {e.MinorAxisLength:F2}";
    }

    private void SetupHyperbola(HyperbolaElements h)
    {
        Vertices.IsVisible = true;
        Foci.IsVisible = true;
        Axis.IsVisible = true;
        Directrix.IsVisible = true;
        AxisLabel = "Eje transverso / Eje conjugado:";
        DirectrixLabel = "Asíntotas:";

        Foci.ExpectedValue = string.Join("; ", h.Foci.Select(f => UiFormat.Point(f)));
        Vertices.ExpectedValue = string.Join("; ", h.Vertices.Select(v => UiFormat.Point(v)));
        Axis.ExpectedValue = $"Transverso = {h.TransverseAxisLength:F2}\nConjugado = {h.ConjugateAxisLength:F2}";
        Directrix.ExpectedValue = string.Join("\n", h.Asymptotes.Select(a => a.EquationString));
    }

    private void SetupParabola(ParabolaElements p)
    {
        Foci.IsVisible = true;
        Axis.IsVisible = true;
        Directrix.IsVisible = true;
        CenterLabel = "Vértice (h, k):";

        Center.ExpectedValue = UiFormat.Point(p.Vertex);
        Foci.ExpectedValue = UiFormat.Point(p.Focus);
        Axis.ExpectedValue = p.AxisOfSymmetry.EquationString;
        Directrix.ExpectedValue = p.Directrix.EquationString;
    }

    public void Corroborate()
    {
        Center.Corroborate();
        Radius.Corroborate();
        Vertices.Corroborate();
        Foci.Corroborate();
        Axis.Corroborate();
        Directrix.Corroborate();
    }
    
    private void ResetFields()
    {
        Radius.IsVisible = false;
        Vertices.IsVisible = false;
        Foci.IsVisible = false;
        Axis.IsVisible = false;
        Directrix.IsVisible = false;
        CenterLabel = "Centro (h, k):";
        AxisLabel = "Eje mayor / Eje menor:";
        DirectrixLabel = "Directriz:";

        Center.ExpectedValue = "";
        Center.Status = "";
        Radius.ExpectedValue = "";
        Radius.Status = "";
        Vertices.ExpectedValue = "";
        Vertices.Status = "";
        Foci.ExpectedValue = "";
        Foci.Status = "";
        Axis.ExpectedValue = "";
        Axis.Status = "";
        Directrix.ExpectedValue = "";
        Directrix.Status = "";
    }
}
