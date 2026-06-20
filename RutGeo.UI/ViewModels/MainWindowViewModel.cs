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

    [ObservableProperty] private ConicDefenseViewModel _conicVM;
    [ObservableProperty] private LimitDefenseViewModel _limitVM;
    
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
        _explanationLog = explanationLog;
        _rutValidator = rutValidator;

        _conicVM = new ConicDefenseViewModel();
        _limitVM = new LimitDefenseViewModel();

        LoadMockConic();
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

        ConicVM = new ConicDefenseViewModel();
        LimitVM = new LimitDefenseViewModel();
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
        
        if (ValidatorResult == null || !ValidatorResult.IsValid) 
        {
            ValidationMessage = $"RUT inválido\n\n{_explanationLog.GetFullLog()}";
            return; 
        }
        
        ValidationMessage = $"RUT correcto: {ValidatorResult.RutBody}-{ValidatorResult.Dv}\n\n{_explanationLog.GetFullLog()}";
        GeneralEquation = _rutEquationGenerator.GenerateGeneralEquation(ValidatorResult);
        if (GeneralEquation != null)
        {
            Conic = new Conic(GeneralEquation);
            CanonicalEquation = _equationTransformer.TransformToCanonical(GeneralEquation, Conic, _explanationLog);
        }
    }

    [RelayCommand]
    private void LoadMockConic()
    {
        CleanResults();
        Random rand = new Random();
        int type = rand.Next(0, 4);

        switch (type)
        {
            case 0: 
                GeneralEquation = new GeneralEquation { A = 1, B = 1, C = 0, D = 0, E = -4 };
                break;
            case 1: 
                GeneralEquation = new GeneralEquation { A = 9, B = 4, C = 0, D = 0, E = -36 };
                break;
            case 2: 
                GeneralEquation = new GeneralEquation { A = 9, B = -4, C = 0, D = 0, E = -36 };
                break;
            case 3: 
                GeneralEquation = new GeneralEquation { A = 1, B = 0, C = 0, D = -1, E = 0 };
                break;
        }

        if (GeneralEquation != null)
        {
            Conic = new Conic(GeneralEquation);
            CanonicalEquation = new CanonicalEquation { 
                ConicType = Conic.Type, 
                FormattedString = "Mock Form" 
            };
        }
    }

    [RelayCommand]
    private void CorroborateAll()
    {
        ConicVM.Corroborate();
        LimitVM.Corroborate();
    }
}
