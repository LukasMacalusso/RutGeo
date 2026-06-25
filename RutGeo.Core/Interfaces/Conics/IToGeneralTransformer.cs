using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;

namespace RutGeo.Core.Interfaces.Conics;

public interface IToGeneralTransformer
{
    GeneralEquation TransformToGeneral(CanonicalEquation canonicalEquation, Conic conic);
}
