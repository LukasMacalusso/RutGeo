using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RutGeo.Core.Interfaces;

namespace RutGeo.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IRutValidator _rutValidator;

    [ObservableProperty]
    private string _rutInput = string.Empty;

    [ObservableProperty]
    private string _validationMessage = "Ingrese un RUT para validarlo.";

    [ObservableProperty]
    private bool _isValid;

    public MainWindowViewModel(IRutValidator rutValidator)
    {
        _rutValidator = rutValidator;
    }

    [RelayCommand]
    private void ValidateRut()
    {
        var result = _rutValidator.Validate(RutInput);

        IsValid = result.IsValid;
        ValidationMessage = result.IsValid
            ? "El RUT es válido."
            : "El RUT no es válido.";
    }
}