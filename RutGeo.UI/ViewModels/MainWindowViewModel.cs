using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RutGeo.Core.Interfaces.Common;
using RutGeo.Core.Interfaces.Conics;
using RutGeo.Core.Interfaces.Functions;
using RutGeo.Core.Interfaces.Validation;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Results;

namespace RutGeo.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private RutValidatorResult? _validatorResult;
    [ObservableProperty] private string _rutText = string.Empty;
    [ObservableProperty] private string _validationMessage = string.Empty;
    [ObservableProperty] private bool _isSolutionsVisible;

    public ConicAnalysisViewModel ConicAnalysisVM { get; } = new();
    public LimitAnalysisViewModel LimitAnalysisVM { get; } = new();

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

        ForwardChildChanges();
    }

    private void ForwardChildChanges()
    {
        PropertyChangedEventHandler forward = (_, e) =>
        {
            if (e.PropertyName != null)
                OnPropertyChanged(e.PropertyName);
        };
        ConicAnalysisVM.PropertyChanged += forward;
        LimitAnalysisVM.PropertyChanged += forward;
    }

    partial void OnValidatorResultChanged(RutValidatorResult? value)
    {
        CorroborateAllCommand.NotifyCanExecuteChanged();
        ToggleSolutionsCommand.NotifyCanExecuteChanged();
    }

    private void CleanResults()
    {
        IsSolutionsVisible = false;
        _explanationLog.Clear();
        ValidatorResult = null;
        ConicAnalysisVM.Clean();
        LimitAnalysisVM.Clean();
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
        ConicAnalysisVM.SetResult(conicResult);

        LimitOrchestrationResult limitResult = _limitOrchestrator.Execute(ValidatorResult);
        LimitAnalysisVM.SetResult(limitResult);
    }

    [RelayCommand(CanExecute = nameof(CanCorroborate))]
    private void CorroborateAll()
    {
        ConicAnalysisVM.Corroborate();
        LimitAnalysisVM.Corroborate();
    }

    private bool CanCorroborate() => ValidatorResult?.IsValid == true;

    [RelayCommand]
    private void ClearAll()
    {
        CleanResults();
        RutText = string.Empty;
        ValidationMessage = string.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanToggleSolutions))]
    private void ToggleSolutions()
    {
        IsSolutionsVisible = !IsSolutionsVisible;
    }

    private bool CanToggleSolutions() => ValidatorResult?.IsValid == true;
}
