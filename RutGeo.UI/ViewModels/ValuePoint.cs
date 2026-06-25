namespace RutGeo.UI.ViewModels;

public record ValuePoint(double X, string Fx)
{
    public string DisplayX => X.ToString("F4");
}
