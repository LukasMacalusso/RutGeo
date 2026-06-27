using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using RutGeo.Core.Models.Results;

namespace RutGeo.UI.ViewModels;

public partial class LimitAnalysisViewModel : ViewModelBase
{
    [ObservableProperty] private LimitAnalysisResult? _limitResult;
    [ObservableProperty] private string _limitCaseDescription = string.Empty;
    [ObservableProperty] private string _limitFunctionExpression = string.Empty;
    [ObservableProperty] private string _limitSelectionRule = string.Empty;

    public ObservableCollection<ValuePoint> LeftValues { get; } = new();
    public ObservableCollection<ValuePoint> RightValues { get; } = new();

    public LimitDefenseViewModel LimitVM { get; } = new();

    public void SetResult(LimitOrchestrationResult result)
    {
        LimitResult = result.AnalysisResult;
        LimitCaseDescription = result.CaseDescription;
        LimitFunctionExpression = result.FunctionExpression;
        LimitSelectionRule = result.SelectionRule;

        LeftValues.Clear();
        RightValues.Clear();
        
        for (int i = 0; i < result.LeftXValues.Length; i++)
            LeftValues.Add(new ValuePoint(result.LeftXValues[i], result.LeftYFormatted[i]));
        for (int i = 0; i < result.RightXValues.Length; i++)
            RightValues.Add(new ValuePoint(result.RightXValues[i], result.RightYFormatted[i]));

        LimitVM.SetExpectedValues(result.AnalysisResult);
    }

    public void Corroborate()
    {
        LimitVM.SetExpectedValues(LimitResult);
        LimitVM.Corroborate();
    }

    public void Clean()
    {
        LimitResult = null;
        LimitCaseDescription = string.Empty;
        LimitFunctionExpression = string.Empty;
        LimitSelectionRule = string.Empty;
        LeftValues.Clear();
        RightValues.Clear();
        LimitVM.SetExpectedValues(null);
        LimitVM.LeftLimit.UserValue = "";
        LimitVM.RightLimit.UserValue = "";
        LimitVM.LimitExists.UserValue = "";
        LimitVM.ValueFA.UserValue = "";
        LimitVM.Continuous.UserValue = "";
        LimitVM.Discontinuity.UserValue = "";
    }
}
