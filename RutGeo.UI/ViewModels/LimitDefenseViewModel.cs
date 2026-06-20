using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RutGeo.UI.ViewModels;

public partial class LimitDefenseViewModel : ObservableObject
{
    [ObservableProperty] private string _leftLimitStatus = string.Empty;
    [ObservableProperty] private string _rightLimitStatus = string.Empty;
    [ObservableProperty] private string _limitExistsStatus = string.Empty;
    [ObservableProperty] private string _continuousStatus = string.Empty;
    [ObservableProperty] private string _discontinuityStatus = string.Empty;
    [ObservableProperty] private string _limitJustificationStatus = string.Empty;

    [RelayCommand]
    public void Corroborate()
    {
        LeftLimitStatus = "✓";
        RightLimitStatus = "✓";
        LimitExistsStatus = "✓";
        ValueFAStatus = "✓";
        ContinuousStatus = "✓";
        DiscontinuityStatus = "✓";
        LimitJustificationStatus = "✓";
    }
}
