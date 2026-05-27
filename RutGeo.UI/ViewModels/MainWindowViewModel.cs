using RutGeo.Core.Interfaces;

namespace RutGeo.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IRutValidator _rutValidator;

    public MainWindowViewModel(IRutValidator rutValidator)
    {
        _rutValidator = rutValidator;
    }
}