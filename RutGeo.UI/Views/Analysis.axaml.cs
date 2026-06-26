using Avalonia.Controls;

namespace RutGeo.UI.Views;

public partial class Analysis : UserControl
{
    public Analysis()
    {
        InitializeComponent();
    }

    public void SwitchToConics()
    {
        ConicContent.IsVisible = true;
        LimitContent.IsVisible = false;
    }

    public void SwitchToLimits()
    {
        ConicContent.IsVisible = false;
        LimitContent.IsVisible = true;
    }
}
