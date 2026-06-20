using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RutGeo.Core.Interfaces;
using RutGeo.Core.Models;
using RutGeo.Core.Services;

namespace RutGeo.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private RutValidatorResult? _validatorResult; 
    [ObservableProperty] private GeneralEquation? _generalEquation;
    [ObservableProperty] private Conic? _conic;
    [ObservableProperty] private CanonicalEquation? _canonicalEquation;
    
    [ObservableProperty] private string _rutText = string.Empty;
    [ObservableProperty] private string _validationMessage = string.Empty;
    
    [ObservableProperty] private string _calculatedCenter = "-";
    [ObservableProperty] private string _calculatedVertices = "-";
    [ObservableProperty] private string _calculatedFocals = "-";
    [ObservableProperty] private string _calculatedAxis = "-";
    [ObservableProperty] private string _calculatedDirective = "-";
    
    private readonly IRutEquationGenerator _rutEquationGenerator;
    private readonly IEquationTransformer _equationTransformer;
    private readonly IExplanationLog _explanationLog;
    private readonly IRutValidator _rutValidator;
    
    public MainWindowViewModel(   
        IRutEquationGenerator rutEquationGenerator,
        IEquationTransformer equationTransformer,
        IExplanationLog explanationLog,
        IRutValidator rutValidator)
    {
        _rutEquationGenerator = rutEquationGenerator;
        _equationTransformer = equationTransformer;
        _rutValidator = rutValidator;
        _explanationLog = explanationLog;
    }

    private void CleanResults()
    {
        _explanationLog.Clear();
        ValidatorResult = null;
        GeneralEquation = null;
        CanonicalEquation = null;
        Conic = null;
        CalculatedCenter = "-";
        CalculatedVertices = "-";
        CalculatedFocals = "-";
        CalculatedAxis = "-";
        CalculatedDirective = "-";
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
        
        ValidatorResult = _rutValidator.Validate(RutText, _explanationLog);
        
        if (!ValidatorResult.IsValid) 
        {
            ValidationMessage = $"RUT inválido\n\n{_explanationLog.GetFullLog()}";
            return; 
        }

        ValidationMessage = $"RUT correcto: {ValidatorResult.RutBody}-{ValidatorResult.Dv}\n\n{_explanationLog.GetFullLog()}";
        GeneralEquation = _rutEquationGenerator.GenerateGeneralEquation(ValidatorResult);
        Conic = new Conic(GeneralEquation);
        CanonicalEquation = _equationTransformer.TransformToCanonical(GeneralEquation, Conic, _explanationLog);
    }

    private void CalculateGeometricElements()
    {
    }
}