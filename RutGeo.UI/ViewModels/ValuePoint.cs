namespace RutGeo.UI.ViewModels;

public class ValuePoint
{
    public double X { get; }
    public string DisplayX => X.ToString("F4");
    private readonly string _fx;
    public string Fx => _fx;

    public ValuePoint(double x, string fx)
    {
        X = x;
        _fx = fx;
    }
}
