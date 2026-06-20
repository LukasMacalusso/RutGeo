using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RutGeo.UI.ViewModels;

public partial class ConicDefenseViewModel : ObservableObject
{
    [ObservableProperty] private string _centerStatus = string.Empty;
    [ObservableProperty] private string _verticesStatus = string.Empty;
    [ObservableProperty] private string _focalsStatus = string.Empty;
    [ObservableProperty] private string _axisStatus = string.Empty;
    [ObservableProperty] private string _directiveStatus = string.Empty;
    [ObservableProperty] private string _justificationStatus = string.Empty;

    [RelayCommand]
    public void Corroborate()
    {
        CenterStatus = "✓";
        VerticesStatus = "✓";
        FocalsStatus = "✓";
        AxisStatus = "✓";
        DirectiveStatus = "✓";
        JustificationStatus = "✓";
    }
}
