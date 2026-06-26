using CommunityToolkit.Mvvm.ComponentModel;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Results;

namespace RutGeo.UI.ViewModels;

public partial class ConicAnalysisViewModel : ViewModelBase
{
    [ObservableProperty] private GeneralEquation? _generalEquation;
    [ObservableProperty] private Conic? _conic;
    [ObservableProperty] private CanonicalEquation? _canonicalEquation;
    [ObservableProperty] private string _conicTransformationSteps = string.Empty;

    public ConicDefenseViewModel ConicVM { get; } = new();

    public void SetResult(ConicOrchestrationResult result)
    {
        GeneralEquation = result.GeneralEquation;
        Conic = result.Conic;
        CanonicalEquation = result.CanonicalEquation;
        ConicTransformationSteps = result.TransformationSteps;
        ConicVM.SetExpectedValues(result.CanonicalEquation?.Elements);
    }

    public void Corroborate()
    {
        ConicVM.SetExpectedValues(CanonicalEquation?.Elements);
        ConicVM.Corroborate();
    }

    public void Clean()
    {
        GeneralEquation = null;
        Conic = null;
        CanonicalEquation = null;
        ConicTransformationSteps = string.Empty;
        ConicVM.SetExpectedValues(null);
        ConicVM.Center.UserValue = "";
        ConicVM.Radius.UserValue = "";
        ConicVM.Vertices.UserValue = "";
        ConicVM.Foci.UserValue = "";
        ConicVM.Axis.UserValue = "";
        ConicVM.Directrix.UserValue = "";
    }
}
