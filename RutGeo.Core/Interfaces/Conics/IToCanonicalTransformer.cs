using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;

namespace RutGeo.Core.Interfaces.Conics;

public interface IToCanonicalTransformer
{
    CanonicalEquation TransformToCanonical(GeneralEquation generalEquation, Conic conic);
}
