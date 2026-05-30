using RutGeo.Core.Models;
using RutGeo.Core.Services;

namespace RutGeo.Core.Interfaces;

public interface IEquationTransformer
{
    CanonicalEquation TransformToCanonical(GeneralEquation generalEquation, Conic conic, IExplanationLog log);
    GeneralEquation TransformToGeneral(CanonicalEquation canonicalEquation, Conic conic, IExplanationLog log);

}
