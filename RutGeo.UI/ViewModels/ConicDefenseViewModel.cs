using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using RutGeo.Core.Models;

namespace RutGeo.UI.ViewModels;

public partial class ConicDefenseViewModel : ObservableObject
{
    [ObservableProperty] private string _userCenter = string.Empty;
    [ObservableProperty] private string _userVertices = string.Empty;
    [ObservableProperty] private string _userFocals = string.Empty;
    [ObservableProperty] private string _userAxis = string.Empty;
    [ObservableProperty] private string _userDirective = string.Empty;

    [ObservableProperty] private string _centerStatus = string.Empty;
    [ObservableProperty] private string _verticesStatus = string.Empty;
    [ObservableProperty] private string _focalsStatus = string.Empty;
    [ObservableProperty] private string _axisStatus = string.Empty;
    [ObservableProperty] private string _directiveStatus = string.Empty;
    [ObservableProperty] private string _justificationStatus = string.Empty;

    [ObservableProperty] private string _expectedCenter = "";
    [ObservableProperty] private string _expectedVertices = "";
    [ObservableProperty] private string _expectedFocals = "";
    [ObservableProperty] private string _expectedAxis = "";
    [ObservableProperty] private string _expectedDirective = "";

    public void SetExpectedValues(ConicElements? elements)
    {
        if (elements == null)
        {
            ExpectedCenter = ExpectedVertices = ExpectedFocals = ExpectedAxis = ExpectedDirective = "";
            return;
        }

        ExpectedCenter = FormatPoint(elements.Center);

        switch (elements)
        {
            case CircleElements c:
                ExpectedVertices = $"radio = {c.Radius:F2}";
                ExpectedFocals = "-";
                ExpectedAxis = $"radio = {c.Radius:F2}";
                ExpectedDirective = "-";
                break;

            case EllipseElements e:
                var fociStr = string.Join("; ", e.Foci.Select(f => FormatPoint(f)));
                ExpectedFocals = fociStr;
                ExpectedVertices = $"Mayores: {string.Join("; ", e.MajorVertices.Select(v => FormatPoint(v)))}";
                var minorStr = string.Join("; ", e.MinorVertices.Select(v => FormatPoint(v)));
                ExpectedVertices += $"\nMenores: {minorStr}";
                ExpectedAxis = $"Mayor = {e.MajorAxisLength:F2}\nMenor = {e.MinorAxisLength:F2}";
                ExpectedDirective = "-";
                break;

            case HyperbolaElements h:
                ExpectedFocals = string.Join("; ", h.Foci.Select(f => FormatPoint(f)));
                ExpectedVertices = string.Join("; ", h.Vertices.Select(v => FormatPoint(v)));
                ExpectedAxis = $"Transverso = {h.TransverseAxisLength:F2}\nConjugado = {h.ConjugateAxisLength:F2}";
                ExpectedDirective = string.Join("; ", h.Asymptotes.Select(a => a.EquationString));
                break;

            case ParabolaElements p:
                ExpectedVertices = FormatPoint(p.Vertex);
                ExpectedFocals = FormatPoint(p.Focus);
                ExpectedAxis = p.AxisOfSymmetry.EquationString;
                ExpectedDirective = p.Directrix.EquationString;
                break;
        }
    }

    private static string FormatPoint(Point2D? p)
    {
        return p == null ? "-" : $"({p.X:F2}, {p.Y:F2})";
    }

    public void Corroborate()
    {
        CenterStatus = Compare(UserCenter, ExpectedCenter) ? "✓" : "✗";
        VerticesStatus = Compare(UserVertices, ExpectedVertices) ? "✓" : "✗";
        FocalsStatus = Compare(UserFocals, ExpectedFocals) ? "✓" : "✗";
        AxisStatus = Compare(UserAxis, ExpectedAxis) ? "✓" : "✗";
        DirectiveStatus = Compare(UserDirective, ExpectedDirective) ? "✓" : "✗";
        JustificationStatus = "✓";
    }

    private static bool Compare(string user, string expected)
    {
        if (string.IsNullOrWhiteSpace(user)) return false;
        string a = user.Trim().ToLowerInvariant().Replace(" ", "");
        string b = expected.Trim().ToLowerInvariant().Replace(" ", "");
        return a == b;
    }
}
