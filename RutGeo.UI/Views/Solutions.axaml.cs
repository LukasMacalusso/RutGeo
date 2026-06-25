using Avalonia.Controls;

namespace RutGeo.UI.Views;

public partial class Solutions : UserControl
{
    public Solutions()
    {
        InitializeComponent();
    }

    public void SwitchToConics()
    {
        ConicSolutions.IsVisible = true;
        LimitSolutions.IsVisible = false;
    }

    public void SwitchToLimits()
    {
        ConicSolutions.IsVisible = false;
        LimitSolutions.IsVisible = true;
    }
}
