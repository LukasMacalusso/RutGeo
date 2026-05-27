using RutGeo.Core.Interfaces;

namespace RutGeo.Core.Services;

public class DiTestRutValidator : IRutValidator
{
    public bool IsValid(string rut) => true;
}