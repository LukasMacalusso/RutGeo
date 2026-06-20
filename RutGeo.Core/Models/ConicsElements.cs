using System.Collections.Generic;

namespace RutGeo.Core.Models;


public record Point2D(double X, double Y);

public record Line2D(double A, double B, double C)
{
    public string EquationString => $"{A}x + {(B >= 0 ? "+" : "")} {B}y + {(C >= 0 ? "+" : "")} {C} = 0"; 
}

public abstract class ConicsElements
{
    public Point2D? Center { get; init; }
}

public class CircleElements : ConicsElements
{
    public required double Radius { get; init; }
}

public class ParabolaElements : ConicsElements
{
    public required Point2D Vertex { get; init; }
    public required Point2D Focus { get; init; }
    public required Line2D Directrix { get; init; }
    public required Line2D AxisOfSymmetry { get; init; }
    public required double FocalDistance { get; init; } 
}

public class EllipseElements : ConicsElements
{
    public required List<Point2D> Foci { get; init; } = new();
    public required List<Point2D> MajorVertices { get; init; } = new();
    public required List<Point2D> MinorVertices { get; init; } = new();
    public required double MajorAxisLength { get; init; } 
    public required double MinorAxisLength { get; init; } 
    public required double Eccentricity { get; init; }
}

public class HyperbolaElements : ConicsElements
{
    public required List<Point2D> Foci { get; init; } = new();
    public required List<Point2D> Vertices { get; init; } = new();
    public required List<Line2D> Asymptotes { get; init; } = new();
    public required double TransverseAxisLength { get; init; } 
    public required double ConjugateAxisLength { get; init; } 
    public required double Eccentricity { get; init; }
}
