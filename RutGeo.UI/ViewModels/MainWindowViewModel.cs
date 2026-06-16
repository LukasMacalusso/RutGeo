using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RutGeo.Core.Interfaces;

namespace RutGeo.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IRutValidator _rutValidator;
    private readonly IExplanationLog _explanationLog;
    
    [ObservableProperty]
    private string _rutText = string.Empty;
    [ObservableProperty]
    private string _validationMessage = string.Empty;

    public MainWindowViewModel(IRutValidator rutValidator, IExplanationLog explanationLog)
    {
        _rutValidator = rutValidator;
        _explanationLog = explanationLog;
    }


    [RelayCommand]
    private void AnalyzeRut()
    {
        if (string.IsNullOrWhiteSpace(RutText))
        {
            ValidationMessage = "No se ingresó ningún RUT";
            return;
        }
        
        _explanationLog.Clear();
        
        var result = _rutValidator.Validate(RutText, _explanationLog);
        
        if (result.IsValid) ValidationMessage = $"RUT correcto: {result.RutBody}-{result.Dv}\n";
        else ValidationMessage = $"RUT inválido\n";

        ValidationMessage += $"\n{_explanationLog.GetFullLog()}";
    }
}