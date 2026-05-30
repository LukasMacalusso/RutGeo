using RutGeo.Core.Services;

namespace RutGeo.Core.Interfaces;

public interface ITransformer
{
    Equation Equation { get; }
    Conic Conic { get; }
    string StepByStep { get; }
}
