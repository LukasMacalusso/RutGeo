using Avalonia.Controls;

namespace RutGeo.UI.Views;

public partial class Defense : UserControl
{
    public Defense()
    {
        InitializeComponent();
    }

    public void SwitchToConics()
    {
        ConicDefense.IsVisible = true;
        LimitDefense.IsVisible = false;
    }

    public void SwitchToLimits()
    {
        ConicDefense.IsVisible = false;
        LimitDefense.IsVisible = true;
    }
}
