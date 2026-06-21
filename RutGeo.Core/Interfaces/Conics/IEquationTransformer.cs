using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Types;

namespace RutGeo.Core.Interfaces.Conics;

public interface IEquationTransformer
{
    CanonicalEquation TransformToCanonical(GeneralEquation generalEquation, Conic conic);
    GeneralEquation TransformToGeneral(CanonicalEquation canonicalEquation, Conic conic);

}
