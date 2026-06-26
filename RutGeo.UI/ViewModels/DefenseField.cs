using CommunityToolkit.Mvvm.ComponentModel;
using RutGeo.Core.Helpers;

namespace RutGeo.UI.ViewModels;

public partial class DefenseField : ObservableObject
{
    [ObservableProperty] private string _userValue = "";
    [ObservableProperty] private string _expectedValue = "";
    [ObservableProperty] private string _status = "";
    [ObservableProperty] private bool _isVisible = true;

    public void Corroborate()
    {
        if (!IsVisible)
        {
            Status = "✓";
            return;
        }

        Status = ValueComparer.AreEqual(UserValue, ExpectedValue) ? "✓" : "✗";
    }
}
