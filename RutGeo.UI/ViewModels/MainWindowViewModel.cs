using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RutGeo.Core.Interfaces.Common;
using RutGeo.Core.Interfaces.Conics;
using RutGeo.Core.Interfaces.Functions;
using RutGeo.Core.Interfaces.Validation;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Results;

namespace RutGeo.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private RutValidatorResult? _validatorResult;
    [ObservableProperty] private GeneralEquation? _generalEquation;
    [ObservableProperty] private Conic? _conic;
    [ObservableProperty] private CanonicalEquation? _canonicalEquation;

    [ObservableProperty] private string _rutText = string.Empty;
    [ObservableProperty] private string _validationMessage = string.Empty;

    [ObservableProperty] private ConicDefenseViewModel _conicVM;
    [ObservableProperty] private LimitDefenseViewModel _limitVM;

    [ObservableProperty] private bool _isSolutionsVisible;

    [ObservableProperty] private string _conicTransformationSteps = string.Empty;

    [ObservableProperty] private LimitAnalysisResult? _limitResult;
    [ObservableProperty] private string _limitCaseDescription = string.Empty;
    [ObservableProperty] private string _limitFunctionExpression = string.Empty;
    [ObservableProperty] private string _limitSelectionRule = string.Empty;

    public ObservableCollection<ValuePoint> LeftValues { get; } = new();
    public ObservableCollection<ValuePoint> RightValues { get; } = new();

    private readonly IRutValidator _rutValidator;
    private readonly IExplanationLogger _explanationLog;
    private readonly IConicOrchestrator _conicOrchestrator;
    private readonly ILimitOrchestrator _limitOrchestrator;

    public MainWindowViewModel(
        IRutValidator rutValidator,
        IExplanationLogger explanationLog,
        IConicOrchestrator conicOrchestrator,
        ILimitOrchestrator limitOrchestrator)
    {
        _rutValidator = rutValidator;
        _explanationLog = explanationLog;
        _conicOrchestrator = conicOrchestrator;
        _limitOrchestrator = limitOrchestrator;

        _conicVM = new ConicDefenseViewModel();
        _limitVM = new LimitDefenseViewModel();
    }

    private void CleanResults()
    {
        _explanationLog.Clear();
        ValidatorResult = null;
        GeneralEquation = null;
        CanonicalEquation = null;
        Conic = null;
        LimitResult = null;
        LimitCaseDescription = string.Empty;
        LimitFunctionExpression = string.Empty;
        LimitSelectionRule = string.Empty;
        LeftValues.Clear();
        RightValues.Clear();

        ConicVM = new ConicDefenseViewModel();
        LimitVM = new LimitDefenseViewModel();
        ConicTransformationSteps = string.Empty;
    }

    [RelayCommand]
    private void Analyze()
    {
        CleanResults();

        if (string.IsNullOrWhiteSpace(RutText))
        {
            ValidationMessage = "No se ingresó ningún RUT";
            return;
        }

        ValidatorResult = _rutValidator.Validate(RutText);

        if (ValidatorResult == null || !ValidatorResult.IsValid)
        {
            ValidationMessage = $"RUT inválido\n\n{_explanationLog.GetFullLog()}";
            return;
        }

        ValidationMessage = $"RUT correcto: {ValidatorResult.RutBody}-{ValidatorResult.Dv}\n\n{_explanationLog.GetFullLog()}";

        ConicOrchestrationResult conicResult = _conicOrchestrator.Execute(ValidatorResult);
        GeneralEquation = conicResult.GeneralEquation;
        Conic = conicResult.Conic;
        CanonicalEquation = conicResult.CanonicalEquation;
        ConicTransformationSteps = conicResult.TransformationSteps;

        LimitOrchestrationResult limitResult = _limitOrchestrator.Execute(ValidatorResult);
        LimitResult = limitResult.AnalysisResult;
        LimitCaseDescription = limitResult.CaseDescription;
        LimitFunctionExpression = limitResult.FunctionExpression;
        LimitSelectionRule = limitResult.SelectionRule;

        LeftValues.Clear();
        RightValues.Clear();
        for (int i = 0; i < limitResult.LeftXValues.Length; i++)
            LeftValues.Add(new ValuePoint(limitResult.LeftXValues[i], limitResult.LeftYFormatted[i]));
        for (int i = 0; i < limitResult.RightXValues.Length; i++)
            RightValues.Add(new ValuePoint(limitResult.RightXValues[i], limitResult.RightYFormatted[i]));

        ConicVM.SetExpectedValues(CanonicalEquation?.Elements);
        LimitVM.SetExpectedValues(LimitResult);
    }

    [RelayCommand]
    private void CorroborateAll()
    {
        ConicVM.SetExpectedValues(CanonicalEquation?.Elements);
        ConicVM.Corroborate();

        LimitVM.SetExpectedValues(LimitResult);
        LimitVM.Corroborate();
    }

    [RelayCommand]
    private void ClearAll()
    {
        CleanResults();
        RutText = string.Empty;
        ValidationMessage = string.Empty;
    }

    [RelayCommand]
    private void ToggleSolutions()
    {
        IsSolutionsVisible = !IsSolutionsVisible;
    }
}
