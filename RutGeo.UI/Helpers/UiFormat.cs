using RutGeo.Core.Models;

namespace RutGeo.UI.Helpers;

public static class UiFormat
{
    public static string Point(Point2D? p)
    {
        return p == null ? "-" : $"({p.X:F2}, {p.Y:F2})";
    }
}
